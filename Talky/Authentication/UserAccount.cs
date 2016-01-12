using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Talky.Database;

namespace Talky.Authentication
{
    class UserAccount
    {

        public int AccountId { get; private set; }
        public string Username { get; private set; }
        public string CreatedAt { get; private set; }
        public string LastLogin { get; private set; }
        public string Role { get; private set; }

        private UserAccount(int accountId, string username, string createdAt, string lastLogin, string role)
        {
            AccountId = accountId;
            Username = username;
            CreatedAt = createdAt;
            LastLogin = lastLogin;
            Role = role;
        }

        public static string Hash(string password)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            return Encoding.ASCII.GetString(sha1.ComputeHash(Encoding.ASCII.GetBytes(password)));
        }

        public static UserAccount Find(string username)
        {
            MySqlConnection connection = MySQLConnector.GetConnection();

            if (connection != null)
            {
                MySqlCommand command = new MySqlCommand("SELECT `id`,`username`,`created_at`,`last_login`,`role` FROM `users` WHERE `username`=@username ORDER BY `id` ASC LIMIT 1", connection);
                command.Prepare();
                command.Parameters.AddWithValue("@username", username);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    UserAccount account = new UserAccount(reader.GetInt32("id"), reader.GetString("username"), reader.GetString("created_at"), reader.GetString("last_login"), reader.GetString("role"));
                    connection.Close();
                    return account;
                }
            }

            return null;
        }

        public static UserAccount Create(string username, string password, bool admin)
        {
            string role = (admin ? "admin" : "user");

            if (username.Length > 16 || string.IsNullOrEmpty(password))
            {
                return null;
            }

            if (Find(username) != null)
            {
                return null;
            }

            MySqlConnection connection = MySQLConnector.GetConnection();

            if (connection != null)
            {
                MySqlCommand command = new MySqlCommand("INSERT INTO `users` VALUES(NULL, @username, @password, NOW(), NOW(), @role)", connection);
                command.Prepare();
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", Hash(password));
                command.Parameters.AddWithValue("@role", role);
                command.ExecuteReader();
                connection.Close();
                return Find(username);
            }

            return null;
        }

        public static UserAccount Attempt(string username, string password)
        {
            if (username.Length > 16 || string.IsNullOrEmpty(password))
            {
                return null;
            }

            MySqlConnection connection = MySQLConnector.GetConnection();

            if (connection != null)
            {
                MySqlCommand command = new MySqlCommand("SELECT `id`,`username`,`created_at`,`last_login`,`role` FROM `users` WHERE `username`=@username AND `password`=@password ORDER BY `id` ASC LIMIT 1", connection);
                command.Prepare();
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", Hash(password));
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    UserAccount account = new UserAccount(id, reader.GetString("username"), reader.GetString("created_at"), reader.GetString("last_login"), reader.GetString("role"));
                    connection.Close();

                    connection = MySQLConnector.GetConnection();
                    MySqlCommand updateCommand = new MySqlCommand("UPDATE `users` SET `last_login`=NOW() WHERE `id`=@id ORDER BY `id` ASC LIMIT 1", connection);
                    updateCommand.Prepare();
                    updateCommand.Parameters.AddWithValue("@id", id);
                    updateCommand.ExecuteReader();
                    connection.Close();

                    return account;
                }
            }

            return null;
        }

    }
}
