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
    class MakerCollection
    {
        public List<MakerData> listContents;
        public ICollectionView collecion;

        private string SearchText = "";
        private int SearchIsSelectino = -9;

        public MakerCollection(List<MakerData> myMakerList)
        {
            listContents = myMakerList;
            //collecion = CollectionViewSource.GetDefaultView(listContents);
            //collecion.SortDescriptions.Clear();
            //collecion.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Descending));
        }

        public string GetMatchStr(string myMatchStr)
        {
            List<MakerData> matchList = new List<MakerData>();

            foreach (MakerData data in listContents)
            {
                if (myMatchStr.Equals(data.MatchStr, StringComparison.OrdinalIgnoreCase))
                    matchList.Add(data);
            }

            if (matchList.Count == 1)
                return matchList[0].GetNameLabel();

            return "";
        }

        public void SetSearchSelection(int myIsSelection)
        {
            SearchIsSelectino = myIsSelection;
        }

        public void Execute()
        {
            string[] arrSearchText = null;

            bool IsFilterFreeWords = false;

            if (SearchText != null)
            {
                arrSearchText = SearchText.Split(' ');
            }
            IsFilterFreeWords = true;

            collecion.Filter = delegate (object o)
            {
                JavData data = o as JavData;

                if (data == null)
                    return false;

                if (IsFilterFreeWords)
                {
                    if (IsFilterFreeWords)
                    {
                        bool r = false;
                        if (arrSearchText == null)
                            return true;

                        foreach (string s in arrSearchText)
                        {
                            if (data.Title.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                                r = true;
                        }
                        if (r == false)
                            return r;
                    }
                }

                if (SearchIsSelectino != -9)
                {
                    if (data.IsSelection == SearchIsSelectino)
                        return true;
                    else
                        return false;
                }

                return true;
            };
        }

    }
}
