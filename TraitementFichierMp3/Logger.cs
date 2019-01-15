using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraitementFichierMp3
{
    public sealed class Logger
    {
        private static Logger _instance = null;
        private static readonly object _padlock = new object();

        private StringBuilder _strBuilder;
        private List<string> _listError;

        private Logger() 
        {
            _strBuilder = new StringBuilder();
            _listError = new List<string>();
        }

        public static Logger Instance
        {
            get 
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new Logger();
                }
                return _instance; 
            }
        }

        /// <summary>
        /// On écrit sur la console et on push dans une list de string
        /// </summary>
        /// <param name="error">Contenu du message d'erreur</param>
        public void WriteLineAndPushError(string error)
        {
            lock (_padlock)
            {
                Console.WriteLine(error);
                // AddError(error);
                _listError.Add(error);
            }
        }

        public void AddError(string error)
        {
            _strBuilder.AppendLine(error);
        }

        /// <summary>
        /// On save l'ensemble des erreurs dans un fichier de log sur le path
        /// </summary>
        /// <param name="savePath">Path avec le nom du fichier où il doit être enregistré</param>
        public void SaveLogger(string savePath)
        {
            using (StreamWriter stw = new StreamWriter(savePath))
                _listError.OrderBy(o => o).ToList().ForEach(item => stw.WriteLine(item));
        }
    }
}
