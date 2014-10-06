using ReactiveUI;
using ShowManagement.Business.Models;
using ShowManagement.Client.WPF.Models;
using ShowManagement.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShowManagement.Client.WPF.ViewModels
{
    class ShowViewModel : TrackableObject
    {
        public ShowViewModel(ShowInfo showInfo)
            : base(true)
        {
            this.SelectedCommand = ReactiveCommand.Create();

            this.WhenAnyValue(svm => svm.HasChanges)
                .Select(x => x || this.IsNew)
                .ToProperty(this, svm => svm.NeedsToBeSaved, out this._needsToBeSaved);

            this.WhenAnyValue(svm => svm.NeedsToBeSaved)
                .Select(x => string.Format("{0}{1}", this.Name, x ? "*" : string.Empty))
                .ToProperty(this, svm => svm.DisplayName, out this._displayName);

            this._model = showInfo;
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

        public string ImbdbId
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
        #endregion

        public bool IsNew
        {
            get { return this.ShowId == 0; }
        }

        public bool NeedsToBeSaved
        {
            get { return this._needsToBeSaved.Value; }
        }
        readonly ObservableAsPropertyHelper<bool> _needsToBeSaved;

        public string DisplayName
        {
            get { return this._displayName.Value; }
        }
        readonly ObservableAsPropertyHelper<string> _displayName;

        public void Update(ShowInfo showInfo)
        {
            this._model = showInfo;

            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.ShowId));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.TvdbId));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.ImbdbId));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Name));
            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.Directory));

            this.RaisePropertyChanged(this.ExtractPropertyName(x => x.IsNew));
        }

        public ReactiveCommand<object> SelectedCommand { get; private set; }

        private ShowInfo _model;
    }
}
