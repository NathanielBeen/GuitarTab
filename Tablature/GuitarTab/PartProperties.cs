using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    class PartProperties : BasePropertyMenu
    {
        private Part part;

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                string error = NameError;
                setStringProperty(ref name, value, ref error);
                NameError = error;
            }
        }

        private string name_error;
        public string NameError
        {
            get { return name_error; }
            set { SetProperty(ref name_error, value); }
        }

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

        public PartProperties(PartTreeNode node, GuiCommandExecutor ex, NodeClick click)
            :base(click, ex)
        {
            part = node.getPart();

            Name = part.SongInfo.SongName;
            Artist = part.SongInfo.ArtistName;
            Album = part.SongInfo.AlbumName;
        }

        public override void resetToDefault()
        {
            Name = part.SongInfo.SongName;
            Artist = part.SongInfo.ArtistName;
            Album = part.SongInfo.AlbumName;
        }

        public override void submitChanges()
        {
            if (NameError != String.Empty || ArtistError != String.Empty || AlbumError != String.Empty) { return; }
            if (Name != part.SongInfo.AlbumName || Artist != part.SongInfo.ArtistName || Album != part.SongInfo.AlbumName)
            {
                executor.executeChangeSongInfoFromProp(getClickCopy(), Name, Artist, Album);
            }
        }
    }
}
