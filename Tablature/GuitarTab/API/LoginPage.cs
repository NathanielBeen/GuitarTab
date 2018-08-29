using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab.API
{
    public enum LoginMode
    {
        LOGIN = 0,
        SIGNUP = 1
    }

    public class LoginPage : BaseViewModel
    {
        private Credentials credentials;
        private IViewModeFactory<LoginMode> factory;

        public NotificationField<LoginMode> Mode { get; private set; }
        public NotificationField<Visibility> Visible { get; private set; }

        public BaseViewModel CurrentView { get; set; }

        public LoginPage(Credentials cred)
        {
            factory = new LoginViewModeFactory(cred, handleViewModeChanged);
            credentials = cred;
            credentials.LoggedIn += handleLoggedIn;
            credentials.LoggedOut += handleLoggedOut;

            initFields();
            Mode.setBackingField(LoginMode.LOGIN);
            Visible.Value = Visibility.Collapsed;
        }

        private void initFields()
        {
            Mode = new NotificationField<LoginMode>(handleViewModeChanged);
            Visible = new NotificationField<Visibility>();
        }

        private void handleViewModeChanged(LoginMode new_mode)
        {
            if (credentials.isLoggedIn())
            {
                Visible.Value = Visibility.Collapsed;
            }
            else if (new_mode != Mode.Value)
            { 
                Visible.Value = Visibility.Visible;
                CurrentView = factory.createView(new_mode);
            }
        }

        private void handleLoggedIn(object sender, EventArgs args)
        {
            Visible.Value = Visibility.Collapsed;
        }

        private void handleLoggedOut(object sender, EventArgs args)
        {
            Mode.setBackingField(LoginMode.LOGIN);
            Visible.Value = Visibility.Collapsed;
        }
    }
}
