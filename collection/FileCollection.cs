using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using wpfScrapingRegister.data;

namespace wpfScrapingRegister.collection
{
    class FileCollection
    {
        public static string REGEX_TARGETFILE_EXTENTION = @".*\.avi$|.*\.wmv$|.*\.mpg$|.*ts$|.*divx$|.*mp4$|.*asf$|.*png$|.*jpg$|.*jpeg$|.*iso$|.*mkv$|.*\.m4v|.*\.rmvb|.*\.rm|.*\.rar|.*\.mov|.*\.3gp";
        public static string REGEX_TARGET_MOVIE_EXTENTION = @".*\.avi$|.*\.wmv$|.*\.mpg$|.*ts$|.*divx$|.*mp4$|.*asf$|.*iso$|.*mkv$|.*\.m4v|.*\.rmvb|.*\.rm|.*\.mov|.*\.3gp";
        public static string REGEX_TARGET_ARCHIVE_EXTENTION = @".*\.rar$";

        public List<FileInfo> listContents = null;
        public ICollectionView collecion;

        private string SearchText = "";
        private int SearchIsSelectino = -9;

        public FileCollection()
        {
            Regex regex = new Regex(REGEX_TARGETFILE_EXTENTION, RegexOptions.IgnoreCase);
            Regex regexEdited = new Regex("^\\[AV|^\\[裏AV|^\\[IV");

            string[] files = Directory.GetFiles(@"D:\DATA\Downloads", "*", System.IO.SearchOption.TopDirectoryOnly);
            //string[] files = Directory.GetFiles(@"C:\mydata", "*", System.IO.SearchOption.TopDirectoryOnly);

            if (listContents == null)
                listContents = new List<FileInfo>();
            else
                listContents.Clear();

            foreach (var file in files)
            {
                if (!regex.IsMatch(file))
                    continue;

                FileInfo fileinfo = new FileInfo(file.ToString());

                if (regexEdited.IsMatch(fileinfo.Name))
                    continue;

                listContents.Add(fileinfo);
            }
            collecion = CollectionViewSource.GetDefaultView(listContents);

            return;
        }

        public void SetSearchText(string mySearchText)
        {
            SearchText = mySearchText;
        }

        public void SetSearchSelection(int myIsSelection)
        {
            SearchIsSelectino = myIsSelection;
        }

        public void Execute()
        {
            string[] arrSearchText = null;

            if (SearchText != null)
            {
                arrSearchText = SearchText.Split(' ');
            }
            collecion.Filter = delegate (object o)
            {
                if (!(o is FileInfo data))
                    return false;

                bool r = false;
                if (arrSearchText == null || arrSearchText.Length <= 0)
                    return true;

                foreach (string s in arrSearchText)
                {
                    if (s.IndexOf("_", StringComparison.Ordinal) >= 0)
                    {
                        string[] splits = s.Split('_');

                        int matchCount = 0;
                        foreach (string split in splits)
                        {
                            if (data.Name.IndexOf(split, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // Debug.Print(s + " " + data.Name);
                                matchCount++;
                                continue;
                            }

                        }
                        if (matchCount >= splits.Length)
                            r = true;
                    }
                    else
                    {
                        if (data.Name.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                            r = true;
                    }
                }

                return r;
            };
        }

    }
}
