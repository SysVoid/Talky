using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talky.Configuration;

namespace Talky.Database
{
    class MySQLConnector
    {

        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection;

            try
            {
                Dictionary<string, string> defaults = new Dictionary<string, string>();
                defaults.Add("connectionString", "datasource=localhost;port=3306;username=talky;password=talky;");
                defaults.Add("database", "talky");

                ConfigurationFile config = new ConfigurationFile("database");
                if (!config.Exists())
                {
                    config.Write(defaults);
                }

                string connectionString;
                string db;
                config.Values(defaults).TryGetValue("connectionString", out connectionString);
                config.Values(defaults).TryGetValue("database", out db);

                connection = new MySqlConnection(connectionString);
                connection.Open();
                connection.ChangeDatabase(db);

                return connection;
            } catch (MySqlException e)
            {
                Program.Instance.OHGODNO("Terminate!! Could not connect to MySQL!! Danger!! Danger!!", e);
                return null;
            }
        }

    }
}
