using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class UpdateEventArgs<T>
    {
        public T Model { get; }
        public IUpdater<T> Updater { get; }

        public UpdateEventArgs(T model, IUpdater<T> updater)
        {
            Model = model;
            Updater = updater;
        }
    }

    public class LoginEventArgs
    {
        public int Id { get; }
        public string Token { get; }

        public LoginEventArgs(int id, string token)
        {
            Id = id;
            Token = token;
        }
    }
}
