using Entities.Pattern;
using Microsoft.Practices.Unity;
using ReactiveUI;
using ShowManagement.Business.Models;
using ShowManagement.Client.WPF.Infrastructure;
using ShowManagement.Client.WPF.Models;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    class ShowViewModel : TrackableObject
    {
        public ShowViewModel(IUnityContainer unityContainer, Services.IServiceProvider serviceProvider, BusyContextManager busyContextManager, ShowInfo showInfo)
            : base(unityContainer, true)
        {
            this._serviceProvider = serviceProvider;
            this._model = showInfo;
            this.ParserModels.AddRange(showInfo.Parsers);

            this.IsEnabled = true;

            this.DefineData();
            this.DefineCommands();
        }

        private void DefineData()
        {
            this.ParserModels.ItemsAdded.Subscribe(p => this.Model.Parsers.Add(p));
            this.ParserViewModels = this.ParserModels.CreateDerivedCollection(
                p =>
                {
                    var vm = new ParserViewModel(this.UnityContainer, p);

                    return vm;
                });
            this.ParserViewModels.ChangeTrackingEnabled = true;
            this.ParsersToSave = this.ParserViewModels.CreateDerivedCollection(pvm => pvm, pvm => pvm.HasChanges || pvm.ObjectState == ObjectState.Deleted);


            this.HasChangesObservable.Subscribe(hasChanges =>
            {
                if (this.ObjectState != ObjectState.Added && this.ObjectState != ObjectState.Deleted)
                {
                    this.ObjectState = hasChanges ? ObjectState.Modified : ObjectState.Unchanged;
                }
            });

            this.NeedsToBeSavedObservable = Observable.Merge
                (
                    this.HasChangesObservable,
                    this.ObservableForProperty(
                        vm => vm.ObjectState, 
                        ob => ob == ObjectState.Deleted
                           || ob == ObjectState.Added, 
                        beforeChange:false),
                    this.ParsersToSave.WhenAny(pvm => pvm.Count, x => x.GetValue() > 0)
                );
            this.NeedsToBeSavedObservable.ToProperty(this, x => x.NeedsToBeSaved, out this._needsToBeSaved);

            // Populate the DisplayName value
            Observable.Merge
                (
                    this.NeedsToBeSavedObservable,
                    this.Changed
                        .Where(x => x.PropertyName == this.ExtractPropertyName(svm => svm.Name))
                        .Select(_ => this.Changes.Any(c => c.ValueName == this.ExtractPropertyName(svm => svm.Name)))
                )
                .Select(x => string.Format("{0}{1}", this.Name, x ? "*" : string.Empty))
                .ToProperty(this, svm => svm.DisplayName, out this._displayName);
        }
        private void DefineCommands()
        {
            var falseObservable = Observable.Return<bool>(false);


            this.SelectedCommand = ReactiveCommand.Create();
            this.CloseCommand = ReactiveCommand.Create();
            this.SaveCommand = ReactiveCommand.Create(this.NeedsToBeSavedObservable);

            this.RefreshCommand = ReactiveCommand.CreateAsyncTask(async x => await this.OnRefresh());
            this.RefreshCommand.Subscribe(async result => await this.OnRefreshComplete(result));
            this.RefreshCommand.ThrownExceptions.Subscribe(ex => { throw ex; });

            this.CancelCommand = ReactiveCommand.Create();
            this.CancelCommand.Subscribe(async _ => await this.OnCancel());

            this.DeleteCommand = ReactiveCommand.Create();
            this.DeleteCommand.Subscribe(async _ => await this.OnDelete());

            this.BrowseDirectoryCommand = ReactiveCommand.Create();
            this.BrowseDirectoryCommand.Subscribe(async _ => await this.OnBrowseDirectory());

            this.CloneCommand = ReactiveCommand.Create(falseObservable);


            this.AddParserCommand = ReactiveCommand.Create();
            this.AddParserCommand.Subscribe(async _ => await this.OnAddParser());
        }

        #region Wrapper Properties
        public int ShowId
        {
            get { return this._model.ShowId; }
        }

        public int TvdbId
        {
            get { return this._model.TvdbId; }
            set { this.LogRaiseAndSetIfChanged(this._model.TvdbId, value, v => this._model.TvdbId = v); }
        }

        public string ImbdId
        {
            get { return this._model.ImdbId; }
            set { this.LogRaiseAndSetIfChanged(this._model.ImdbId, value, v => this._model.ImdbId = v); }
        }

        public string Name
        {
            get { return this._model.Name; }
            set { this.LogRaiseAndSetIfChanged(this._model.Name, value, v => this._model.Name = v); }
        }

        public string Directory
        {
            get { return this._model.Directory; }
            set { this.LogRaiseAndSetIfChanged(this._model.Directory, value, v => this._model.Directory = v); }
        }

        public ObjectState ObjectState
        {
            get { return this._model.ObjectState; }
            //set { this.LogRaiseAndSetIfChanged(this._model.ObjectState, value, v => this._model.ObjectState = v); }
            set
            {
                if (this._model.ObjectState != value)
                {
                    this.RaisePropertyChanging(this.ExtractPropertyName(x => x.ObjectState));
                    this._model.ObjectState = value;
                    this.RaisePropertyChanged(this.ExtractPropertyName(x => x.ObjectState));
                }
            }
        }

        public int ParserCount { get { return 2; } }
        #endregion


        #region Parsers
        public IReactiveDerivedList<ParserViewModel> ParserViewModels { get; private set; }
        private ReactiveList<Parser> ParserModels = new ReactiveList<Parser>();
        private IReactiveDerivedList<ParserViewModel> ParsersToSave { get; set; }

        public ReactiveCommand<object> AddParserCommand { get; private set; }
        private async Task OnAddParser()
        {
            var parser = new Parser { ObjectState = ObjectState.Added };

            this.ParserModels.Add(parser);
        }
        #endregion


        public bool IsNew
        {
            get { return this.ShowId == 0; }
        }

        public string DisplayName
        {
            get { return this._displayName.Value; }
        }
        private ObservableAsPropertyHelper<string> _displayName;

        public bool NeedsToBeSaved
        {
            get { return this._needsToBeSaved.Value; }
        }
        private ObservableAsPropertyHelper<bool> _needsToBeSaved;
        private IObservable<bool> NeedsToBeSavedObservable { get; set; }

        public bool IsEnabled
        {
            get { return this._isEnabled; }
            set { this.RaiseAndSetIfChanged(ref this._isEnabled, value); }
        }
        private bool _isEnabled;


        #region Refresh
        public ReactiveCommand<Tuple<ShowInfo, BusyContext>> RefreshCommand { get; private set; }

        private async Task<Tuple<ShowInfo, BusyContext>> OnRefresh()
        {
            var busyContext = this.AddBusyContext(string.Format("Refreshing {0}...", this.Name));

            this.IsEnabled = false;

            ShowInfo result = await this.ServiceProvider.GetShow(this.ShowId);

            return new Tuple<ShowInfo, BusyContext>(result, busyContext);
        }
        private async Task OnRefreshComplete(Tuple<ShowInfo, BusyContext> result)
        {
            if (result != null)
            {
                using (result.Item2)
                {
                    if (result.Item1 == null)
                    {
                        throw new ArgumentNullException("result.Item1", string.Format("Failed to retrieve {0}", this.Name));
                    }

                    this.Update(result.Item1, true);

                    this.IsEnabled = true;
                }
            }
            else
            {
                throw new ArgumentNullException("result");
            }
        } 
        #endregion

        #region Save
        public ReactiveCommand<object> SaveCommand { get; private set; }


        #endregion

        #region Cancel
        public ReactiveCommand<object> CancelCommand { get; private set; }

        private async Task OnCancel()
        {
            if (this.HasChanges)
            {
                this.RefreshCommand.Execute(null);
            }

            this.CloseCommand.Execute(null);
        }
        #endregion

        #region Close
        public ReactiveCommand<object> CloseCommand { get; private set; }

        #endregion

        #region Selected
        public ReactiveCommand<object> SelectedCommand { get; private set; }

        #endregion

        #region Delete
        public ReactiveCommand<object> DeleteCommand { get; private set; }

        private async Task OnDelete()
        {
            this.CloseCommand.Execute(null);

            this.ObjectState = ObjectState.Deleted;
        }
        #endregion

        #region Clone
        public ReactiveCommand<object> CloneCommand { get; private set; }

        #endregion

        #region BrowseDirectory
        public ReactiveCommand<object> BrowseDirectoryCommand { get; private set; }

        private async Task OnBrowseDirectory()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = "Select Series Directory",
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = this.Directory
            };

            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.Directory = dialog.SelectedPath;
            }
        }
        #endregion

        public ReactiveCommand<object> SearchTvdbCommand { get; private set; }

        public void Update(ShowInfo showInfo, bool clearChanges)
        {
            this.TurnTrackChangesOff(clearChanges);

            this._model = showInfo;

            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.ShowId));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.TvdbId));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.ImbdId));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Name));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Directory));

            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.IsNew));

            this.TurnTrackChangesOn();
        }



        #region ServiceProvider
        private Services.IServiceProvider ServiceProvider
        {
            get
            {
                if (this._serviceProvider == null)
                {
                    this._serviceProvider = this.UnityContainer.Resolve<Services.IServiceProvider>();
                }
                return this._serviceProvider;
            }
            set { this._serviceProvider = value; }
        }
        private Services.IServiceProvider _serviceProvider;
        #endregion

        public ShowInfo Model
        {
            get { return this._model; }
        }
        private ShowInfo _model;
    }
}
