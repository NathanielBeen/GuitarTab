using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public static class Utility
    {
        public static LoginResult attemptLogin(string username, string password)
        {
            var req = new UserLoginRequest(username, password);
            TokenIDMessageResult res = APIRequest.login(req).GetAwaiter().GetResult();
            return new LoginResult(res.Id, res.Token, res.Error);
        }

        public static LoginResult attemptSignUp(string username, string password)
        {
            var req = new UserFieldsRequest(new UserModel(0, username, password, 0));
            IDMessageResult res = APIRequest.createUser(req).GetAwaiter().GetResult();

            if (res.Error != null) { return new LoginResult(0, "", res.Error); }
            return attemptLogin(username, password);
        }

        public static Result<SongModel> attemptSearch(SearchTerms terms)
        {
            if (terms == null) { return new Result<SongModel>(); }
            if (terms.Name == null && terms.Artist == null && terms.Album == null && terms.Author == null && terms.Rating == 0 && terms.Tags.Length == 0)
            {
                return APIRequest.getAllSongs().GetAwaiter().GetResult();
            }

            SongSearchRequest search = new SongSearchRequest(terms.Name, terms.Artist, terms.Album, terms.Author, terms.Rating, terms.Tags); 
            return APIRequest.searchSongs(search).GetAwaiter().GetResult();
        }

        public static TokenIDMessageResult attemptChangePassword(string name, string password, string new_password)
        {
            var req = new ChangePasswordRequest(name, password, new_password);
            return APIRequest.changePassword(req).GetAwaiter().GetResult();
        }

        public static MessageResult attemptRemoveAccount(string name, string password)
        {
            var req = new UserUpdateRequest(new UserModel(0, name, password, 0));
            return APIRequest.removeOwnAccount(req).GetAwaiter().GetResult();
        }
    }
}
