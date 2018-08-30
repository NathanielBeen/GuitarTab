using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    class AdminSongCollection : BaseInputViewModel
    {
        public BaseAdminModelCollection<SongModel> Collection { get; }

        public NotificationField<List<SimpleTagViewModel>> Tags { get; private set; }
        public StringInputField Artist { get; private set; }
        public StringInputField Album { get; private set; }
        public StringInputField AddTag { get; private set; }

        public ICommand ClearCommand { get; private set; }
        public ICommand ConfirmCommand { get; private set; }
        public ICommand AddTagCommand { get; private set; }
        public ICommand RemoveTagCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public AdminSongCollection(BaseAdminModelCollection<SongModel> collection)
        {
            Collection = collection;
            initFields();
            initCommands();
        }

        private void initFields()
        {
            Tags = new NotificationField<List<SimpleTagViewModel>>();
            Artist = new StringInputField("Artist", 1, 64);
            Album = new StringInputField("Album", 1, 64);
            AddTag = new StringInputField("AddTag", 1, 64);
        }

        private void initCommands()
        {
            ClearCommand = new RelayCommand(clear);
            ConfirmCommand = new RelayCommand(handleUpdate);
            AddTagCommand = new RelayCommand(handleAddTag);
            RemoveTagCommand = new RelayCommand(handleRemoveTag);
            DeleteCommand = new RelayCommand(handleDelete);
        }

        private void handleDelete()
        {
            Collection.handleMultipleDeleteRequest();
        }

        private void handleUpdate()
        {
            if (hasErrors()) { return; }
            IUpdater<SongModel> updater = createUpdater();
            Collection.handleMultipleUpdateRequest(updater);
            clear();
        }

        private void clear()
        {
            Artist.clearField();
            Album.clearField();
            AddTag.clearField();
            Tags.Value = new List<SimpleTagViewModel>();
        }

        private void handleAddTag()
        {
            if (AddTag.hasErrors()) { return; }
            foreach (var tag in Tags.Value)
            {
                if (tag.Name.Equals(AddTag)) { return; }
            }

            Tags.Value.Add(new SimpleTagViewModel(AddTag.Value, TagModel.NO_TYPE));
            onPropertyChanged(nameof(Tags));
        }

        private void handleRemoveTag()
        {
            SimpleTagViewModel model = Tags.Value.Where(t => t.Selected.Value).FirstOrDefault();
            if (model == null) { return; }

            Tags.Value.Remove(model);
            onPropertyChanged(nameof(Tags));
        }

        private bool hasErrors()
        {
            return (Artist.hasErrors() || Album.hasErrors());
        }

        private IUpdater<SongModel> createUpdater()
        {
            return SongUpdater.createNoTabUpdater(null, Artist.Value, Album.Value, createTagString());
        }

        private string createTagString()
        {
            var tag = new StringBuilder();
            foreach (SimpleTagViewModel model in Tags.Value) { tag.Append(model.Name); }
            return tag.ToString();
        }
    }
}
