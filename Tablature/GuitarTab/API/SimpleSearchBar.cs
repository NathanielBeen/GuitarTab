using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class SimpleSearchBar : BaseViewModel
    {
        public StringInputField Name { get; }
        public ICommand SearchCommand { get; }
        public ICommand AdvancedCommand { get; }

        public SimpleSearchBar()
        {
            Name = new StringInputField("Name", 0, 32);
            SearchCommand = new RelayCommand(handleSearch);
            AdvancedCommand = new RelayCommand(handleAdvancedSearch);
        }

        private void handleSearch()
        {
            if (Name.hasErrors()) { return; }
            var terms = new SearchTerms(Name.Value, null, null, 0, new string[] { });

            Result<SongModel> res = Utility.attemptSearch(terms);
            Searched?.Invoke(this, res);
        }

        private void handleAdvancedSearch() { AdvancedSearch?.Invoke(this, null); }

        public event EventHandler<Result<SongModel>> Searched;
        public event EventHandler AdvancedSearch;
    }
}
