using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab.API
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

        public NotificationField<AccountMode> Mode { get; private set; }
        public NotificationField<Visibility> Visible { get; private set; }

        public BaseViewModel CurrentView { get; set; }

        public AccountPage(Credentials cred)
        {
            credentials = cred;
            credentials.LoggedOut += handleLogout;
            initFields();
        }

        private void initFields()
        {
            Mode = new NotificationField<AccountMode>(handleViewModeChanged);
            Visible = new NotificationField<Visibility>();
        }

        private void handleViewModeChanged(AccountMode new_mode)
        {
            if (!credentials.isLoggedIn())
            {
                Visible.Value = Visibility.Collapsed;
                return;
            }
            if (new_mode == Mode.Value) { return; }
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
            if (model == null) { return; }
            RatingSelected?.Invoke(this, model);
        }

        private void handleLogout(object sender, EventArgs args)
        {
            Mode.setBackingField(AccountMode.MAIN);
            Visible.Value = Visibility.Collapsed;
        }

        public event EventHandler<RatingModel> RatingSelected;
    }
}
