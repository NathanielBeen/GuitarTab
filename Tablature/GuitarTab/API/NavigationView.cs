using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class NavigationView : BaseViewModel
    {
        private Credentials credentials;

        private bool logged_in;
        public bool LoggedIn
        {
            get { return logged_in; }
            set { SetProperty(ref logged_in, value); }
        }

        private bool logged_in_admin;
        public bool LoggedInAdmin
        {
            get { return logged_in_admin; }
            set { SetProperty(ref logged_in_admin, value); }
        }

        public ICommand PopupCommand { get; set; }
        public ICommand AdminCommand { get; set; }

        public NavigationView(Credentials cred)
        {
            credentials = cred;
            credentials.LoggedIn += handleLogin;
            credentials.LoggedOut += handleLogout;

            PopupCommand = new RelayCommand(() => PopupLaunched?.Invoke(this, null));
            AdminCommand = new RelayCommand(() => AdminSelected?.Invoke(this, null));
        }

        private void handleLogin(object sender, EventArgs args)
        {
            LoggedIn = (credentials.CurrentUser != null);
            LoggedInAdmin = (credentials.CurrentUser != null && credentials.CurrentUser.Type != 0);
        }

        private void handleLogout(object sender, EventArgs args)
        {
            LoggedIn = false;
            LoggedInAdmin = false;
        }

        public event EventHandler PopupLaunched;
        public event EventHandler AdminSelected;
    }
}
