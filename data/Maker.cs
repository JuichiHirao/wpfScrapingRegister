using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpfScrapingRegister.data
{
    class MakerData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public int Kind { get; set; }
        public string MatchStr { get; set; }
        public string MatchProductNumber { get; set; }
        public string MatchProductNumberValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string GetNameLabel()
        {
            string namelabel = "";
            if (Label == null || Label.Length <= 0)
                namelabel = Name;
            else
                namelabel = Name + "：" + Label;

            return namelabel;
        }

    }
}
