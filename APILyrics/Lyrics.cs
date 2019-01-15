using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APILyrics
{
    public class Lyrics
    {
        private string _webSite = "http://api.ntag.fr/lyrics/?artist={artist}&title={name}";

        /// <summary>
        /// On récupère les Lycrics via l'API "api.ntag.fr/lyrics/?artist=xxxxx&title=xxxxxx"
        /// </summary>
        /// <param name="artist">nom de l'artiste</param>
        /// <param name="song">nom de la musique</param>
        /// <returns></returns>
        public string GetLyrics(string artist, string song)
        {
            string currentAdress = _webSite.Replace("{artist}", artist.Replace(" ", "+")).Replace("{name}", song.Replace(" ", "+"));
            string lyrics = string.Empty;
            using (WebClient client = new WebClient()) // classe WebClient hérite IDisposable
                lyrics = client.DownloadString(currentAdress);
            return lyrics;
        }
    }
}
