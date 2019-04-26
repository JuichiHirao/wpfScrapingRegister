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
    class JavDao : BaseDao
    {
        public List<JavData> GetList()
        {
            cn.Open();

            //"SELECT id, title, post_date, package, thumbnail, sell_date, download_links, product_number, is_selection FROM jav ORDER BY post_date",
            string sql = "SELECT id, title, post_date, package "
                         + ", thumbnail, sell_date, actress, maker"
                         + ", label, download_links, url, product_number "
                         + ", is_selection, rating, download_files, files_info "
                         + "FROM jav ORDER BY post_date ";

            MySqlCommand command =
                new MySqlCommand(sql,
                    cn);
            MySqlDataReader reader = command.ExecuteReader();

            List<JavData> list = new List<JavData>();
            while (reader.Read())
            {
                JavData data = new JavData();

                int columnNo = 0;
                data.Id = DbExportCommon.GetDbInt(reader, columnNo);
                data.Title = DbExportCommon.GetDbString(reader, ++columnNo);
                data.PostDate = DbExportCommon.GetDbDateTime(reader, ++columnNo);
                data.Package = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Thumbnail = DbExportCommon.GetDbString(reader, ++columnNo);
                data.SellDate = DbExportCommon.GetDbDateTime(reader, ++columnNo);
                data.Actress = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Maker = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Label = DbExportCommon.GetDbString(reader, ++columnNo);
                data.DownloadLinks = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Url = DbExportCommon.GetDbString(reader, ++columnNo);
                data.ProductNumber = DbExportCommon.GetDbString(reader, ++columnNo);
                data.IsSelection = DbExportCommon.GetDbInt(reader, ++columnNo);
                data.Rating = DbExportCommon.GetDbInt(reader, ++columnNo);
                data.DownloadFiles = DbExportCommon.GetDbString(reader, ++columnNo);
                data.FilesInfo = DbExportCommon.GetDbString(reader, ++columnNo);

                list.Add(data);
            }

            cn.Close();

            return list;
        }

        public void Update(JavData javData)
        {
            cn.Open();

            MySqlCommand command = new MySqlCommand("UPDATE jav SET name = @Name, sell_date = @SellDate, actress = @Actress, product_number = @ProductNumber, package = @Package, thumbnail = @Thumbnail, download_files = @DownloadFiles WHERE id = @Id", cn);

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@Name", MySqlDbType.Text);
            sqlparam.Value = javData.Name;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@SellDate", MySqlDbType.Date);
            sqlparam.Value = javData.SellDate;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@Actress", MySqlDbType.VarChar);
            sqlparam.Value = javData.Actress;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@ProductNumber", MySqlDbType.VarChar);
            sqlparam.Value = javData.ProductNumber;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@Package", MySqlDbType.VarChar);
            sqlparam.Value = javData.Package;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@Thumbnail", MySqlDbType.VarChar);
            sqlparam.Value = javData.Thumbnail;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@DownloadFiles", MySqlDbType.VarChar);
            sqlparam.Value = javData.DownloadFiles;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@Id", MySqlDbType.Int16);
            sqlparam.Value = javData.Id;
            command.Parameters.Add(sqlparam);

            int cnt = command.ExecuteNonQuery();

            Debug.Print("UPDATE ID [" + javData.Id + "] " + cnt + "件");

            cn.Close();
        }

        public void UpdateDownloadFiles(JavData javData, string myDownloadFiles)
        {
            cn.Open();

            MySqlCommand command = new MySqlCommand("UPDATE jav SET download_files = @DownloadFiles WHERE id = @Id", cn);

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@DownloadFiles", MySqlDbType.Text);
            sqlparam.Value = myDownloadFiles;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@Id", MySqlDbType.Int32);
            sqlparam.Value = javData.Id;
            command.Parameters.Add(sqlparam);

            int cnt = command.ExecuteNonQuery();

            Debug.Print("UPDATE ID [" + javData.Id + "] " + cnt + "件");

            cn.Close();
        }

        public void UpdateRating(JavData javData)
        {
            cn.Open();

            MySqlCommand command = new MySqlCommand("UPDATE jav SET rating = @Rating WHERE id = @Id", cn);

            List<MySqlParameter> listSqlParams = new List<MySqlParameter>();

            MySqlParameter sqlparam = new MySqlParameter("@Name", MySqlDbType.Text);
            sqlparam.Value = javData.Name;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@Rating", MySqlDbType.Int32);
            sqlparam.Value = javData.Rating;
            command.Parameters.Add(sqlparam);

            sqlparam = new MySqlParameter("@Id", MySqlDbType.Int32);
            sqlparam.Value = javData.Id;
            command.Parameters.Add(sqlparam);

            int cnt = command.ExecuteNonQuery();

            Debug.Print("UPDATE ID [" + javData.Id + "] Rating [" + javData.Rating + "]");

            cn.Close();
        }

        public void UpdateIsSelection(int myValue, int myId)
        {
            cn.Open();

            MySqlCommand command = new MySqlCommand("UPDATE jav SET IS_SELECTION = @IsSelection WHERE id = @Id", cn);

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