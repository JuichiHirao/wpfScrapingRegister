using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using wpfScrapingRegister.data;

namespace wpfScrapingRegister.dao
{
    class Jav2Dao : BaseDao
    {
        public List<Jav2Data> GetList()
        {
            cn.Open();

            MySqlCommand command =
                new MySqlCommand(
                    "SELECT id, title, post_date, package, thumbnail, download_links, files_info, kind, url, detail FROM jav2 ORDER BY id",
                    cn);
            MySqlDataReader reader = command.ExecuteReader();

            List<Jav2Data> list = new List<Jav2Data>();
            while (reader.Read())
            {
                Jav2Data data = new Jav2Data();

                int columnNo = 0;
                data.Id = DbExportCommon.GetDbInt(reader, columnNo);
                data.Title = DbExportCommon.GetDbString(reader, ++columnNo);
                data.PostDate = DbExportCommon.GetDbDateTime(reader, ++columnNo);
                data.Package = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Thumbnail = DbExportCommon.GetDbString(reader, ++columnNo);
                data.DownloadLinks = DbExportCommon.GetDbString(reader, ++columnNo);
                data.FilesInfo = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Kind = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Url = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Detail = DbExportCommon.GetDbString(reader, ++columnNo);

                list.Add(data);
            }

            cn.Close();

            return list;
        }

    }
}
