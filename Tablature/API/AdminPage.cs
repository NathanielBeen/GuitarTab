using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public enum AdminMode
    {
        MAIN = 0,
        RATING = 1,
        USER = 2,
        TAG = 3,
        SONG = 4
    }

    public class AdminPage : BaseViewModel
    {
        private AdminMode mode;
        public AdminMode Mode
        {
            get { return mode; }
            set
            {
                handleViewModeChanged(value);
                SetProperty(ref mode, value);
            }
        }

        public BaseViewModel CurrentView { get; set; }
        public ICollectionUpdater UpdaterView { get; set; }

        public AdminPage()
        {
            Mode = AdminMode.MAIN;
        }

        private void handleViewModeChanged(AdminMode new_mode)
        {
            if (new_mode == Mode) { return; }
            switch (new_mode)
            {
                case AdminMode.MAIN:
                    CurrentView = new BaseViewModel();
                    return;
                case AdminMode.RATING:
                    CurrentView = createRatingCollection();
                    return;
                case AdminMode.SONG:
                    CurrentView = createSongCollection();
                    return;
                case AdminMode.TAG:
                    CurrentView = createTagCollection();
                    return;
                case AdminMode.USER:
                    CurrentView = createUserCollection();
                    return;

            }
        }

        private AdminRatingCollection createRatingCollection()
        {
            var req = new RatingAdminRequestHandler();
            var fac = new RatingAdminVMFactory();
            var coll = new BaseAdminModelCollection<RatingModel>(req, fac);
            return new AdminRatingCollection(coll);
        }

        private AdminUserCollection createUserCollection()
        {
            var req = new UserAdminRequestHandler();
            var fac = new UserAdminVMFactory();
            var coll = new BaseAdminModelCollection<UserModel>(req, fac);
            return new AdminUserCollection(coll);
        }

        private AdminTagCollection createTagCollection()
        {
            var req = new TagAdminRequestHandler();
            var fac = new TagAdminVMFactory();
            var coll = new BaseAdminModelCollection<TagModel>(req, fac);
            return new AdminTagCollection(coll);
        }

        private AdminSongCollection createSongCollection()
        {
            var req = new SongAdminRequestHandler();
            var fac = new SongAdminVMFactory();
            var coll = new BaseAdminModelCollection<SongModel>(req, fac);
            return new AdminSongCollection(coll);
        }

    }
}

