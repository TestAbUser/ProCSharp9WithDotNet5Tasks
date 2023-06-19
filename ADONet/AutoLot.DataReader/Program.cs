using System;
using Microsoft.Data.SqlClient;

Console.WriteLine("***** Fun with Data Readers *****\n");

// Create and open a connection.
using (SqlConnection connection = new ())
{
    connection.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Integrated Security=true;Initial Catalog=AutoLot";
    connection.Open();

    // Create a SQL command object.
    string sql =
    @"Select i.id, m.Name as Make, i.Color, i.Petname
    FROM Inventory i
    INNER JOIN Makes m on m.Id = i.MakeId";

    sql += ";Select * from Customers;";

    SqlCommand myCommand = new SqlCommand(sql, connection);

    // Obtain a data reader a la ExecuteReader().
    using (SqlDataReader myDataReader = myCommand.ExecuteReader())
    {
        // Loop over the results.
        do
        {
            while (myDataReader.Read())
            {
                for (int i = 0; i < myDataReader.FieldCount; i++)
                {
                    Console.Write(i != myDataReader.FieldCount - 1
                    ? $"{myDataReader.GetName(i)} = {myDataReader.GetValue(i)}, "
                    : $"{myDataReader.GetName(i)} = {myDataReader.GetValue(i)} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        } while (myDataReader.NextResult());
    }
}
Console.ReadLine();
static void ShowConnectionStatus(SqlConnection connection)
{
    // Show various stats about current connection object.
    Console.WriteLine("***** Info about your connection *****");
    Console.WriteLine($@"Database location:
{connection.DataSource}");
    Console.WriteLine($"Database name: {connection.Database}");
    Console.WriteLine($@"Timeout:
{connection.ConnectionTimeout}");
    Console.WriteLine($"Connection state:{ connection.State}\n");
}