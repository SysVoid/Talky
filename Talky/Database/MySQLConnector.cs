using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Talky.Database
{
    class MySQLConnector
    {

        public static MySqlConnection GetConnection()
        {
            MySqlConnection connection;

            try
            {
                connection = new MySqlConnection("datasource=localhost;port=3306;username=talky;password=talky;");
                connection.Open();
                connection.ChangeDatabase("talky");

                return connection;
            } catch (MySqlException e)
            {
                Program.Instance.OHGODNO("Terminate!! Could not connect to MySQL!! Danger!! Danger!!", e);
                return null;
            }
        }

    }
}
