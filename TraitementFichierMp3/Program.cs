using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TraitementFichierMp3
{
    class Program
    {
        private static Regex _regex = new Regex(@"\{(.*?)\}");
        
        /*
                track
                title
                album
                artist
                annee
                genre
                debit
         */

        static void Main(string[] args)
        {
            string formatFichier = ConfigurationManager.AppSettings["formatFile"];
            List<string> FileInformation = MatchFormatRegex(formatFichier);

            // string directoryName = ConfigurationManager.AppSettings["musiqueDirectory"];
            int debitEncodage = int.Parse(ConfigurationManager.AppSettings["DebitEncodage"]);
            string searchFiles = ConfigurationManager.AppSettings["searchFiles"];

            DirectoryInfo musiqueDirectory = new DirectoryInfo(ConfigurationManager.AppSettings["musiqueDirectory"]);
            // Console.WriteLine(musiqueDirectory.GetDirectories().FirstOrDefault());
            // Parallel.ForEach(musiqueDirectory.GetDirectories(), alb => 
            // foreach (var alb in musiqueDirectory.GetDirectories())
            {
                foreach (var mus in musiqueDirectory.GetFiles("*.*", SearchOption.AllDirectories))
                //Parallel.ForEach(musiqueDirectory.GetFiles("*.*", SearchOption.AllDirectories), mus =>
                {
                    if (searchFiles.Contains(mus.Extension))    // On ne se fait pas chier =D
                    {
                        Console.WriteLine(string.Format("Traitement fichier : {0}", mus.Name));
                        Mp3Information mp3 = new Mp3Information(mus.FullName);

                        if (ConfigurationManager.AppSettings["Lyrics"] == "1")
                            mp3.SetLyric(new APILyrics.Lyrics().GetLyrics(mp3.GetInformation("artist"), mp3.GetInformation("title")));

                        if (ConfigurationManager.AppSettings["ConvertMP3"] == "1")
                            mp3.ConvertToMP3(mus.Extension, debitEncodage);

                        if (ConfigurationManager.AppSettings["ForceFileNameToMetaData"] == "1")
                            mp3.ForceSetMetaDataWithNameFile();

                        if (ConfigurationManager.AppSettings["Rename"] == "1")
                            mp3.RenameFile(string.Format("{0}{1}", formatFichier, mus.Extension), FileInformation, mus.FullName, mus.DirectoryName);

                        if (ConfigurationManager.AppSettings["DeleteOldFile"] == "1")
                            mp3.DeleteFile();
                    }
                }
                // );
            }
            // );

            Logger.Instance.SaveLogger(ConfigurationManager.AppSettings["savePath"]);
            Console.WriteLine("End");
            Console.ReadKey();
        }

        static public List<string> MatchFormatRegex(string regex)
        {
            List<string> list = new List<string>();
            foreach (var item in _regex.Matches(regex))
                list.Add(item.ToString());
            return list;
        }
    }
}
