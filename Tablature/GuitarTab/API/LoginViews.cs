using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class LoginView : BaseViewModel
    {
        private Credentials credentials;

        public NotificationField<string> Error { get; private set; }
        public StringInputField Username { get; private set; }
        public StringInputField Password { get; private set; }

        public ICommand SubmitCommand { get; private set; }
        public ICommand GoToSignupCommand { get; private set; }

        public LoginView(Credentials cred)
        {
            credentials = cred;
            initFields();
            initCommands();
        }

        private void initFields()
        {
            Error = new NotificationField<string>();
            Username = new StringInputField(nameof(Username), 4, 32);
            Password = new StringInputField(nameof(Password), 8, 32);
        }

        private void initCommands()
        {
            SubmitCommand = new RelayCommand(handleSubmit);
            GoToSignupCommand = new RelayCommand(handleGoToSignup);
        }
        
        private void handleSubmit()
        {
            if (hasErrors()) { return; }
            LoginResult res = Utility.attemptLogin(Username.getConvertedField(), Password.getConvertedField());

            if (res.Error != null) { Error.Value = res.Error.Message; }
            else
            {
                Error.Value = String.Empty;
                credentials.Login(res);
            }
        }

        private void handleGoToSignup() { GoToSignup?.Invoke(this, new EventArgs()); }

        public bool hasErrors()
        {
            return (Username.hasErrors() || Password.hasErrors());
        }

        public event EventHandler GoToSignup;
    }

    public class SignUpView : BaseViewModel
    {
        private Credentials credentials;

        public NotificationField<string> Error { get; private set; }

        public StringInputField Username { get; private set; }
        public StringInputField Password { get; private set; }
        public StringInputField ConfirmPassword { get; private set; }

        public ICommand SubmitCommand { get; private set; }
        public ICommand GoToLoginCommand { get; private set; }

        public SignUpView(Credentials cred)
        {
            credentials = cred;
            initFields();
            initCommands();
        }

        private void initFields()
        {
            Error = new NotificationField<string>();
            Username = new StringInputField(nameof(Username), 4, 32);
            Password = new StringInputField(nameof(Password), 8, 32);
            ConfirmPassword = new StringInputField(nameof(ConfirmPassword), 8, 32);
        }

        private void initCommands()
        {
            SubmitCommand = new RelayCommand(handleSubmit);
            GoToLoginCommand = new RelayCommand(handleGoToLogin);
        }

        private void handleSubmit()
        {
            if (hasError()) { return; }
            LoginResult res = Utility.attemptSignUp(Username.getConvertedField(), Password.getConvertedField());

            if (res.Error != null) { Error.Value = res.Error.Message; }
            else
            {
                Error.Value = String.Empty;
                credentials.Login(res);
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

        public event EventHandler GoToLogin;
    }
}
