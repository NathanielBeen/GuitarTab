using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public enum LoginMode
    {
        LOGIN = 1,
        SIGNUP = 2
    }

    public class LoginPage : BaseViewModel
    {
        private LoginMode mode;
        public LoginMode Mode
        {
            get { return mode; }
            set { SetProperty(ref mode, value); }
        }

        public BaseViewModel CurrentView { get; set; }

        public LoginPage()
        {
            CurrentView = createLoginView();
            Mode = LoginMode.LOGIN;
        }

        private void handleViewModeChanged(LoginMode new_mode)
        {
            switch (new_mode)
            {
                case LoginMode.LOGIN:
                    CurrentView = createLoginView();
                    Mode = new_mode;
                    return;
                case LoginMode.SIGNUP:
                    CurrentView = createSignUpView();
                    Mode = new_mode;
                    return;
            }
        }

        private LoginView createLoginView()
        {
            var view = new LoginView();
            view.GoToSignup += ((o, m) => handleViewModeChanged(LoginMode.SIGNUP));
            view.Login += handleLoginOrSignUp;
            return view;
        }

        private SignUpView createSignUpView()
        {
            var view = new SignUpView();
            view.GoToLogin += ((o, m) => handleViewModeChanged(LoginMode.LOGIN));
            view.SignedUp += handleLoginOrSignUp;
            return view;
        }

        private void handleLoginOrSignUp(object sender, LoginResult result)
        {
            //fire an event or something (maybe reference current user like in the account page)
        }
    }
}
