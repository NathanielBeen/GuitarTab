using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class Result<T>
    {
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "result")]
        public List<T> Items { get; set; }
    }

    public class Error
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "error_type")]
        public int ErrorType { get; set; }
    }

    public class MessageResult
    {
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }

        [JsonProperty(PropertyName = "result")]
        public string Message { get; set; }
    }

    public class IDMessageResult : MessageResult
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }

    public class TokenIDMessageResult : IDMessageResult
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }

    public class ModelMessageResult<T> : MessageResult
    {
        public T NewModel { get; }

        public ModelMessageResult(Error error, string message, T new_model)
        {
            Error = error;
            Message = message;
            NewModel = new_model;
        }
    }

    public class LoginResult
    {
        public UserModel Model { get; }
        public string Token { get; }
        public Error Error { get; }

        public LoginResult(UserModel model, string token, Error error)
        {
            Model = model;
            Token = token;
            Error = error;
        }
    }
}
