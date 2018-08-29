using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class SearchPage : BaseInputViewModel
    {
        private string error;
        public string Error
        {
            get { return error; }
            set { SetProperty(ref error, value); }
        }

        public SearchParametersViewModel Search { get; set; }
        public BaseModelCollection<SongModel> Results { get; set; }

        public ICommand SearchCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public SearchPage(SearchParametersViewModel search, BaseViewModelFactory<SongModel> factory)
        {
            Error = String.Empty;
            Search = search;
            Results = new BaseModelCollection<SongModel>(factory, handleSongSelected);

            SearchCommand = new RelayCommand(handleSearch);
            ClearCommand = new RelayCommand(handleClear);
        }

        public void addSimpleSearchTerms(string name, Result<SongModel> results)
        {
            Search.Name.Value = name;
            Error = (results.Error == null) ? String.Empty : results.Error.Message;
            Results.populateModels(results.Items);
        }

        public void handleSongSelected(SongModel selected)
        {
            if (selected != null)
            {
                Selected?.Invoke(this, selected);
            }
        }

        public void handleSearch()
        {
            SearchTerms terms = Search.getSearchTerms();
            Result<SongModel> res = Utility.attemptSearch(terms);

            if (res.Error != null)
            {
                Error = res.Error.Message;
            }
            else
            {
                Error = String.Empty;
                Results.populateModels(res.Items);
            }
        }

        public void handleClear()
        {
            Search.clearSearch();
        }

        public event EventHandler<SongModel> Selected;
    }
}
