using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{ 
    public class AccountNavigationView : BaseViewModel
    {
        private AccountMode mode;
        public AccountMode Mode
        {
            get { return mode; }
            set
            {
                SetProperty(ref mode, value);
                ModeChanged?.Invoke(this, value);
            }
        }

        public AccountNavigationView()
        {
            Mode = AccountMode.MAIN;
        }

        public event EventHandler<AccountMode> ModeChanged;
    }

    public class ChangePasswordView : BaseErrorViewModel
    {
        private UserModel model;

        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public StringInputField Password { get; }
        public StringInputField NewPassword { get; }
        public StringInputField ConfirmNewPassword { get; }
        public ICommand SubmitCommand { get; }

        public ChangePasswordView(UserModel model)
            :base()
        {
            this.model = model;
            Password = new StringInputField(nameof(Password), 8, 32);
            NewPassword = new StringInputField(nameof(NewPassword), 8, 32);
            ConfirmNewPassword = new StringInputField(nameof(ConfirmNewPassword), 8, 32);
            
        }

        private void handleSubmitCommand()
        {
            if (hasErrors()) { return; }
            TokenIDMessageResult res = Utility.attemptChangePassword(model.Name, Password.Value, NewPassword.Value);
            if (res.Error != null) { Error = res.Error.Message; }
            else { Message = res.Message; }
        }

        private bool hasErrors()
        {
            if (NewPassword.Value.Equals(Password.Value)) { NewPassword.Error = "must enter a new password"; }
            if (NewPassword.Value.Equals(ConfirmNewPassword.Value)) { ConfirmNewPassword.Error = "password must match"; }
            return (Password.Error != string.Empty || NewPassword.Error != string.Empty || ConfirmNewPassword.Error != string.Empty);
        }
    }

    public class RemoveAccountView : BaseErrorViewModel
    {
        public StringInputField Username { get; }
        public StringInputField Password { get; }

        public ICommand DeleteCommand { get; set; }

        public RemoveAccountView()
        {
            Username = new StringInputField("Username", 4, 32);
            Password = new StringInputField("Password", 8, 32);
        }

        private void handleDeleted()
        {
            MessageResult res = Utility.attemptRemoveAccount(Username.Value, Password.Value);

            if (res.Error != null) { Error = res.Error.Message; }
            else { Deleted?.Invoke(this, new EventArgs()); }
        }

        public event EventHandler Deleted;
    }

    public class MainAccountView : BaseViewModel
    {
        private UserModel model;

        public string Name
        {
            get { return model.Name; }
        }

        public string Admin
        {
            get { return (model.Type == 0) ? "False" : "True"; }
        }
        public ICommand LogOutCommand { get; set; }

        public MainAccountView(UserModel model)
        {
            LogOutCommand = new RelayCommand(handleLogOut);
        }

        private void handleLogOut()
        {
            LoggedOut?.Invoke(this, new EventArgs());
        }

        public event EventHandler LoggedOut;
    }
}
