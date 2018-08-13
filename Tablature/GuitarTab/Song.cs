using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class SongInfo
    {
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }

        public SongInfo(string song_name, string artist_name, string album_name)
        {
            SongName = song_name;
            ArtistName = artist_name;
            AlbumName = album_name;
        }

        public static SongInfo createDefault() { return new SongInfo("Name", "Artist", "Album"); }
    }

    public class InstrumentInfo
    {
        public InstrumentType Type { get; set; }
        public int Strings { get; set; }

        public InstrumentInfo(InstrumentType type, int strings)
        {
            Type = type;
            Strings = strings;
        }

        public static InstrumentInfo createDefault() { return new InstrumentInfo(InstrumentType.ElecGuitar, 6); }
    }

    public enum InstrumentType
    {
        None = 0,
        AcousGuitar = 1,
        ElecGuitar = 2,
        Bass = 3
    }

    public static class InstrumentTypeExtensions
    {
        public static string getStringFromInstrumentType(this InstrumentType type)
        {
            switch (type)
            {
                case InstrumentType.AcousGuitar:
                    return "Acoustic Guitar";
                case InstrumentType.ElecGuitar:
                    return "Electric Guitar";
                case InstrumentType.Bass:
                    return "Bass";
                default:
                    return String.Empty;
            }
        }

        public static InstrumentType getInstrumentTypeFromString(string type)
        {
            switch (type)
            {
                case "Acoustic Guitar":
                    return InstrumentType.AcousGuitar;
                case "Electric Guitar":
                    return InstrumentType.ElecGuitar;
                case "Base":
                    return InstrumentType.Bass;
                default:
                    return InstrumentType.None;
            }
        }

        public static List<string> getAllInstrumentTypeStrings()
        {
            return (from type in Enum.GetValues(typeof(InstrumentType)).Cast<InstrumentType>()
                    where type != InstrumentType.None
                    select type.getStringFromInstrumentType()).ToList();
        }
    }
}
