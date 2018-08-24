using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private UserModel model;

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

        public BaseViewModel CurrentView { get; set; }

        public AccountPage(UserModel model)
        {
            this.model = model;
            CurrentView = new BaseViewModel();
            Mode = AccountMode.MAIN;
        }

        private void handleViewModeChanged(AccountMode new_mode)
        {
            if (new_mode == Mode) { return; }
            switch (new_mode)
            {
                case AccountMode.MAIN:
                    CurrentView = createMainView();
                    return;
                case AccountMode.CHANGE_PASSWORD:
                    CurrentView = new ChangePasswordView(model);
                    return;
                case AccountMode.VIEW_RATINGS:
                    CurrentView = createRatingView();
                    return;
                case AccountMode.REMOVE:
                    CurrentView = createRemoveView();
                    return;
            }
        }

        private MainAccountView createMainView()
        {
            var view = new MainAccountView(model);
            view.LoggedOut += handleLogoutOrAccountRemoved;
            return view;
        }

        private BaseModelCollection<RatingModel> createRatingView()
        {
            var factory = new RatingVMFactory();
            var collection = new BaseModelCollection<RatingModel>(factory, handleRatingSelected);

            collection.populateModels(APIRequest.getRatingsByUserId(model.Id).GetAwaiter().GetResult().Items);
            return collection;
        }

        private RemoveAccountView createRemoveView()
        {
            var view = new RemoveAccountView();
            view.Deleted += handleLogoutOrAccountRemoved;
            return view;
        }

        private void handleRatingSelected(RatingModel model)
        {
            //fire event or something that prompts the main screen to go to the rating
        }

        private void handleLogoutOrAccountRemoved(object sender, EventArgs args)
        {
            //probably fire an event that collapses menu and adjusts current user (maybe have reference to
            // current user in the view and that adjusts so all other references recieve it
        }
    }
}
