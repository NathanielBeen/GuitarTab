using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public interface IViewModeFactory<T>
    {
        BaseViewModel createView(T value);
    }

    public class AdminViewModeFactory : IViewModeFactory<AdminMode>
    {
        public AdminViewModeFactory() { }

        public BaseViewModel createView(AdminMode value)
        {
            switch (value)
            {
                case AdminMode.RATING:
                    return createRatingCollection();
                case AdminMode.SONG:
                    return createSongCollection();
                case AdminMode.TAG:
                    return createTagCollection();
                case AdminMode.USER:
                    return createUserCollection();
                default:
                    return new BaseViewModel();
            }
        }

        private BaseViewModel createRatingCollection()
        {
            var req = new RatingAdminRequestHandler();
            var fac = new RatingAdminVMFactory();
            var coll = new BaseAdminModelCollection<RatingModel>(req, fac);
            return new AdminRatingCollection(coll);
        }

        private BaseViewModel createUserCollection()
        {
            var req = new UserAdminRequestHandler();
            var fac = new UserAdminVMFactory();
            var coll = new BaseAdminModelCollection<UserModel>(req, fac);
            return new AdminUserCollection(coll);
        }

        private BaseViewModel createTagCollection()
        {
            var req = new TagAdminRequestHandler();
            var fac = new TagAdminVMFactory();
            var coll = new BaseAdminModelCollection<TagModel>(req, fac);
            return new AdminTagCollection(coll);
        }
        
        private BaseViewModel createSongCollection()
        {
            var req = new SongAdminRequestHandler();
            var fac = new SongAdminVMFactory();
            var coll = new BaseAdminModelCollection<SongModel>(req, fac);
            return new AdminSongCollection(coll);
        }
    }

    public class LoginViewModeFactory : IViewModeFactory<LoginMode>
    {
        private Credentials credentials;
        private Action<LoginMode> change_mode;

        public LoginViewModeFactory(Credentials cred, Action<LoginMode> change)
        {
            credentials = cred;
            change_mode = change;
        }

        public BaseViewModel createView(LoginMode value)
        {
            switch (value)
            {
                case LoginMode.LOGIN:
                    return createLoginView();
                case LoginMode.SIGNUP:
                    return createSignUpView();
                default:
                    return new BaseViewModel();
            }
        }

        private BaseViewModel createLoginView()
        {
            var view = new LoginView(credentials);
            view.GoToSignup += ((o, m) => change_mode?.Invoke(LoginMode.SIGNUP));
            return view;
        }

        private BaseViewModel createSignUpView()
        {
            var view = new SignUpView(credentials);
            view.GoToLogin += ((o, m) => change_mode?.Invoke(LoginMode.LOGIN));
            return view;
        }
    }

    public class MainViewModeFactory : IViewModeFactory<MainViewMode>
    {
        private Credentials credentials;

        public MainViewModeFactory(Credentials cred)
        {
            credentials = cred;
        }

        public BaseViewModel createView(MainViewMode value)
        {
            switch (value)
            {
                case MainViewMode.ADMIN:
                    if (credentials == null || credentials.CurrentUser == null
                        || credentials.CurrentUser.Type == 0)
                    {
                        return createSearchView();
                    }
                    return createAdminView();
                case MainViewMode.CREATE_SONG:
                    return createCreateSongView();
                case MainViewMode.EDIT_SONG:
                    return createEditSongView();
                case MainViewMode.SEARCH:
                    return createSearchView();
                case MainViewMode.VIEW_SONG:
                    return createViewSongView();
                default:
                    return new BaseViewModel();
            }
        }

        private BaseViewModel createAdminView()
        {
            var factory = new AdminViewModeFactory();
            return new AdminPage(factory);
        }

        private BaseViewModel createSearchView()
        {
            var tag = new TagSearch(APIRequest.getAllTags().GetAwaiter().GetResult().Items);
            var parameters = new SearchParametersViewModel(tag);
            var factory = new SongVMFactory();
            return new SearchPage(parameters, factory);
        }

        private BaseViewModel createCreateSongView()
        {

        }

        private BaseViewModel createEditSongView()
        {

        }

        private BaseViewModel createViewSongView()
        {

        }
    }

    public class PopupViewModeFactory : IViewModeFactory<PopupViewMode>
    {
        private Credentials credentials;

        public PopupViewModeFactory(Credentials cred)
        {
            credentials = cred;
        }

        public BaseViewModel createView(PopupViewMode value)
        {
            switch (value)
            {
                case PopupViewMode.LOGIN:
                    return createLoginPage();
                case PopupViewMode.USER:
                    return createAccountPage();
                default:
                    return new BaseViewModel();
            }
        }

        private BaseViewModel createLoginPage() { return new LoginPage(credentials); }

        private BaseViewModel createAccountPage() { return new AccountPage(credentials); }
    }
}
