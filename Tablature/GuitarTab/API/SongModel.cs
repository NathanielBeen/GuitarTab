using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public class SongModel
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Artist")]
        public string Artist { get; set; }

        [JsonProperty(PropertyName = "Album")]
        public string Album { get; set; }

        [JsonProperty(PropertyName = "AuthorName")]
        public string AuthorName { get; set; }

        [JsonProperty(PropertyName = "AuthorId")]
        public int AuthorId { get; set; }

        [JsonProperty(PropertyName = "Rating")]
        public double Rating { get; set; }

        [JsonProperty(PropertyName = "Tab")]
        public string Tab { get; set; }

        [JsonProperty(PropertyName = "Tags")]
        public string Tags { get; set; }

        [JsonProperty(PropertyName = "TagTypes")]
        public string TagTypes { get; set; }

        public SongModel(int id, string name, string artist, string album, string author_name, int author_id, double rating, string tab, string tags, string tag_types)
        {
            Id = id;
            Name = name;
            Artist = artist;
            Album = album;
            AuthorName = author_name;
            AuthorId = author_id;
            Rating = rating;
            Tab = tab;
            Tags = tags;
            TagTypes = tag_types;
        }

        public SongModel() { }

        public List<SimpleTagViewModel> CreateTagList()
        {
            var list = new List<SimpleTagViewModel>();
            string[] names = Tags.Split(',');
            string[] types = TagTypes.Split(',');

            for (int i = 0; i < Tags.Length; i++)
            {
                list.Add(new SimpleTagViewModel(names[i], types[i]));
            }

            return list;
        }
    }

    public class MultipleSongIdRequest
    {
        [JsonProperty(PropertyName = "song_ids")]
        public int[] Ids { get; }

        public MultipleSongIdRequest(List<SongModel> song_models)
        {
            Ids = (from model in song_models select model.Id).ToArray();
        }
    }

    public class SongFieldsRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "artist")]
        public string Artist { get; set; }

        [JsonProperty(PropertyName = "album")]
        public string Album { get; set; }

        [JsonProperty(PropertyName = "author")]
        public int AuthorId { get; set; }

        [JsonProperty(PropertyName = "tab")]
        public string Tab { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public string[] Tags { get; set; }

        public SongFieldsRequest(SongModel model)
        {
            Name = model.Name;
            Artist = model.Artist;
            Album = model.Album;
            AuthorId = model.AuthorId;
            Tags = model.Tags.Split(',');
            Tab = model.Tab;
        }
    }

    public class SongUpdateRequest : SongFieldsRequest
    {
        [JsonProperty(PropertyName = "song_id")]
        public int Id { get; }

        public SongUpdateRequest(SongModel model)
            :base(model)
        {
            Id = model.Id;
        }
    }

    public class MultipleSongUpdateRequest : SongFieldsRequest
    {
        [JsonProperty(PropertyName = "song_ids")]
        public int[] Ids { get; }

        public MultipleSongUpdateRequest(List<SongModel> model_ids, SongModel model)
            : base(model)
        {
            Ids = (from model_id in model_ids select model_id.Id).ToArray();
        }
    }

    public class SongSearchRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "artist")]
        public string Artist { get; set; }

        [JsonProperty(PropertyName = "album")]
        public string Album { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string AuthorName { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public double? Rating { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public string[] Tags { get; set; }

        public SongSearchRequest(string name, string artist, string album, string author_name, double? rating, string[] tags)
        {
            Name = name;
            Artist = artist;
            Album = album;
            AuthorName = author_name;
            Rating = rating;
            Tags = tags;
        }
    }

    public class SongUpdater : IUpdater<SongModel>
    {
        private string name;
        private string artist;
        private string album;
        private string tags;
        private string tab;

        public static SongUpdater createTabUpdater(string name, string artist, string album, string tags, string tab)
        {
            return new SongUpdater(name, artist, album, tags, tab);
        }

        public static SongUpdater createNoTabUpdater(string name, string artist, string album, string tags)
        {
            return new SongUpdater(name, artist, album, tags, null);
        }

        private SongUpdater(string name, string artist, string album, string tags, string tab)
        {
            this.name = name;
            this.artist = artist;
            this.album = album;
            this.tags = tags;
            this.tab = tab;
        }

        public SongModel createRequestModel(SongModel model)
        {
            return new SongModel(model.Id, name, artist, album, model.AuthorName, model.AuthorId, model.Rating, tab, tags, "");
        }

        public SongModel updateModel(SongModel model)
        {
            string new_name = name ?? model.Name;
            string new_artist = artist ?? model.Artist;
            string new_album = album ?? model.Album;
            string new_tab = tab ?? model.Tab;

            string new_tags = tags ?? model.Tags;
            string new_tag_types = createTagTypeString(model);

            return new SongModel(model.Id, new_name, new_artist, new_album, model.AuthorName, model.AuthorId,
                model.Rating, new_tab, new_tags, new_tag_types);
        }

        private string createTagTypeString(SongModel model)
        {
            if (tags == null) { return model.TagTypes; }

            int tag_count = tags.Split(',').Length;
            return new StringBuilder(2 * tag_count).Insert(0, ",0", tag_count).Remove(0, 1).ToString();
        }
    }
}
