using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GuitarTab.API
{
    public enum MainViewMode
    {
        SEARCH,
        VIEW_SONG,
        EDIT_SONG,
        CREATE_SONG,
        ADMIN
        //SONG_LISTING
        //USER_PROFILE
        //ARTIST_PAGE
    }

    public enum PopupViewMode
    {
        NONE,
        LOGIN,
        USER
    }

    public class MainView : BaseViewModel
    {
        private Credentials credentials;
        private SongModel current_song;
        private IViewModeFactory<MainViewMode> main_factory;
        private IViewModeFactory<PopupViewMode> pop_factory;

        public NotificationField<MainViewMode> Mode { get; private set; }
        public NotificationField<PopupViewMode> PopMode { get; private set; }
        public NotificationField<Visibility> PopVisible { get; private set; }

        public BaseViewModel View { get; set; }
        public BaseViewModel PopView { get; set; }
        public NavigationView NavView { get; set; }
        public SimpleSearchBar SearchView { get; set; }

        public MainView(Credentials cred, NavigationView nav, SimpleSearchBar search, IViewModeFactory<MainViewMode> fac,
            IViewModeFactory<PopupViewMode> pop)
        {
            credentials = cred;
            NavView = nav;
            SearchView = search;
            main_factory = fac;
            pop_factory = pop;

            initFields();
            Mode.Value = MainViewMode.SEARCH;
            PopMode.Value = PopupViewMode.NONE;

            NavView.AdminSelected += handleAdminSelection;
            NavView.PopupLaunched += handlePopupLaunched;
            SearchView.Searched += handleSearch;
            SearchView.AdvancedSearch += handleAdvancedSearch;
        }

        private void initFields()
        {
            Mode = new NotificationField<MainViewMode>(handleViewModeChanged);
            PopMode = new NotificationField<PopupViewMode>(handlePopupViewModeChanged);
            PopVisible = new NotificationField<Visibility>();
        }

        private void handleViewModeChanged(MainViewMode new_mode)
        {
            if (new_mode != Mode.Value)
            {
                PopMode.Value = PopupViewMode.NONE;
                View = main_factory.createView(new_mode);
            }
        }

        private void handlePopupViewModeChanged(PopupViewMode new_mode)
        {
            if (new_mode != PopMode.Value)
            {
                PopView = pop_factory.createView(new_mode);
                PopVisible.Value = (new_mode == PopupViewMode.NONE) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private SearchPage createSearchPage()
        {
            var tag = new TagSearch(APIRequest.getAllTags().GetAwaiter().GetResult().Items);
            var parameters = new SearchParametersViewModel(tag);
            var factory = new SongVMFactory();
            return new SearchPage(parameters, factory);
        }

        private void handlePopupLaunched(object sender, EventArgs args)
        {
            PopMode.Value = (credentials.CurrentUser == null) ? PopupViewMode.LOGIN : PopupViewMode.USER;
        }

        private void handleAdminSelection(object sender, EventArgs args)
        {
            if (credentials.CurrentUser != null && credentials.CurrentUser.Type != 0)
            {
                Mode.Value = MainViewMode.ADMIN;
            }
        }

        private void handleSearch(object sender, Result<SongModel> results)
        {
            Mode.Value = MainViewMode.SEARCH;
            (View as SearchPage)?.addSimpleSearchTerms(SearchView.Name.Value, results);
        }

        private void handleAdvancedSearch(object sender, EventArgs args) { Mode.Value = MainViewMode.SEARCH; }
    }
}
