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

        public void LogOut()
        {
            CurrentUser = null;
            Token = null;
            LoggedOut?.Invoke(this, null);
        }

        public void Login(LoginResult login)
        {
            if (login.Error == null)
            {
                CurrentUser = login.Model;
                Token = login.Token;
                APIRequest.addToken(this);
                LoggedIn?.Invoke(this, null);
            }
        }

        public bool isLoggedIn()
        {
            return (CurrentUser != null);
        }

        public event EventHandler LoggedIn;
        public event EventHandler LoggedOut;
    }
}
