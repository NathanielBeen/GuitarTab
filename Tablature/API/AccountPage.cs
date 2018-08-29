using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace API
{
    public enum AccountMode
    {
        MAIN = 0,
        CHANGE_PASSWORD = 1,
        VIEW_RATINGS = 2,
        REMOVE = 3
    }

    public class AccountPage : BaseViewModel
    {
        private Credentials credentials;

        private AccountMode mode;
        public AccountMode Mode
        {
            get { return mode; }
            set
            {
                handleViewModeChanged(value);
                SetProperty(ref mode, value);
            }
        }

        private Visibility visible;
        public Visibility Visible
        {
            get { return visible; }
            set { SetProperty(ref visible, value); }
        }

        public BaseViewModel CurrentView { get; set; }

        public AccountPage(Credentials cred)
        {
            credentials = cred;
            credentials.LoggedOut += handleLogout;
            CurrentView = new BaseViewModel();
            Mode = AccountMode.MAIN;
        }

        private void handleViewModeChanged(AccountMode new_mode)
        {
            if (!credentials.isLoggedIn())
            {
                Visible = Visibility.Collapsed;
                return;
            }
            if (new_mode == Mode) { return; }
            switch (new_mode)
            {
                case AccountMode.MAIN:
                    CurrentView = new MainAccountView(credentials);
                    return;
                case AccountMode.CHANGE_PASSWORD:
                    CurrentView = new ChangePasswordView(credentials);
                    return;
                case AccountMode.VIEW_RATINGS:
                    CurrentView = createRatingView();
                    return;
                case AccountMode.REMOVE:
                    CurrentView = new RemoveAccountView(credentials);
                    return;
            }
        }

        private BaseModelCollection<RatingModel> createRatingView()
        {
            var factory = new RatingVMFactory();
            var collection = new BaseModelCollection<RatingModel>(factory, handleRatingSelected);

            collection.populateModels(APIRequest.getRatingsByUserId(credentials.CurrentUser.Id).GetAwaiter().GetResult().Items);
            return collection;
        }

        private void handleRatingSelected(RatingModel model)
        {
            //fire event or something that prompts the main screen to go to the rating
        }

        private void handleLogout(object sender, EventArgs args)
        {
            mode = AccountMode.MAIN;
            Visible = Visibility.Collapsed;
        }

        public event EventHandler<RatingModel> RatingSelected;
    }
}
