using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using wpfScrapingRegister.data;

namespace wpfScrapingRegister.dao
{
    class MakerDao : BaseDao
    {
        public List<MakerData> GetList()
        {
            cn.Open();

            string sql = "SELECT id, name, label, kind "
                         + ", match_str, match_product_number "
                         + ", created_at, updated_at "
                         + "FROM maker "
                         + "WHERE deleted = 0 ";

            MySqlCommand command =
                new MySqlCommand(sql,
                    cn);
            MySqlDataReader reader = command.ExecuteReader();

            List<MakerData> list = new List<MakerData>();
            while (reader.Read())
            {
                MakerData data = new MakerData();

                int columnNo = 0;
                data.Id = DbExportCommon.GetDbInt(reader, columnNo);
                data.Name = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Label = DbExportCommon.GetDbString(reader, ++columnNo);
                data.Kind = DbExportCommon.GetDbInt(reader, ++columnNo);
                data.MatchStr = DbExportCommon.GetDbString(reader, ++columnNo);
                data.MatchProductNumber = DbExportCommon.GetDbString(reader, ++columnNo);
                data.CreatedAt = DbExportCommon.GetDbDateTime(reader, ++columnNo);
                data.UpdatedAt = DbExportCommon.GetDbDateTime(reader, ++columnNo);

                list.Add(data);
            }

            cn.Close();

            return list;
        }

    }
}
