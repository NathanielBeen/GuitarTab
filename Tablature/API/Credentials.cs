using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class Credentials
    {
        public string Token { get; set; }
        public UserModel CurrentUser { get; set; }
        //maybe like an expiration time or something

        public Credentials(UserModel current_user, string token)
        {
            Token = token;
            CurrentUser = current_user;
        }
    }
}
