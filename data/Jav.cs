using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfScrapingRegister.data
{
    class JavData : INotifyPropertyChanged
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
        public string Name { get; set; }
        public DateTime PostDate { get; set; }

        public string Package { get; set; }

        public string Thumbnail { get; set; }

        public DateTime SellDate { get; set; }

        public string Actress { get; set; }

        public string Maker { get; set; }

        public string Label { get; set; }

        public string DownloadLinks { get; set; }

        public string FilesInfo { get; set; }

        public string ProductNumber { get; set; }

        public string DownloadFiles { get; set; }

        private int _Rating;
        public int Rating
        {
            get
            {
                return _Rating;
            }
            set
            {
                _Rating = value;
                NotifyPropertyChanged("Rating");
            }
        }

        public string Url { get; set; }

        private int _IsSelection;
        public int IsSelection
        {
            get
            {
                return _IsSelection;
            }
            set
            {
                _IsSelection = value;
                NotifyPropertyChanged("IsSelection");
            }
        }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
