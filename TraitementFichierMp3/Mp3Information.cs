using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TraitementFichierMp3
{
    class Mp3Information
    {
        private TagLib.File _file;
        private bool _haveAllInformation;
        private bool _isEncoded;
        private Regex _regexFileName = new Regex(@"\d{1,2} - .[^\\|/|!|:]+\.mp3");
        private Regex _regexAlbumName = new Regex(@".[^\\|/|!|:]+ - .[^\\|/|!|:]+");

        public Mp3Information(string pathFile)
        {
            _haveAllInformation = true;
            _isEncoded = false;
            _file = TagLib.File.Create(pathFile);
        }

        /// <summary>
        /// On récupère les informations de la musique
        /// </summary>
        /// <param name="informationName">track - title - album - artist - annee - genre - debit</param>
        /// <returns>Retourne la valeur souhaité en string</returns>
        public string GetInformation(string informationName)
        {
            switch (informationName)
            {
                case "track":
                    if (_file.Tag.Track == 0)
                        _haveAllInformation = false;
                    return _file.Tag.Track.ToString("00");
                case "title":
                    if (_file.Tag.Title == string.Empty)
                        _haveAllInformation = false;
                    return _file.Tag.Title;
                case "album": return _file.Tag.Album;
                case "artist": return String.Join("/", _file.Tag.Performers);
                case "annee": return _file.Tag.Year.ToString();
                case "genre": return _file.Tag.FirstGenre;
                case "debit": return _file.Tag.BeatsPerMinute.ToString();
                default: return string.Empty;
            }
        }

        /// <summary>
        /// Utiliser la classe xxx pour récupérer les paroles
        /// Utilise le site 
        /// http://api.ntag.fr/lyrics/?artist=nom+de+lartiste&title=titre+de+la+chanson
        /// http://api.ntag.fr/lyrics/?artist=avenged+sevenfold&title=nightmare
        /// </summary>
        /// <param name="lyrics">Les paroles de la chanson</param>
        public void SetLyric(string lyrics)
        {
            if (lyrics == string.Empty)
                Logger.Instance.WriteLineAndPushError(string.Format("Impossible de trouver les paroles pour la chanson {0} - {1} - {2}", GetInformation("track"), GetInformation("artist"), GetInformation("title")));
            _file.Tag.Lyrics = lyrics;
            _file.Save();
        }

        /// <summary>
        /// En attente
        /// </summary>
        /// <param name="picture"></param>
        private void SetPicture(TagLib.IPicture[] picture)
        {
            _file.Tag.Pictures = picture;
            _file.Save();
        }

        /// <summary>
        /// On traite le nom du fichier de musique en rapport avec le format donné
        /// </summary>
        /// <param name="format">le format saisie dans app.config "formatFile". Exemple : "{track} - {title}"</param>
        /// <param name="informations">La liste des éléments se trouvant dans le formatFile. Exemple : pos1 - "{track}" : post2 - "{title}"</param>
        /// <returns>Retourne le nouveau nom du fichier de musique</returns>
        private string FormatNameFile(string format, List<string> informations)
        {
            string finalName = format;
            foreach (var item in informations)
                finalName = finalName.Replace( item, GetInformation(item.Replace("{","").Replace("}","")) );
            return finalName;
        }

        /// <summary>
        /// On renomme le fichier avec son nom final
        /// </summary>
        /// <param name="formatFinal">le format saisie dans app.config "formatFile". Exemple : "{track} - {title}"</param>
        /// <param name="informationsReplaceFormatFinal">La liste des éléments se trouvant dans le formatFile. Exemple : pos1 - "{track}" : post2 - "{title}"</param>
        /// <param name="directoryFileFullName">Le directory complet avec le chemin du fichier</param>
        /// <param name="directoryName">Le directory où se trouve le fichier</param>
        public void RenameFile(string formatFinal, List<string> informationsReplaceFormatFinal, string directoryFileFullName, string directoryName)
        {
            string newFileName = FormatNameFile(formatFinal, informationsReplaceFormatFinal);

            if (!_haveAllInformation)
            {
                if(!StringFileToMetaData(_file.Name))
                    Logger.Instance.WriteLineAndPushError(string.Format("Erreur, le fichier '{0}' ne possède pas toutes les informations", _file.Name));
                return;
            }

            System.IO.File.Move(
                directoryFileFullName,
                string.Format(@"{0}\{1}", directoryName, ReplaceCaracteresIncorrect(newFileName))
            );
        }

        public void ForceSetMetaDataWithNameFile()
        {
            StringFileToMetaData(_file.Name);
        }

        private bool StringFileToMetaData(string fileName)
        {
            string[] nomFichier = fileName.Split('\\');
            string file = nomFichier.LastOrDefault();
            string album = nomFichier[nomFichier.Count() - 2];
            if (_regexFileName.IsMatch(file)) // Si le fichier est de type 00 - xxxxxxx.mp3 alors ok, renvoi true
            {
                // Pour le titre
                // On split le file avec le number et le titre
                string numeroFile = file.Split('-')[0];
                string titleFile = file.Replace(string.Format("{0}- ", numeroFile), "").Replace(".mp3", "");    // GROS PORC !

                _file.Tag.Title = titleFile;
                _file.Tag.Track = uint.Parse(numeroFile);

                // Pour l'album
                if (_regexAlbumName.IsMatch(album))
                {
                    string artiste = album.Split('-')[0];
                    string nomAlbum = album.Replace(string.Format("{0}- ", artiste), "").Split('(')[0];

                    _file.Tag.Album = nomAlbum;
                    _file.Tag.Performers = new string[] { artiste };

                    try
                    {
                        string annee = album.Split('(')[1].Split(')')[0];
                        _file.Tag.Year = uint.Parse(annee);
                    }
                    catch { } // Pas d'année
                }

                _file.Save();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remplace tout les caractères incorrectes pour enregistrer le fichier sur l'ordinateur
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ReplaceCaracteresIncorrect(string value)
        {
            return value.Replace("/", " ")
                .Replace("\\", " ")
                .Replace("|", " ")
                .Replace(">", " ")
                .Replace("<", " ")
                .Replace(":", " ")
                .Replace("*", " ")
                .Replace("?", " ")
                .Replace("\"", " ")
                ;
        }

        /// <summary>
        /// Converti le fichier courant vers l'extention MP3
        /// </summary>
        /// <param name="extention"></param>
        /// <param name="debitEncodage"></param>
        public void ConvertToMP3(string extention, int debitEncodage)
        {
            APIAudioConverter.IAudioConverter iAC = null;
            
            switch (extention)
            {
                case ".mp3": return; // Sert à rien de convertir un MP3 en MP3 !!! :o
                case ".wma": iAC = new APIAudioConverter.WmaAudioConverter();
                    break;
                case ".wav": iAC = new APIAudioConverter.WavAudioConverter();
                    break;
                case ".flac": iAC = new APIAudioConverter.FlacAudioConverter();
                    iAC.ConvertToMp3ffmpeg(_file.Name.Replace(extention, ""), debitEncodage);
                    return;
                case ".m4a": iAC = new APIAudioConverter.MediaFoundationAudioConverter();
                    break;
                default:
                    Logger.Instance.WriteLineAndPushError(string.Format("Format de conversion '{3}' non disponible {0} - {1} - {2}", GetInformation("track"), GetInformation("artist"), GetInformation("title"), extention));
                    return;
            }
            try
            {
                byte[] byteArray = byteArray = iAC.ConvertToMp3(_file.Name, debitEncodage);
                string pathNewFile = string.Format("{0}{1}", _file.Name.Replace(extention, ""), ".mp3");
                System.IO.FileStream fileStream = new System.IO.FileStream(pathNewFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                fileStream.Write(byteArray, 0, byteArray.Length);
                fileStream.Close();
                CopyCurrentTagToAnotherFile(pathNewFile);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLineAndPushError(string.Format("Erreur de conversion pour le fichier {0} - {1} - {2} \n\n {3}", GetInformation("track"), GetInformation("artist"), GetInformation("title"), ex.Message));
            }
        }

        private void CopyCurrentTagToAnotherFile(string TargetFileTag)
        {
            TagLib.File targetFile = TagLib.File.Create(TargetFileTag);

            // Moche mais pas de IEnumerable sur le Tag :(
            targetFile.Tag.Album = _file.Tag.Album;
            targetFile.Tag.AlbumArtists = _file.Tag.AlbumArtists;
            targetFile.Tag.AlbumArtistsSort = _file.Tag.AlbumArtistsSort;
            targetFile.Tag.AlbumSort = _file.Tag.AlbumSort;
            targetFile.Tag.AmazonId = _file.Tag.AmazonId;
            targetFile.Tag.BeatsPerMinute = _file.Tag.BeatsPerMinute;
            targetFile.Tag.Composers = _file.Tag.Composers;
            targetFile.Tag.ComposersSort = _file.Tag.ComposersSort;
            targetFile.Tag.Conductor = _file.Tag.Conductor;
            targetFile.Tag.Disc = _file.Tag.Disc;
            targetFile.Tag.DiscCount = _file.Tag.DiscCount;
            targetFile.Tag.Genres = _file.Tag.Genres;
            targetFile.Tag.Grouping = _file.Tag.Grouping;
            targetFile.Tag.Lyrics = _file.Tag.Lyrics;
            targetFile.Tag.MusicBrainzArtistId = _file.Tag.MusicBrainzArtistId;
            targetFile.Tag.MusicBrainzDiscId = _file.Tag.MusicBrainzDiscId;
            targetFile.Tag.MusicBrainzReleaseArtistId = _file.Tag.MusicBrainzReleaseArtistId;
            targetFile.Tag.MusicBrainzReleaseCountry = _file.Tag.MusicBrainzReleaseCountry;
            targetFile.Tag.MusicBrainzReleaseId = _file.Tag.MusicBrainzReleaseId;
            targetFile.Tag.MusicBrainzReleaseStatus = _file.Tag.MusicBrainzReleaseStatus;
            targetFile.Tag.MusicBrainzReleaseType = _file.Tag.MusicBrainzReleaseType;
            targetFile.Tag.MusicBrainzTrackId = _file.Tag.MusicBrainzTrackId;
            targetFile.Tag.MusicIpId = _file.Tag.MusicIpId;
            targetFile.Tag.Performers = _file.Tag.Performers;
            targetFile.Tag.PerformersSort = _file.Tag.PerformersSort;
            targetFile.Tag.Pictures = _file.Tag.Pictures;
            targetFile.Tag.Title = _file.Tag.Title;
            targetFile.Tag.TitleSort = _file.Tag.TitleSort;
            targetFile.Tag.Track = _file.Tag.Track;
            targetFile.Tag.TrackCount = _file.Tag.TrackCount;
            targetFile.Tag.Year = _file.Tag.Year;

            targetFile.Save();
            _isEncoded = true;
        }

        // Supprime le fichier courant du pc
        public void DeleteFile()
        {
            if (!_isEncoded)
                return;

            string path = _file.Name;
            _file = null;
            System.IO.File.Delete(path);
        }
    }
}
