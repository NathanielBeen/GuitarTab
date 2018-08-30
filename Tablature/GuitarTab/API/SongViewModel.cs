using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class SongViewModel : IViewModel<SongModel>
    {
        public SongModel Base { get; }
        public VMType ViewType { get { return VMType.BASE; } }

        public int Id
        {
            get { return Base.Id; }
        }

        public string Name
        {
            get { return Base.Name; }
        }

        public string Artist
        {
            get { return Base.Artist; }
        }

        public string Album
        {
            get { return Base.Album; }
        }

        public string Author
        {
            get { return Base.AuthorName; }
        }
        
        public double Rating
        {
            get { return Base.Rating; }
        }

        public List<SimpleTagViewModel> Tags { get; }

        public SongViewModel(SongModel model)
        {
            Base = model;
            Tags = model.CreateTagList();
        }
    }

    public class EditSongViewModel : BaseInputViewModel, IEditModel<SongModel>
    {
        public SongModel Base { get; }
        public VMType ViewType { get { return VMType.BASE_EDIT; } }

        public int Id { get { return Base.Id; } }

        public string Name { get { return Base.Name; } }

        public string Author { get { return Base.AuthorName; } }

        public double Rating { get { return Base.Rating; } }

        public StringInputField Artist { get; private set; }
        public StringInputField Album { get; private set; }
        public StringInputField AddTag { get; private set; }
        public NotificationField<List<SimpleTagViewModel>> Tags { get; private set; }

        public ICommand CancelCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ConfirmCommand { get; set; }
        public ICommand AddTagCommand { get; set; }
        public ICommand RemoveTagCommand { get; set; }

        public event EventHandler<SongModel> CancelEdit;
        public event EventHandler<UpdateEventArgs<SongModel>> ConfirmEdit;

        public EditSongViewModel(SongModel model)
        {
            Base = model;
            initFields(model);
            initCommands();
        }

        private void initFields(SongModel model)
        {
            Artist = new StringInputField("Artist", 0, 32);
            Album = new StringInputField("Album", 0, 32);
            AddTag = new StringInputField("Added Tag Name", 0, 32);
            Tags = new NotificationField<List<SimpleTagViewModel>>();

            Artist.Value = model.Artist;
            Album.Value = model.Album;
            Tags.Value = Base.CreateTagList();
        }

        private void initCommands()
        {
            CancelCommand = new RelayCommand(handleCancel);
            ResetCommand = new RelayCommand(handleReset);
            ConfirmCommand = new RelayCommand(handleConfirm);
            AddTagCommand = new RelayCommand(handleTagAdded);
            RemoveTagCommand = new RelayCommand(handleTagRemoved);
        }

        public void handleReset()
        {
            Artist.Value = Base.Artist;
            Album.Value = Base.Album;
            AddTag.clearField();
            Tags.Value = Base.CreateTagList();
        }

        public void handleCancel() { CancelEdit?.Invoke(this, Base); }

        public void handleConfirm()
        {
            if (Artist.hasErrors() || Album.hasErrors()) { return; }

            string new_tags = String.Join(",", (from tag in Tags.Value select tag.Name));
            //string new_types = String.Join(",", (from tag in Tags select tag.Type));

            var updater = SongUpdater.createNoTabUpdater(null, Artist.Value, Album.Value, new_tags);
            var args = new UpdateEventArgs<SongModel>(Base, updater);
            ConfirmEdit?.Invoke(this, args);
        }

        public void handleTagAdded()
        {
            if (AddTag.hasErrors()) { return; }
            foreach (var tag in Tags.Value)
            {
                if (tag.Name.Equals(AddTag)) { return; }
            }

            Tags.Value.Add(new SimpleTagViewModel(AddTag.Value, TagModel.NO_TYPE));
            onPropertyChanged(nameof(Tags));
        }

        public void handleTagRemoved()
        {
            SimpleTagViewModel model = Tags.Value.Where(t => t.Selected.Value).FirstOrDefault();
            if (model == null) { return; }

            Tags.Value.Remove(model);
            onPropertyChanged(nameof(Tags));
        }
    }
}
