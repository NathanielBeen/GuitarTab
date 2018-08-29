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

        public StringInputField Artist { get; }
        public StringInputField Album { get; }
        public StringInputField AddTag { get; }

        private List<SimpleTagViewModel> tags;
        public List<SimpleTagViewModel> Tags
        {
            get { return tags; }
            set { SetProperty(ref tags, value); }
        }

        public ICommand ClearCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand AddTagCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public AdminSongCollection(BaseAdminModelCollection<SongModel> collection)
        {
            Collection = collection;
            Artist = new StringInputField("Artist", 1, 64);
            Album = new StringInputField("Album", 1, 64);
            AddTag = new StringInputField("AddTag", 1, 64);
            initCommands();
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
            Tags = new List<SimpleTagViewModel>();
        }

        private void handleAddTag()
        {
            if (AddTag.hasErrors()) { return; }
            foreach (var tag in Tags)
            {
                if (tag.Name.Equals(AddTag)) { return; }
            }

            Tags.Add(new SimpleTagViewModel(AddTag.Value, TagModel.NO_TYPE));
            onPropertyChanged(nameof(Tags));
        }

        private void handleRemoveTag()
        {
            SimpleTagViewModel model = Tags.Where(t => t.Selected).FirstOrDefault();
            if (model == null) { return; }

            Tags.Remove(model);
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
            foreach (SimpleTagViewModel model in Tags) { tag.Append(model.Name); }
            return tag.ToString();
        }
    }
}
