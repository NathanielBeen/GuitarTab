using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public abstract class BaseAdminViewModelFactory<T>
    {
        private CollectionEventDelegate<T> begin_edit_delegate;
        private CollectionEventDelegate<T> cancel_edit_delegate;
        private CollectionEventDelegate<T> delete_delegate;
        private CollectionUpdateEventDelegate<T> update_delegate;

        public BaseAdminViewModelFactory() { }

        public void setEventHandlers(CollectionEventDelegate<T> begin, CollectionEventDelegate<T> cancel, 
            CollectionEventDelegate<T> delete, CollectionUpdateEventDelegate<T> update)
        {
            begin_edit_delegate = begin;
            cancel_edit_delegate = cancel;
            delete_delegate = delete;
            update_delegate = update;
        }

        protected HighlightView<T> addHighlightLayer(IViewModel<T> model) { return new HighlightView<T>(model); }

        protected ExpandView<T> addExpandLayer(IViewModel<T> model) { return new ExpandView<T>(model); }

        protected DeleteView<T> addDeleteLayer(IViewModel<T> model)
        {
            var delete = new DeleteView<T>(model);
            delete.Deleted += ((s, m) => delete_delegate?.Invoke(m));
            return delete;
        }

        protected EditView<T> addEditLayer(IViewModel<T> model)
        {
            var edit = new EditView<T>(model);
            edit.Edited += ((s, m) => begin_edit_delegate?.Invoke(m));
            return edit;
        }

        protected void handleEditEvents(IEditModel<T> model)
        {
            model.CancelEdit += ((s, m) => cancel_edit_delegate?.Invoke(m));
            model.ConfirmEdit += ((s, a) => update_delegate?.Invoke(a.Model, a.Updater));
        }

        public abstract IViewModel<T> createStandardVM(T model);
        public abstract IViewModel<T> createEditVM(T model);
    }

    public class TagAdminVMFactory : BaseAdminViewModelFactory<TagModel>
    {
        public TagAdminVMFactory() { }

        public override IViewModel<TagModel> createStandardVM(TagModel model)
        {
            var core = new TagViewModel(model);
            var edit = addEditLayer(core);
            var delete = addDeleteLayer(edit);
            return addHighlightLayer(delete);
        }

        public override IViewModel<TagModel> createEditVM(TagModel model)
        {
            var edit = new EditTagViewModel(model);
            handleEditEvents(edit);
            return addDeleteLayer(edit);
        }
    }

    public class SongAdminVMFactory : BaseAdminViewModelFactory<SongModel>
    {
        public SongAdminVMFactory() { }
        public override IViewModel<SongModel> createStandardVM(SongModel model)
        {
            var core = new SongViewModel(model);
            var expand = addExpandLayer(core);
            var edit = addEditLayer(expand);
            var delete = addDeleteLayer(edit);
            return addHighlightLayer(delete);
        }

        public override IViewModel<SongModel> createEditVM(SongModel model)
        {
            var core = new EditSongViewModel(model);
            handleEditEvents(core);
            return addDeleteLayer(core);
        }
    }

    public class UserAdminVMFactory : BaseAdminViewModelFactory<UserModel>
    {
        public UserAdminVMFactory() { }

        public override IViewModel<UserModel> createStandardVM(UserModel model)
        {
            var core = new UserViewModel(model);
            var edit = addEditLayer(core);
            return addDeleteLayer(edit);
        }

        public override IViewModel<UserModel> createEditVM(UserModel model)
        {
            var core = new EditUserViewModel(model);
            handleEditEvents(core);
            return addDeleteLayer(core);
        }
    }

    public class RatingAdminVMFactory : BaseAdminViewModelFactory<RatingModel>
    {
        public RatingAdminVMFactory() { }

        public override IViewModel<RatingModel> createStandardVM(RatingModel model)
        {
            var core = new RatingViewModel(model);
            var expand = addExpandLayer(core);
            var edit = addEditLayer(expand);
            var delete = addDeleteLayer(edit);
            return addHighlightLayer(delete);
        }

        public override IViewModel<RatingModel> createEditVM(RatingModel model)
        {
            var core = new EditRatingViewModel(model);
            handleEditEvents(core);
            return addDeleteLayer(core);
        }
    }
}
