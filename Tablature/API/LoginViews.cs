using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class LoginView : BaseErrorViewModel
    {
        public StringInputField Username { get; }
        public StringInputField Password { get; }

        public ICommand SubmitCommand { get; set; }
        public ICommand GoToSignupCommand { get; set; }

        public LoginView()
        {
            Username = new StringInputField(nameof(Username), 4, 32);
            Password = new StringInputField(nameof(Password), 8, 32);

            SubmitCommand = new RelayCommand(handleSubmit);
            GoToSignupCommand = new RelayCommand(handleGoToSignup);
        }
        
        private void handleSubmit()
        {
            if (hasErrors()) { return; }
            LoginResult res = Utility.attemptLogin(Username.getConvertedField(), Password.getConvertedField());

            if (res.Error != null) { Error = res.Error.Message; }
            else
            {
                Error = string.Empty;
                Login?.Invoke(this, res);
            }
        }

        private void handleGoToSignup() { GoToSignup?.Invoke(this, new EventArgs()); }

        public bool hasErrors()
        {
            return (Username.hasErrors() || Password.hasErrors());
        }

        public event EventHandler<LoginResult> Login;
        public event EventHandler GoToSignup;
    }

    public class SignUpView : BaseErrorViewModel
    {
        public StringInputField Username { get; }
        public StringInputField Password { get; }
        public StringInputField ConfirmPassword { get; }

        public ICommand SubmitCommand { get; set; }
        public ICommand GoToLoginCommand { get; set; }

        public SignUpView()
        {
            Username = new StringInputField(nameof(Username), 4, 32);
            Password = new StringInputField(nameof(Password), 8, 32);
            ConfirmPassword = new StringInputField(nameof(ConfirmPassword), 8, 32);

            SubmitCommand = new RelayCommand(handleSubmit);
            GoToLoginCommand = new RelayCommand(handleGoToLogin);
        }

        private void handleSubmit()
        {
            if (hasError()) { return; }
            LoginResult res = Utility.attemptSignUp(Username.getConvertedField(), Password.getConvertedField());

            if (res.Error != null) { Error = res.Error.Message; }
            else
            {
                Error = string.Empty;
                SignedUp?.Invoke(this, res);
            }
        }

        private void handleGoToLogin()
        {
            GoToLogin?.Invoke(this, new EventArgs());
        }

        private bool hasError()
        {
            if (!Password.Value.Equals(ConfirmPassword.Value)) { ConfirmPassword.Error = "passwords do not match"; }

            return (Username.hasErrors() || Password.hasErrors() || ConfirmPassword.hasErrors());
        }

        public event EventHandler<LoginResult> SignedUp;
        public event EventHandler GoToLogin;
    }
}
