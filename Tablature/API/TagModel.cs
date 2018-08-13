using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class TagModel
    {
        public const string NO_TYPE = "";

        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Type")]
        public string Type { get; set; }

        public TagModel(int id, string name, string type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

        public TagModel() { }
    }

    public class MultipleTagIdRequest
    {
        [JsonProperty(PropertyName = "tag_ids")]
        public int[] Ids { get; set; }

        public MultipleTagIdRequest(List<TagModel> models)
        {
            Ids = (from model in models select model.Id).ToArray();
        }
    }

    public class TagFieldsRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        public TagFieldsRequest(TagModel model)
        {
            Name = model.Name;
            Type = model.Type;
        }
    }

    public class TagUpdateRequest : TagFieldsRequest
    {
        [JsonProperty(PropertyName = "tag_id")]
        public int Id { get; }

        public TagUpdateRequest(TagModel model)
            :base(model)
        {
            Id = model.Id;
        }
    }

    public class MultipleTagUpdateRequest : TagFieldsRequest
    {
        [JsonProperty(PropertyName = "tag_ids")]
        public int[] Ids { get; }

        public MultipleTagUpdateRequest(List<TagModel> model_ids, TagModel model)
            :base(model)
        {
            Ids = (from model_id in model_ids select model_id.Id).ToArray();
        }
    }

    public class TagUpdater : IUpdater<TagModel>
    {
        private string name;
        private string type;

        public static TagUpdater createTypeUpdater(string type)
        {
            return new TagUpdater(null, type);
        }

        public static TagUpdater createNameTypeUpdater(string name, string type)
        {
            return new TagUpdater(name, type);
        }

        private TagUpdater(string name, string type)
        {
            this.name = name;
            this.type = type;
        }

        public TagModel createRequestModel(TagModel model)
        {
            return new TagModel(model.Id, name, type);
        }

        public TagModel updateModel(TagModel model)
        {
            string new_name = name ?? model.Name;
            string new_type = type ?? model.Type;

            return new TagModel(model.Id, new_name, new_type);
        }
    }
}
