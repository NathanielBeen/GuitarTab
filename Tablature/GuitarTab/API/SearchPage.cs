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

        public NotificationField<string> Error;

        public SearchParametersViewModel Search { get; set; }
        public BaseModelCollection<SongModel> Results { get; set; }

        public ICommand SearchCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public SearchPage(SearchParametersViewModel search, BaseViewModelFactory<SongModel> factory)
        {
            Search = search;
            Results = new BaseModelCollection<SongModel>(factory, handleSongSelected);

            initFields();
            initCommands();
        }

        private void initFields()
        {
            Error = new NotificationField<string>();
        }

        private void initCommands()
        {
            SearchCommand = new RelayCommand(handleSearch);
            ClearCommand = new RelayCommand(handleClear);
        }

        public void addSimpleSearchTerms(string name, Result<SongModel> results)
        {
            Search.Name.Value = name;
            Error.Value = (results.Error == null) ? String.Empty : results.Error.Message;
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
                Error.Value = res.Error.Message;
            }
            else
            {
                Error.Value = String.Empty;
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
