using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab16_Exercise3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Vorname: ");
            string vorname = Console.ReadLine();
            Console.WriteLine("Nachname: ");
            string nachname = Console.ReadLine();
            Console.WriteLine("Alter: ");
            string alter = Console.ReadLine();

            string query = "INSERT INTO Personen ([Vorname], [Nachname], [Alter]) VALUES (@vorname, @nachname, @alter)";
            Console.WriteLine(query);

            SqlCommand command = new SqlCommand(query);
            SqlParameter paraVorname = new SqlParameter("@vorname", vorname);
            SqlParameter paraNachname = new SqlParameter("@nachname", nachname);
            SqlParameter paraAlter = new SqlParameter("@alter", alter);
            command.Parameters.Add(paraVorname);
            command.Parameters.Add(paraNachname);
            command.Parameters.Add(paraAlter);

            string connectionString = @"xxxxxxxx";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                command.Connection = connection;
                connection.Open();
                command.ExecuteNonQuery(); 
            }
        }
    }
}
