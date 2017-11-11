using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BloodDonation
{
    public static class DatabaseAdapter
    {
        private static string UserID { get; set; } = "geartest";
        private static string UserPass { get; set; } = "Ck2143!Eq66-";
        private static string DataSource { get; set; } = "mssql6.gear.host";
        private static SqlConnectionStringBuilder dbConStringBuilder { get; set; }

        private static SqlConnection connection;

        public static async Task<SqlConnection> GetConnection()
        {
            if (connection == null)
            {
                dbConStringBuilder = new SqlConnectionStringBuilder();
                dbConStringBuilder.UserID = UserID;
                dbConStringBuilder.Password = UserPass;
                dbConStringBuilder.DataSource = DataSource;
                dbConStringBuilder.ConnectTimeout = 30;
                connection = new SqlConnection(dbConStringBuilder.ConnectionString);
                await connection.OpenAsync();
                return connection;
            }
            else
            {
                return connection;
            }
        }

        public static async Task<List<Person>> GetPersons()
        {
            var command = new SqlCommand("select name from persons", GetConnection().Result);

            List<Person> personList = new List<Person>();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    personList.Add(new Person()
                    {
                        CustomerId = 0,
                        CustomerName = Convert.ToString(reader["name"])
                    });
                }
            }

            return personList;
        }

        public static async Task<Person> LogIn(Credentials credentials)
        {
            var command = new SqlCommand($"select top 1 id, name from persons where login like '{credentials.User}' and pass like '{credentials.Pass}'", GetConnection().Result);

            command.CommandTimeout = 30;

            var person = new Person();

            using (SqlDataReader reader = command.ExecuteReader())
            {

                if (await reader.ReadAsync() == false)
                    return null;

                person.CustomerId = Convert.ToInt32(reader["id"]);
                person.CustomerName = Convert.ToString(reader["name"]);

                return person;
            }
        }
    }
}