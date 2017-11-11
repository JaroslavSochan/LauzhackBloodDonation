using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AndroidSmartAirports
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

        public static async Task<Flight> GetFlights(int flightID)
        {
            var command = new SqlCommand($"select top 1 name, source, destination, time , gate from flights where id = {flightID}", GetConnection().Result);

            Flight flight = new Flight();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    flight.Name = Convert.ToString(reader["name"]);
                    flight.Source = Convert.ToString(reader["source"]);
                    flight.Destination = Convert.ToString(reader["destination"]);
                    flight.Gate = Convert.ToString(reader["gate"]);
                    flight.Time = DateTime.Parse(Convert.ToString(reader["time"]));
                }
            }

            return flight;
        }

        public static async Task<List<Flight>> GetAllFlightsForPassenger(int passengerID)
        {
            var command = new SqlCommand($"select f.name, f.source, f.destination,f.time, f.gate from Flights f inner join PassengersInflights p on f.id = p.flight_id where p.pass_id = {passengerID};", GetConnection().Result);

            List<Flight> listFlights = new List<Flight>();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    Flight flight = new Flight();
                    flight.Name = Convert.ToString(reader["name"]);
                    flight.Source = Convert.ToString(reader["source"]);
                    flight.Destination = Convert.ToString(reader["destination"]);

                    flight.Time = DateTime.Parse(Convert.ToString(reader["time"]));
                    listFlights.Add(flight);
                }
            }

            return listFlights;
        }

        public static async Task<List<Payment>> GetAllPaymentForPassenger(int passengerID)
        {
            var command = new SqlCommand($"select shop, value from Payments where id_pass = {passengerID}", GetConnection().Result);

            List<Payment> listPayments = new List<Payment>();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    Payment payment = new Payment();
                    payment.Shop = Convert.ToString(reader["shop"]);
                    payment.Value = Convert.ToInt32(reader["value"]);
                    listPayments.Add(payment);
                }
            }

            return listPayments;
        }

        public static async Task<Position> GetPositionForPassenger(int passengerID)
        {
            var command = new SqlCommand($"select top 1 x,y,prevX,prevY from persons where id = {passengerID}", GetConnection().Result);

            Position position = new Position();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (await reader.ReadAsync())
                {
                    position.X = Convert.ToInt32(reader["X"]);
                    position.Y = Convert.ToInt32(reader["Y"]);
                    position.PrevX = Convert.ToInt32(reader["PrevX"]);
                    position.PrevY = Convert.ToInt32(reader["PrevY"]);
                }
            }

            return position;
        }
    }
}