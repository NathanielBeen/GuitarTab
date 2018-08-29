using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{ 
    public class ChangePasswordView : BaseViewModel
    {
        private Credentials credentials;

        public NotificationField<string> Error { get; private set; }
        public NotificationField<string> Message { get; private set; }

        public StringInputField Password { get; private set; }
        public StringInputField NewPassword { get; private set; }
        public StringInputField ConfirmNewPassword { get; private set; }

        public ICommand SubmitCommand { get; private set; }

        public ChangePasswordView(Credentials cred)
            :base()
        {
            credentials = cred;
            initFields();
            initCommands();
        }

        private void initFields()
        {
            Error = new NotificationField<string>();
            Message = new NotificationField<string>();
            Password = new StringInputField(nameof(Password), 8, 32);
            NewPassword = new StringInputField(nameof(NewPassword), 8, 32);
            ConfirmNewPassword = new StringInputField(nameof(ConfirmNewPassword), 8, 32);
        }

        private void initCommands()
        {
            SubmitCommand = new RelayCommand(handleSubmitCommand);
        }

        private void handleSubmitCommand()
        {
            if (hasErrors()) { return; }
            TokenIDMessageResult res = Utility.attemptChangePassword(credentials.CurrentUser.Name, Password.Value, NewPassword.Value);
            if (res.Error != null) { Error.Value = res.Error.Message; }
            else { Message.Value = res.Message; }
        }

        private bool hasErrors()
        {
            if (NewPassword.Value.Equals(Password.Value)) { NewPassword.Error = "must enter a new password"; }
            if (NewPassword.Value.Equals(ConfirmNewPassword.Value)) { ConfirmNewPassword.Error = "password must match"; }
            return (Password.hasErrors() || NewPassword.hasErrors() || ConfirmNewPassword.hasErrors());
        }
    }

    public class RemoveAccountView : BaseViewModel
    {
        private Credentials credentials;

        public NotificationField<string> Error { get; private set; }
        public StringInputField Username { get; private set; }
        public StringInputField Password { get; private set; }

        public ICommand DeleteCommand { get; set; }

        public RemoveAccountView(Credentials cred)
        {
            credentials = cred;
            initFields();
            initCommands();
        }

        private void initFields()
        {
            Error = new NotificationField<string>();
            Username = new StringInputField("Username", 4, 32);
            Password = new StringInputField("Password", 8, 32);
        }

        private void initCommands()
        {
            DeleteCommand = new RelayCommand(handleDeleted);
        }

        private void handleDeleted()
        {
            MessageResult res = Utility.attemptRemoveAccount(Username.Value, Password.Value);

            if (res.Error != null) { Error.Value = res.Error.Message; }
            else { credentials.LogOut(); }
        }
    }

    public class MainAccountView : BaseViewModel
    {
        private Credentials credentials;

        public string Name
        {
            get { return credentials.CurrentUser.Name; }
        }

        public bool Admin
        {
            get { return (credentials.CurrentUser.Type == 0); }
        }
        public ICommand LogOutCommand { get; set; }

        public MainAccountView(Credentials cred)
        {
            credentials = cred;
            LogOutCommand = new RelayCommand(handleLogOut);
        }

        private void handleLogOut()
        {
            LoggedOut?.Invoke(this, new EventArgs());
        }

        public event EventHandler LoggedOut;
    }

    public class AccountRatingsView : BaseViewModel
    {
        private UserModel model;

        public BaseModelCollection<RatingModel> Ratings { get; set; }

        public AccountRatingsView(UserModel m, BaseViewModelFactory<RatingModel> factory)
        {
            model = m;
            Ratings = new BaseModelCollection<RatingModel>(factory, handleRatingSelected);
            populateRatings();
        }

        private void populateRatings()
        {
            Result<RatingModel> ratings = APIRequest.getRatingsByUserId(model.Id).GetAwaiter().GetResult();
            if (ratings.Error == null) { Ratings.populateModels(ratings.Items); }
        }

        private void handleRatingSelected(RatingModel model)
        {
            if (model != null) { RatingSelected?.Invoke(this, model); }
        }

        public event EventHandler<RatingModel> RatingSelected;
    }
}
