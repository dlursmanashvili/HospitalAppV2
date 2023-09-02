using System;
using System.Data.SqlClient;

namespace HospitalApp.DBHelper
{
    public static class DbHelperClass
    {
        private static string serverName = "."; // Имя сервера (замените на свое)
        private static string databaseName = "HospitalDB"; // Имя базы данных

        public static void Checkdb()
        {
            string connectionString = $"Server={serverName};Integrated Security=SSPI;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT COUNT(*) FROM sys.databases WHERE name = @DatabaseName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DatabaseName", databaseName);
                        int count = (int)command.ExecuteScalar();

                        if (count == 0)
                        {
                            string createDatabaseQuery = $"CREATE DATABASE {databaseName}";
                            using (SqlCommand comm = new SqlCommand(createDatabaseQuery, connection))
                            {
                                comm.ExecuteNonQuery();
                                Console.WriteLine($"База данных {databaseName} успешно создана.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        public static void CheckTables()
        {
            string connectionString = $"Server={serverName};Database={databaseName};Integrated Security=SSPI;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string checkPatientsTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Patients'";
                    using (SqlCommand command = new SqlCommand(checkPatientsTableQuery, connection))
                    {
                        int patientsTableCount = (int)command.ExecuteScalar();

                        if (patientsTableCount == 0)
                        {
                            string createPatientsTableQuery = @"
                                 CREATE TABLE dbo.Patients (
                                 ID INT IDENTITY,
                                 FullName NVARCHAR(200) NOT NULL,
                                 Dob DATE NOT NULL,
                                 GenderID INT NOT NULL,
                                 Phone VARCHAR(50) NULL,
                                 Address NVARCHAR(500) NULL,
                                 IsDeleted BIT NOT NULL DEFAULT 0)";
                            using (SqlCommand createCommand = new SqlCommand(createPatientsTableQuery, connection))
                            {
                                createCommand.ExecuteNonQuery();
                                Console.WriteLine("Таблица Patients успешно создана.");
                            }
                        }
                    }

                    string checkGenderTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Gender'";
                    using (SqlCommand command = new SqlCommand(checkGenderTableQuery, connection))
                    {
                        int genderTableCount = (int)command.ExecuteScalar();

                        if (genderTableCount == 0)
                        {
                            string createGenderTableQuery = "CREATE TABLE dbo.Gender (GenderID INT IDENTITY, GenderName NVARCHAR(30) NOT NULL)";
                            using (SqlCommand createCommand = new SqlCommand(createGenderTableQuery, connection))
                            {
                                createCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    CheckGender(connection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error: {ex.Message}");
                }
            }
        }

        private static void CheckGender(SqlConnection connection)
        {
            string checkMaleQuery = "SELECT COUNT(*) FROM dbo.Gender WHERE GenderName = 'Male'";
            using (SqlCommand command = new SqlCommand(checkMaleQuery, connection))
            {
                int maleCount = (int)command.ExecuteScalar();

                if (maleCount == 0)
                {
                    string insertMaleQuery = "INSERT INTO dbo.Gender (GenderName) VALUES ('Male')";
                    using (SqlCommand insertCommand = new SqlCommand(insertMaleQuery, connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
            string checkFemaleQuery = "SELECT COUNT(*) FROM dbo.Gender WHERE GenderName = 'Famle'";
            using (SqlCommand femaleCommand = new SqlCommand(checkFemaleQuery, connection))
            {
                int femaleCount = (int)femaleCommand.ExecuteScalar();

                if (femaleCount == 0)
                {
                    string insertFemaleQuery = "INSERT INTO dbo.Gender (GenderName) VALUES ('Famle')";
                    using (SqlCommand insertCommand = new SqlCommand(insertFemaleQuery, connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public static string GetConnectioinString() => $"Server={serverName};Database={databaseName};Integrated Security=SSPI;";

    }
}


