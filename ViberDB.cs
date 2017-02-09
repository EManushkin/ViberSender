using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViberSender2017
{
    public static class ViberDB
    {
        private static SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
        private static SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection();
        private static string config_path = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates)) + "Users\\" + Environment.UserName + "\\AppData\\Roaming\\ViberPC\\[account]\\viber.db";

        public static int GetMessageStatus(string account, string phone)
        {
            int status = 0;
            config_path = config_path.Replace("[account]", account.Replace("+", ""));
            if (File.Exists(config_path))
            {
                try
                {
                    connection.ConnectionString = "Data Source = " + config_path;
                    connection.Open();
                    SQLiteDataReader reader;
                    SQLiteCommand sqLiteCommand = new SQLiteCommand(connection);
                    sqLiteCommand.CommandText = String.Format("SELECT `MessageStatus`, `Number`, `EventID` FROM `EventInfo` WHERE Number = '{0}' ORDER BY EventID DESC LIMIT 1", phone);
                    sqLiteCommand.CommandType = CommandType.Text;
                    reader = sqLiteCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        status = reader.GetInt32(0);
                    }
                    connection.Close();
                }
                catch
                {
                } 
            }
            return status;
        }
    }
}
