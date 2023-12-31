﻿using System;
using System.Collections.Generic;
using System.Data;
using AutoLot.Dal.Models;
using Microsoft.Data.SqlClient;

namespace AutoLot.Dal.DataOperations
{
    public class InventoryDal : IDisposable
    {
        private readonly string _connectionString;
        bool _disposed = false;

        public InventoryDal() : this(@"Data Source=(localdb)\mssqllocaldb;Integrated Security=True;Initial Catalog=AutoLot")
        {
        }

        public InventoryDal(string connectionString) => _connectionString = connectionString;

        private SqlConnection? _sqlConnection = null;

        private void OpenConnection()
        {
            _sqlConnection = new SqlConnection
            {
                ConnectionString = _connectionString
            };
            _sqlConnection.Open();
        }

        private void CloseConnection()
        {
            if (_sqlConnection?.State != ConnectionState.Closed)
            {
                _sqlConnection?.Close();
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _sqlConnection?.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public List<CarViewModel> GetAllInventory()
        {
            OpenConnection();

            // This will hold the records.
            List<CarViewModel> inventory = new List<CarViewModel>();

            // Prep command object.
            string sql =
            @"SELECT i.Id, i.Color, i.PetName,m.Name as Make
            FROM Inventory i
            INNER JOIN Makes m on m.Id = i.MakeId";

            using SqlCommand command = new(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };

            command.CommandType = CommandType.Text;
            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                inventory.Add(new CarViewModel
                {
                    Id = (int)dataReader["Id"],
                    Color = (string)dataReader["Color"],
                    Make = (string)dataReader["Make"],
                    PetName = (string)dataReader["PetName"]
                });
            }
            dataReader.Close();
            return inventory;
        }

        public CarViewModel GetCar(int id)
        {
            OpenConnection();
            CarViewModel car = null;

            SqlParameter param = new SqlParameter
            {
                ParameterName = "@carId",
                Value = id,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            //This should use parameters for security reasons
            string sql =
            $@"SELECT i.Id, i.Color, i.PetName,m.Name as Make
            FROM Inventory i
            INNER JOIN Makes m on m.Id = i.MakeId
            WHERE i.Id = @carId";

            using SqlCommand command = new SqlCommand(sql, _sqlConnection)
            {
                CommandType = CommandType.Text
            };

            command.Parameters.Add(param);

            SqlDataReader dataReader = command.ExecuteReader(CommandBehavior.CloseConnection);

            while (dataReader.Read())
            {
                car = new CarViewModel
                {
                    Id = (int)dataReader["Id"],
                    Color = (string)dataReader["Color"],
                    Make = (string)dataReader["Make"],
                    PetName = (string)dataReader["PetName"]
                };
            }
            dataReader.Close();
            return car;
        }

        public void InsertAuto(string color, int makeId, string petName)
        {
            OpenConnection();

            // Format and execute SQL statement.
            string sql = $"Insert Into Inventory (MakeId, Color, PetName) Values ('{makeId}','{color}', '{petName}')";

            // Execute using our connection.
            using (SqlCommand command = new(sql, _sqlConnection))
            {
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        public void InsertAuto(Car car)
        {
            OpenConnection();

            // Note the "placeholders" in the SQL query.
            string sql = "Insert Into Inventory" +
            "(MakeId, Color, PetName) Values" +
            "(@MakeId, @Color, @PetName)";

            // This command will have internal parameters.
            using (SqlCommand command = new SqlCommand(sql, _sqlConnection))
            {
                // Fill params collection.
                SqlParameter parameter = new SqlParameter
                {
                    ParameterName = "@MakeId",
                    Value = car.MakeId,
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input
                };

                command.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@Color",
                    Value = car.Color,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 50,
                    Direction = ParameterDirection.Input
                };

                command.Parameters.Add(parameter);

                parameter = new SqlParameter
                {
                    ParameterName = "@PetName",
                    Value = car.PetName,
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 50,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();
                CloseConnection();
            }
        }

        public void DeleteCar(int id)
        {
            OpenConnection();

            // Get ID of car to delete, then do so.
            string sql = $"Delete from Inventory where Id = @carId";

            SqlParameter param = new SqlParameter
            {
                ParameterName = "@carId",
                Value = id,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };

            using (SqlCommand command = new(sql, _sqlConnection))
            {
                try
                {
                    command.Parameters.Add(param);
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    Exception error = new Exception("Sorry! That car is on order!", ex);
                    throw error;
                }
            }
            CloseConnection();
        }

        public void UpdateCarPetName(int id, string newPetName)
        {
            OpenConnection();

            SqlParameter paramId = new SqlParameter
            {
                ParameterName = "@carId",
                Value = id,
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Input
            };
            SqlParameter paramName = new SqlParameter
            {
                ParameterName = "@petName",
                Value = newPetName,
                SqlDbType = SqlDbType.NVarChar,
                Size = 50,
                Direction = ParameterDirection.Input
            };

            // Get ID of car to modify the pet name.
            string sql = $"Update Inventory Set PetName = @petName Where Id = @carId";

            using (SqlCommand command = new(sql, _sqlConnection))
            {
                command.Parameters.Add(paramId);
                command.Parameters.Add(paramName);
                command.ExecuteNonQuery();
            }
            CloseConnection();
        }

        public string LookUpPetName(int carId)
        {
            OpenConnection();
            string carPetName;

            // Establish name of stored proc.
            using (SqlCommand command = new ("GetPetName", _sqlConnection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Input param.
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@carId",
                    SqlDbType = SqlDbType.Int,
                    Value = carId,
                    Direction = ParameterDirection.Input
                };
                command.Parameters.Add(param);

                // Output param.
                param = new SqlParameter
                {
                    ParameterName = "@petName",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 50,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(param);

                // Execute the stored proc.
                command.ExecuteNonQuery();

                // Return output param.
                carPetName = (string)command.Parameters["@petName"].Value;
                CloseConnection();
            }
            return carPetName;
        }
    }
}
