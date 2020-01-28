using System;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using Microsoft.CSharp;
using Codeplex.Data;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

namespace wpfScrapingRegister.dao
{
    class BaseDao
    {
        class MysqlConnectInfo
        {

        }
        protected MySqlConnection cn = null;

        public static string DockerDatabase = "av";
        public static string DockerDataSource = "127.0.0.1";
        public static string DockerPort = "43306";
        public static string DockerUser = "root";
        public static string DockerPassword = "mysql";

        public BaseDao()
        {
            string target = "mysql";

            if (!File.Exists("credential.json"))
            {
                throw new Exception("credential.jsonがDebug or Release配下に存在しません");
            }
            string json = File.ReadAllText("credential.json");
            var obj = DynamicJson.Parse(json);
            string database = "", datasource = "", user = "", password = "";
            try
            {
                var targetObj = obj[target];

                database = targetObj.database;
                datasource = targetObj.datasource;
                user = targetObj.user;
                password = targetObj.password;
            }
            catch (RuntimeBinderException ex)
            {
                throw new Exception("credential.jsonに存在しない[" + target + "]が指定されました or " + ex.Message);
            }

            String connectionInfo = "Database=" + database + "; Data Source=" + datasource + ";User Id=" + user + "; Password=" + password;
            cn = new MySqlConnection(connectionInfo);
        }

        public BaseDao(string myDatabase, string myDataSource, string myPort, string myUser, string myPassword)
        {
            String connectionInfo = "Database=" + myDatabase + "; Data Source=" + myDataSource + "; port=" + myPort + "; User Id=" + myUser + "; Password=" + myPassword + "; ConnectionTimeout=600; DefaultCommandTimeout=600";
            cn = new MySqlConnection(connectionInfo);
        }

    }
}
