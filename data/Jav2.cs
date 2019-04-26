using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfScrapingRegister.data
{
    class Jav2Data : INotifyPropertyChanged
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

        public string Package { get; set; }

        public string Thumbnail { get; set; }

        public string DownloadLinks { get; set; }

        public string FilesInfo { get; set; }

        public string Kind { get; set; }

        public string Url { get; set; }

        public string Detail { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
