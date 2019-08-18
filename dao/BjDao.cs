using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using wpfScrapingRegister.data;

namespace wpfScrapingRegister.dao
{
    class BjDao : BaseDao
    {
        public List<BjData> GetList()
        {
            cn.Open();

            MySqlCommand command =
                new MySqlCommand(
                    "SELECT id, title, post_date, thumbnails, thumbnails_count, download_link, url, posted_in, is_selection, is_downloads FROM bj ORDER BY post_date",
                    cn);
            MySqlDataReader reader = command.ExecuteReader();

            List<BjData> list = new List<BjData>();
            while (reader.Read())
            {
                BjData data = new BjData();

                data.Id = DbExportCommon.GetDbInt(reader, 0);
                data.Title = DbExportCommon.GetDbString(reader, 1);
                data.PostDate = DbExportCommon.GetDbDateTime(reader, 2);
                data.Thumbnails = DbExportCommon.GetDbString(reader, 3);
                data.ThumbnailsCount = DbExportCommon.GetDbInt(reader, 4);
                data.DownloadLink = DbExportCommon.GetDbString(reader, 5);
                data.Url = DbExportCommon.GetDbString(reader, 6);
                data.PostedIn = DbExportCommon.GetDbString(reader, 7);
                data.IsSelection = DbExportCommon.GetDbInt(reader, 8);
                data.IsDownloads = DbExportCommon.GetDbInt(reader, 9);

                list.Add(data);
            }

            cn.Close();

            return list;
        }
        public void UpdateIsSelection(int myValue, int myId)
        {
            cn.Open();

            MySqlCommand command = new MySqlCommand("UPDATE bj SET IS_SELECTION = @IsSelection WHERE id = @Id", cn);

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@IsSelection", MySqlDbType.Int16);
            sqlparam.Value = myValue;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@Id", MySqlDbType.Int16);
            sqlparam.Value = myId;
            command.Parameters.Add(sqlparam);

            int cnt = command.ExecuteNonQuery();

            Debug.Print("UPDATE ID [" + myId + "] " + cnt + "件");

            cn.Close();
        }

    }
}
