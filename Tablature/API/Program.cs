using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    class Program
    {
        static void Main(string[] args)
        {
            APIRequest.configureClient();
            //var request = new SongRequestModel("new_name", "new_artist", "new_album", 0, "", new string[0]);
            //var result = APIRequest.addSong(request).GetAwaiter().GetResult();
            //Console.Write(result);
        }
    }
}
