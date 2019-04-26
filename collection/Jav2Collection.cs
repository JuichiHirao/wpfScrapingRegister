using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using wpfScrapingRegister.data;

namespace wpfScrapingRegister.collection
{
    class Jav2Collection
    {
        public List<Jav2Data> listContents;
        public ICollectionView collecion;

        private string SearchText = "";

        public Jav2Collection(List<Jav2Data> myJav2List)
        {
            listContents = myJav2List;
            collecion = CollectionViewSource.GetDefaultView(listContents);
            collecion.SortDescriptions.Clear();
            collecion.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
        }

        public void SetSearchText(string mySearchText)
        {
            SearchText = mySearchText;
        }

        public void Execute()
        {
            string[] ArrSearchText = null;

            bool IsFilterFreeWords = false;

            if (SearchText != null)
            {
                ArrSearchText = SearchText.Split(' ');
            }
            IsFilterFreeWords = true;

            collecion.Filter = delegate (object o)
            {
                Jav2Data data = o as Jav2Data;

                if (IsFilterFreeWords)
                {
                    if (IsFilterFreeWords)
                    {
                        bool r = false;
                        foreach (string s in ArrSearchText)
                        {
                            if (s.IndexOf("_", StringComparison.Ordinal) >= 0)
                            {
                                string[] splits = s.Split('_');

                                int matchCount = 0;
                                foreach (string split in splits)
                                {
                                    if (data.Title.IndexOf(split, StringComparison.OrdinalIgnoreCase) >= 0)
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
                                if (data.Title.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                                    r = true;
                            }
                        }
                        if (r == false)
                            return r;
                    }
                }

                return true;
            };
        }

    }
}
