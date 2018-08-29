using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace API
{
    public enum LoginMode
    {
        LOGIN = 0,
        SIGNUP = 1
    }

    public class LoginPage : BaseViewModel
    {
        private Credentials credentials;

        private Visibility visible;
        public Visibility Visible
        {
            get { return visible; }
            set { SetProperty(ref visible, value); }
        }

        private LoginMode mode;
        public LoginMode Mode
        {
            get { return mode; }
            set { SetProperty(ref mode, value); }
        }

        public BaseViewModel CurrentView { get; set; }

        public LoginPage(Credentials cred)
        {
            credentials = cred;
            credentials.LoggedIn += handleLoggedIn;

            Visible = Visibility.Collapsed;

            CurrentView = createLoginView();
            Mode = LoginMode.LOGIN;
        }

        private void handleViewModeChanged(LoginMode new_mode)
        {
            if (credentials.isLoggedIn())
            {
                Visible = Visibility.Collapsed;
                return;
            }
            if (new_mode == Mode) { return; }

            switch (new_mode)
            {
                case LoginMode.LOGIN:
                    Visible = Visibility.Visible;
                    CurrentView = createLoginView();
                    Mode = new_mode;
                    return;
                case LoginMode.SIGNUP:
                    Visible = Visibility.Visible;
                    CurrentView = createSignUpView();
                    Mode = new_mode;
                    return;
            }
        }

        private LoginView createLoginView()
        {
            var view = new LoginView(credentials);
            view.GoToSignup += ((o, m) => Mode = LoginMode.SIGNUP);
            return view;
        }

        private SignUpView createSignUpView()
        {
            var view = new SignUpView(credentials);
            view.GoToLogin += ((o, m) => Mode = LoginMode.LOGIN);
            return view;
        }

        private void handleLoggedIn(object sender, EventArgs args)
        {
            Visible = Visibility.Collapsed;
        }

        private void handleLoggedOut(object sender, EventArgs args)
        {
            mode = LoginMode.LOGIN;
            Visible = Visibility.Collapsed;
        }
    }
}
