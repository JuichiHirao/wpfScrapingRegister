using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using wpfScrapingRegister.data;

namespace WpfScrapingRegister.collection
{
    class BjCollection
    {
        public List<BjData> listContents;
        public ICollectionView collecion;

        private string SearchText = "";
        private int SearchIsSelectino = -9;

        public BjCollection(List<BjData> myBjList)
        {
            listContents = myBjList;
            collecion = CollectionViewSource.GetDefaultView(listContents);
            collecion.SortDescriptions.Clear();
            collecion.SortDescriptions.Add(new SortDescription("PostDate", ListSortDirection.Ascending));
        }

        public void SetDataIsSelection(int myId, int myIsSelection)
        {
            foreach (BjData jav in listContents)
            {
                if (jav.Id == myId)
                    jav.IsSelection = myIsSelection;
            }
        }

        public void SetSearchText(string mySearchText)
        {
            SearchText = mySearchText;
            SearchIsSelectino = -9;
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
                BjData data = o as BjData;

                if (data == null)
                    return false;

                if (IsFilterFreeWords)
                {
                    bool r = false;
                    if (arrSearchText == null)
                        return true;

                    foreach (string s in arrSearchText)
                    {
                        if (data.Title.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0
                            || data.DownloadLink.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Debug.Print("[" + s + "] " + data.Title + "");
                            r = true;
                        }
                    }

                    if (r == false)
                        return r;
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
