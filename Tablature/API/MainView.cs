using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace API
{
    public enum MainViewMode
    {
        SEARCH,
        SONG,
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

        private MainViewMode mode;
        public MainViewMode Mode
        {
            get { return mode; }
            set
            {
                handleViewModeChanged(value);
                SetProperty(ref mode, value);
            }
        }

        private PopupViewMode pop_mode;
        public PopupViewMode PopMode
        {
            get { return pop_mode; }
            set
            {
                handlePopupViewModeChanged(value);
                SetProperty(ref pop_mode, value);
            }
        }

        private Visibility pop_visible;
        public Visibility PopVisible
        {
            get { return pop_visible; }
            set { SetProperty(ref pop_visible, value); }
        }

        public BaseViewModel View { get; set; }
        public BaseViewModel PopView { get; set; }
        public NavigationView NavView { get; set; }

        public MainView()
        {
            credentials = new Credentials(null, null);
            NavView = new NavigationView(credentials);

            Mode = MainViewMode.SEARCH;
            PopMode = PopupViewMode.NONE;

            NavView.AdminSelected += handleAdminSelection;
            NavView.PopupLaunched += handlePopupLaunched;
        }

        private void handleViewModeChanged(MainViewMode new_mode)
        {
            if (new_mode == Mode) { return; }
            switch (new_mode)
            {
                case MainViewMode.ADMIN:
                    if (credentials.CurrentUser == null || credentials.CurrentUser.Type == 0)
                    {
                        Mode = MainViewMode.SEARCH;
                        return;
                    }
                    View = new AdminPage();
                    PopMode = PopupViewMode.NONE;
                    break;
                case MainViewMode.SEARCH:
                    View = createSearchPage();
                    PopMode = PopupViewMode.NONE;
                    break;
                case MainViewMode.SONG:
                    PopMode = PopupViewMode.NONE;
                    //insert the standard view here
                    break;
            }
        }

        private void handlePopupViewModeChanged(PopupViewMode new_mode)
        {
            if (new_mode == PopMode) { return; }
            switch (new_mode)
            {
                case PopupViewMode.NONE:
                    PopVisible = Visibility.Collapsed;
                    break;
                case PopupViewMode.LOGIN:
                    PopView = new LoginPage(credentials);
                    PopVisible = Visibility.Visible;
                    break;
                case PopupViewMode.USER:
                    PopView = new AccountPage(credentials);
                    PopVisible = Visibility.Visible;
                    break;
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
            PopMode = (credentials.CurrentUser == null) ? PopupViewMode.LOGIN : PopupViewMode.USER;
        }

        private void handleAdminSelection(object sender, EventArgs args)
        {
            if (credentials.CurrentUser != null && credentials.CurrentUser.Type != 0)
            {
                Mode = MainViewMode.ADMIN;
            }
        }
    }
}
