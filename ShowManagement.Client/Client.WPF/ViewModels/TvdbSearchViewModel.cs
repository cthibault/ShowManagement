using Microsoft.Practices.Unity;
using ShowManagement.Business.Models;
using System;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShowManagement.Client.WPF.Models;
using ShowManagement.Client.WPF.Services;

namespace ShowManagement.Client.WPF.ViewModels
{
    interface ITvdbSearchViewModel
    {

    }

    class TvdbSearchViewModel : BaseViewModel, ITvdbSearchViewModel
    {
        public TvdbSearchViewModel(IUnityContainer unityContainer, ITvdbSearchProvider tvdbSearchProvider, ShowViewModel showViewModel)
            : base(unityContainer)
        {
            if (showViewModel == null)
            {
                throw new ArgumentNullException("showViewModel");
            }

            if (tvdbSearchProvider == null)
            {
                throw new ArgumentNullException("tvdbSearchProvider");
            }

            // BusyContextManager just for this ViewModel
            this.BusyContextManager = new Infrastructure.BusyContextManager();

            this.ShowViewModel = showViewModel;
            this.SearchProvider = tvdbSearchProvider;

            this.DefineData();
            this.DefineCommands();
        }

        private void DefineData()
        {
            this.VisibleSearchResults = this._searchResults.CreateDerivedCollection(sr => sr, null, (sr1, sr2) => sr1.Title.CompareTo(sr2.Title));
        }

        private void DefineCommands()
        {
            //TODO: Exception Handling?

            this.OpenCommand = ReactiveCommand.Create();
            this.OpenCommand.Subscribe(_ => this.OnOpenSearch());

            this.SearchCommand = ReactiveCommand.CreateAsyncTask(async x => await this.OnSearch());
            this.SearchCommand.Subscribe(async results => await this.OnSearchComplete(results));

            this.OkCommand = ReactiveCommand.Create(this.WhenAny(vm => vm.SelectedResult, vm => vm.GetValue() != null));
            this.OkCommand.Subscribe(_ => this.OnOk());

            this.CancelCommand = ReactiveCommand.Create();
            this.CancelCommand.Subscribe(_ => this.OnCancel());
        }

        #region Open
        public ReactiveCommand<object> OpenCommand { get; private set; }
        private void OnOpenSearch()
        {
            if (!this.ShowViewModel.Name.StartsWith(ShowViewModel.NEWSHOW_NAME))
            {
                this.TitleSearchText = this.ShowViewModel.Name;
            }

            this.IsOpen = true;
        }
        #endregion

        #region Close
        private void Close()
        {
            this.IsOpen = false;

            this._searchResults.Clear();
            this.TitleSearchText = string.Empty;
        } 
        #endregion

        #region Search
        public ReactiveCommand<Tuple<List<SeriesSearchResult>, BusyContext>> SearchCommand { get; private set; }

        private async Task<Tuple<List<SeriesSearchResult>, BusyContext>> OnSearch()
        {
            List<SeriesSearchResult> results = null;

            var busyContext = this.AddBusyContext("Searching Tvdb for Series Information...");

            if (!string.IsNullOrEmpty(this.TitleSearchText))
            {
                results = await this.SearchProvider.SearchForSeries(this.TitleSearchText);
            }
            else
            {
                results = new List<SeriesSearchResult>(0);
            }

            return new Tuple<List<SeriesSearchResult>, BusyContext>(results, busyContext);
        }
        private async Task OnSearchComplete(Tuple<List<SeriesSearchResult>, BusyContext> results)
        {
            if (results != null)
            {
                using (results.Item2)
                {
                    using (var context = this.AddBusyContext("Populating Series Information..."))
                    {
                        this._searchResults.Clear();

                        if (results.Item1 != null)
                        {
                            this._searchResults.AddRange(results.Item1);
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentNullException("results");
            }
        }
        #endregion

        #region Ok
        public ReactiveCommand<object> OkCommand { get; private set; }
        private void OnOk()
        {
            if (this.SelectedResult != null)
            {
                this.ShowViewModel.TvdbId = this.SelectedResult.TvdbId;
                this.ShowViewModel.ImbdId = this.SelectedResult.ImdbId;
                
                if (this.ShowViewModel.Name.StartsWith(ShowViewModel.NEWSHOW_NAME))
                {
                    this.ShowViewModel.Name = this.SelectedResult.Title;
                }
            }

            this.Close();
        }
        #endregion

        #region Cancel
        public ReactiveCommand<object> CancelCommand { get; private set; }
        private void OnCancel()
        {
            //TODO: What happens when the search is still running?  How should it be canceled?

            this.Close();
        }
        #endregion


        public bool IsOpen
        {
            get { return this._isOpen; }
            set { this.RaiseAndSetIfChanged(ref this._isOpen, value); }
        }
        private bool _isOpen;

        public string TitleSearchText
        {
            get { return this._titleSearchText; }
            set { this.RaiseAndSetIfChanged(ref this._titleSearchText, value); }
        }
        private string _titleSearchText;

        public IReactiveDerivedList<SeriesSearchResult> VisibleSearchResults { get; private set; }
        private ReactiveList<SeriesSearchResult> _searchResults = new ReactiveList<SeriesSearchResult>();

        public SeriesSearchResult SelectedResult
        {
            get { return this._selectedResult; }
            set { this.RaiseAndSetIfChanged(ref this._selectedResult, value); }
        }
        private SeriesSearchResult _selectedResult;

        private ShowViewModel ShowViewModel { get; set; }
        private ITvdbSearchProvider SearchProvider { get; set; }
    }
}
