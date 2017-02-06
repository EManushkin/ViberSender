using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViberSender2017
{
    public static class WorkBD
    {
        private static SQLiteDataAdapter sql = new SQLiteDataAdapter();
        private static DataTable dt = new DataTable();
        private static BindingSource bs = new BindingSource();
        private static string path_config = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates)) + "Users\\" + Environment.UserName + "\\AppData\\Roaming\\ViberPC\\config.db";
        private static SQLiteFactory factory = (SQLiteFactory)null;
        private static SQLiteConnection connection = (SQLiteConnection)null;

        public static BindingSource DownloadBD()
        {
            try
            {
                DataColumnCollection columns = WorkBD.dt.Columns;
                DataColumn column = new DataColumn();
                column.AutoIncrementSeed = 1L;
                int num = 1;
                column.AutoIncrement = num != 0;
                string str = "№";
                column.ColumnName = str;
                columns.Add(column);
                WorkBD.factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
                WorkBD.connection = (SQLiteConnection)WorkBD.factory.CreateConnection();
                WorkBD.connection.ConnectionString = "Data Source = " + WorkBD.path_config;
                WorkBD.connection.Open();
                SQLiteCommand sqLiteCommand = new SQLiteCommand(WorkBD.connection);
                sqLiteCommand.CommandText = "SELECT * FROM Accounts";
                sqLiteCommand.CommandType = CommandType.Text;
                sqLiteCommand.ExecuteNonQuery();
                WorkBD.sql.SelectCommand = sqLiteCommand;
                WorkBD.sql.Fill(WorkBD.dt);
                WorkBD.bs.DataSource = (object)WorkBD.dt;
                WorkBD.connection.Close();
            }
            catch
            {
            }
            return WorkBD.bs;
        }

        public static void SetAcc(string phone)
        {
            try
            {
                WorkBD.connection.ConnectionString = "Data Source = " + WorkBD.path_config;
                WorkBD.connection.Open();
                SQLiteCommand sqLiteCommand = new SQLiteCommand(WorkBD.connection);
                sqLiteCommand.CommandText = "UPDATE 'Accounts' SET 'IsDefault'='0'";
                sqLiteCommand.CommandType = CommandType.Text;
                sqLiteCommand.ExecuteNonQuery();
                sqLiteCommand.CommandText = "UPDATE \"main\".\"Accounts\" SET \"IsDefault\"=1 WHERE \"ID\"=\"" + phone + "\"";
                sqLiteCommand.CommandType = CommandType.Text;
                sqLiteCommand.ExecuteNonQuery();
                WorkBD.connection.Close();
            }
            catch
            {
            }
        }

        public static void OffAccs()
        {
            try
            {
                WorkBD.connection.ConnectionString = "Data Source = " + WorkBD.path_config;
                WorkBD.connection.Open();
                SQLiteCommand sqLiteCommand = new SQLiteCommand(WorkBD.connection);
                sqLiteCommand.CommandText = "UPDATE 'Accounts' SET 'IsValid'='0'";
                sqLiteCommand.CommandType = CommandType.Text;
                sqLiteCommand.ExecuteNonQuery();
                WorkBD.connection.Close();
            }
            catch
            {
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static async Task<BindingSource> WaitNewAcc()
        {
            try
            {
                WorkBD.connection.ConnectionString = "Data Source = " + WorkBD.path_config;
                WorkBD.connection.Open();
                SQLiteCommand command = new SQLiteCommand(WorkBD.connection);
                command.CommandText = "SELECT COUNT(*) FROM 'Accounts'";
                command.CommandType = CommandType.Text;
                int i = Convert.ToInt32(command.ExecuteScalar());
                while (WorkBD.FindWindow("Qt5QWindowOwnDCIcon", (string)null) == IntPtr.Zero)
                    await Task.Delay(100);
                while (WorkBD.FindWindow("Qt5QWindowOwnDCIcon", (string)null) != IntPtr.Zero)
                {
                    await Task.Delay(100);
                    if (i < Convert.ToInt32(command.ExecuteScalar()))
                        break;
                }
                command.CommandText = "UPDATE 'Accounts' SET 'IsValid'='1'";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                command.CommandText = "SELECT * FROM Accounts";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                WorkBD.dt.Clear();
                WorkBD.sql.SelectCommand = command;
                WorkBD.sql.Fill(WorkBD.dt);
                WorkBD.bs.DataSource = (object)WorkBD.dt;
                WorkBD.connection.Close();
                return WorkBD.bs;
            }
            catch
            {
                return WorkBD.bs;
            }
        }

        public static void ClearHistory(string phone)
        {
            try
            {
                if (Directory.Exists(WorkBD.path_config.Replace("config.db", "") + "\\" + phone.Replace("+", "") + "\\Avatars"))
                    Directory.Delete(WorkBD.path_config.Replace("config.db", "") + "\\" + phone.Replace("+", "") + "\\Avatars", true);
                WorkBD.connection.ConnectionString = "Data Source = " + WorkBD.path_config.Replace("config.db", "") + "\\" + phone.Replace("+", "") + "\\viber.db";
                WorkBD.connection.Open();
                SQLiteCommand sqLiteCommand = new SQLiteCommand(WorkBD.connection);
                sqLiteCommand.CommandText = "delete from Messages";
                sqLiteCommand.CommandType = CommandType.Text;
                sqLiteCommand.ExecuteNonQuery();
                sqLiteCommand.CommandText = "delete from ChatInfo";
                sqLiteCommand.ExecuteNonQuery();
                WorkBD.connection.Close();
            }
            catch
            {
            }
        }

        public static void SetLanguage(string phone)
        {
            try
            {
                WorkBD.connection.ConnectionString = "Data Source = " + WorkBD.path_config.Replace("config.db", "") + "\\" + phone.Replace("+", "") + "\\viber.db";
                WorkBD.connection.Open();
                SQLiteCommand sqLiteCommand = new SQLiteCommand(WorkBD.connection);
                sqLiteCommand.CommandText = "UPDATE \"main\".\"Settings\" SET \"SettingValue\" = 'ru' WHERE \"SettingTitle\" = 'UILanguage'";
                sqLiteCommand.CommandType = CommandType.Text;
                sqLiteCommand.ExecuteNonQuery();
                WorkBD.connection.Close();
            }
            catch
            {
            }
        }
    }
}
