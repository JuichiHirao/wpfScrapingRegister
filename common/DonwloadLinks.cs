using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using wpfScrapingRegister.collection;

namespace wpfScrapingRegister.common
{
    class DonwloadLinks
    {
        public static string GetFilesText(string myListLinks)
        {
            string[] links = myListLinks.Split(' ');

            Regex regex = new Regex(FileCollection.REGEX_TARGET_MOVIE_EXTENTION, RegexOptions.IgnoreCase);
            List<string> listFiles = new List<string>();
            foreach (string link in links)
            {
                string editLink = link.Replace(".html", "");
                if (regex.IsMatch(editLink))
                    listFiles.Add(Path.GetFileName(editLink));
            }
            Regex rarRegex = new Regex(FileCollection.REGEX_TARGET_ARCHIVE_EXTENTION, RegexOptions.IgnoreCase);
            foreach (string link in links)
            {
                string editLink = link.Replace(".html", "");
                if (rarRegex.IsMatch(editLink))
                    listFiles.Add(Path.GetFileName(editLink));
            }

            return string.Join(" ", listFiles);
        }

        public static string GetTargetOnly(string myDownloadLinks)
        {
            string[] links = myDownloadLinks.Split(' ');

            Regex regex = new Regex(FileCollection.REGEX_TARGET_MOVIE_EXTENTION, RegexOptions.IgnoreCase);
            int downloadKind = 0;
            List<string> movieLinks = new List<string>();
            foreach (string link in links)
            {
                string editLink = link.Replace(".html", "");
                if (link.IndexOf("uploaded", StringComparison.OrdinalIgnoreCase) >= 0)
                    downloadKind = 1;
                else if (link.IndexOf("extmatrix", StringComparison.OrdinalIgnoreCase) >= 0)
                    downloadKind = 2;

                if (regex.IsMatch(editLink))
                {
                    if (downloadKind == 1)
                        movieLinks.Add(link);
                    else
                    {
                        if (editLink.IndexOf("1080", StringComparison.OrdinalIgnoreCase) >= 0)
                            movieLinks.Add(link);
                    }
                }
            }

            if (movieLinks.Count <= 0)
                return myDownloadLinks;

            return string.Join(" ", movieLinks);
        }
    }
}
