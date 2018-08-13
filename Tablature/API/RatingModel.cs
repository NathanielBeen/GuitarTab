using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class RatingModel
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "SongId")]
        public int SongId { get; set; }

        [JsonProperty(PropertyName = "UserId")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "Rating")]
        public double Rating { get; set; }

        [JsonProperty(PropertyName = "Text")]
        public string Text { get; set; }

        public RatingModel(int id, int song_id, int user_id, double rating, string text)
        {
            Id = id;
            SongId = song_id;
            UserId = user_id;
            Rating = rating;
            Text = text;
        }

        public RatingModel() { }
    }

    public class RatingWithNameModel : RatingModel
    {
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        public RatingWithNameModel(int id, int song_id, int user_id, double rating, string text, string name)
            :base(id, song_id, user_id, rating, text)
        {
            Name = name;
        }

        public RatingWithNameModel() { }
    }

    public class MultipleRatingIdRequest
    {
        [JsonProperty(PropertyName = "rating_ids")]
        public int[] Ids { get; }

        public MultipleRatingIdRequest(List<RatingModel> rating_models)
        {
            Ids = (from rating in rating_models select rating.Id).ToArray();
        }
    }

    public class RatingFieldsRequest
    {
        [JsonProperty(PropertyName = "song_id")]
        public int SongId { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "rating")]
        public double Rating { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        public RatingFieldsRequest(RatingModel model)
        {
            SongId = model.SongId;
            UserId = model.UserId;
            Rating = model.Rating;
            Text = model.Text;
        }
    }

    public class RatingUpdateRequest : RatingFieldsRequest
    {
        [JsonProperty(PropertyName = "rating_id")]
        public int Id { get; }

        public RatingUpdateRequest(RatingModel model)
            :base(model)
        {
            Id = model.Id;
        }
    }

    public class RatingUpdater : IUpdater<RatingModel>
    {
        private bool rating_set;
        private double rating;
        private string text;

        public RatingUpdater(double? r, string t)
        {
            rating_set = (r != null);
            rating = r ?? 0;
            text = t;
        }

        public RatingModel createRequestModel(RatingModel model)
        {
            return new RatingModel(model.Id, model.SongId, model.UserId, rating, text);
        }

        public RatingModel updateModel(RatingModel model)
        {
            double new_rating = (rating_set) ? rating : model.Rating;
            string new_text = text ?? model.Text;

            return new RatingModel(model.Id, model.SongId, model.UserId, new_rating, new_text);
        }
    }
}
