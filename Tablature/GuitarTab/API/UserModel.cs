using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public class UserModel
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        //never used by JSON
        public string Password { get; set; }

        [JsonProperty(PropertyName = "Type")]
        public int Type { get; set; }

        public UserModel(int id, string name, string password, int type)
        {
            Id = id;
            Name = name;
            Password = password;
            Type = type;
        }

        public UserModel() { }
    }

    public class UserLoginRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; }

        public UserLoginRequest(UserModel model)
        {
            Name = model.Name;
            Password = model.Password;
        }

        public UserLoginRequest(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }

    public class UserFieldsRequest : UserLoginRequest
    {
        [JsonProperty(PropertyName = "type")]
        public int Type { get; }

        public UserFieldsRequest(UserModel model)
            :base(model)
        {
            Type = model.Type;
        }
    }

    public class UserUpdateRequest : UserFieldsRequest
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        public UserUpdateRequest(UserModel model)
            :base(model)
        {
            Id = model.Id;
        }
    }

    public class ChangePasswordRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "new_password")]
        public string NewPassword { get; set; }

        public ChangePasswordRequest(string name, string password, string new_password)
        {
            Name = name;
            Password = password;
            NewPassword = new_password;
        }
    }

    public class UserUpdater: IUpdater<UserModel>
    {
        private bool type_set;
        private int type;

        public UserUpdater(int? t)
        {
            type_set = (t != null);
            type = t ?? 0;
        }

        public UserModel createRequestModel(UserModel model)
        {
            return new UserModel(model.Id, model.Name, model.Password, type);
        }

        public UserModel updateModel(UserModel model)
        {
            int new_type = (type_set) ? type : model.Type;
            return new UserModel(model.Id, model.Name, model.Password, new_type);
        }
    }
}
