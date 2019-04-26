using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfScrapingRegister.data
{
    class BjData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PostDate { get; set; }

        public string Thumbnails { get; set; }

        public int ThumbnailsCount { get; set; }

        public DateTime SellDate { get; set; }

        public string DownloadLink { get; set; }

        public string Url { get; set; }

        public string PostedIn { get; set; }

        private int _IsSelection;

        public int IsSelection
        {
            get { return _IsSelection; }
            set
            {
                _IsSelection = value;
                NotifyPropertyChanged("IsSelection");
            }
        }

        private int _IsDownloads;

        public int IsDownloads
        {
            get { return _IsDownloads; }
            set
            {
                _IsDownloads = value;
                NotifyPropertyChanged("IsDownloads");
            }
        }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
