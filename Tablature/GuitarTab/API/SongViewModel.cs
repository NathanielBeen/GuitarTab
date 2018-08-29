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

        private string artist;
        public string Artist
        {
            get { return artist; }
            set
            {
                string error = ArtistError;
                setStringProperty(ref artist, value, ref error);
                ArtistError = error;
            }
        }

        private string artist_error;
        public string ArtistError
        {
            get { return artist_error; }
            set { SetProperty(ref artist_error, value); }
        }

        private string album;
        public string Album
        {
            get { return album; }
            set
            {
                string error = AlbumError;
                setStringProperty(ref album, value, ref error);
                AlbumError = error;
            }
        }

        private string album_error;
        public string AlbumError
        {
            get { return album_error; }
            set { SetProperty(ref album_error, value); }
        }

        private string add_tag;
        public string AddTag
        {
            get { return add_tag; }
            set
            {
                string error = AddTagError;
                setStringProperty(ref add_tag, value, ref error);
                AddTagError = error;
            }
        }

        private string add_tag_error;
        public string AddTagError
        {
            get { return add_tag_error; }
            set { SetProperty(ref add_tag_error, value); }
        }

        private List<SimpleTagViewModel> tags;
        public List<SimpleTagViewModel> Tags
        {
            get { return tags; }
            set { SetProperty(ref tags, value); }
        }

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

            Artist = model.Artist;
            ArtistError = "";
            Album = model.Album;
            AlbumError = "";
            Tags = Base.CreateTagList();

            initCommands();
        }

        public void initCommands()
        {
            CancelCommand = new RelayCommand(handleCancel);
            ResetCommand = new RelayCommand(handleReset);
            ConfirmCommand = new RelayCommand(handleConfirm);
            AddTagCommand = new RelayCommand(handleTagAdded);
            RemoveTagCommand = new RelayCommand(handleTagRemoved);
        }

        public void handleReset()
        {
            Artist = Base.Artist;
            ArtistError = "";
            Album = Base.Album;
            AlbumError = "";
            AddTag = "";
            AddTagError = "";
            Tags = Base.CreateTagList();
        }

        public void handleCancel() { CancelEdit?.Invoke(this, Base); }

        public void handleConfirm()
        {
            if (ArtistError != String.Empty || AlbumError != String.Empty) { return; }

            string new_tags = String.Join(",", (from tag in Tags select tag.Name));
            //string new_types = String.Join(",", (from tag in Tags select tag.Type));

            var updater = SongUpdater.createNoTabUpdater(null, artist, album, new_tags);
            var args = new UpdateEventArgs<SongModel>(Base, updater);
            ConfirmEdit?.Invoke(this, args);
        }

        public void handleTagAdded()
        {
            if (AddTagError != String.Empty) { return; }
            foreach (var tag in Tags)
            {
                if (tag.Name.Equals(AddTag)) { return; }
            }

            Tags.Add(new SimpleTagViewModel(AddTag, TagModel.NO_TYPE));
            onPropertyChanged(nameof(Tags));
        }

        public void handleTagRemoved()
        {
            SimpleTagViewModel model = Tags.Where(t => t.Selected).FirstOrDefault();
            if (model == null) { return; }

            Tags.Remove(model);
            onPropertyChanged(nameof(Tags));
        }
    }
}
