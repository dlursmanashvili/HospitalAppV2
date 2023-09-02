using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalApp.DBHelper
{
    public class PatientHelper
    {
        public List<PatientModel> LoadPatientsFromDatabase(string connectionString)
        {
            List<PatientModel> patients = new List<PatientModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID, FullName, Dob, GenderID, Phone, Address,IsDeleted FROM dbo.Patients";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PatientModel patient = new PatientModel
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            BirthDate = reader.GetDateTime(2),
                            GenderId = reader.GetInt32(3),
                            PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                            IsDeleted = reader.GetBoolean(6)
                        };
                        patients.Add(patient);
                    }
                }
            }
            return patients;
        }

        public PatientModel GetPatientByIdFromDatabase(string connectionString, int patientId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID, FullName, Dob, GenderID, Phone, Address,IsDeleted FROM dbo.Patients WHERE ID = @PatientId AND IsDeleted = 0";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientId", patientId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PatientModel patient = new PatientModel
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                BirthDate = reader.GetDateTime(2),
                                GenderId = reader.GetInt32(3),
                                PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Address = reader.IsDBNull(5) ? null : reader.GetString(5),
                                IsDeleted = reader.GetBoolean(6)

                            };
                            return patient;
                        }
                    }
                }
            }
            return null; // Patient with the given ID not found
        }

        public bool UpdatePatientInDatabase(string connectionString, PatientModel patient)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE dbo.Patients " +
                  "SET FullName = @FullName, Dob = @BirthDate, GenderID = @GenderId, Phone = @PhoneNumber, Address = @Address, IsDeleted = @IsDeleted " +
                  "WHERE ID = @PatientId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientId", patient.Id);
                    command.Parameters.AddWithValue("@FullName", patient.FullName);
                    command.Parameters.AddWithValue("@BirthDate", patient.BirthDate);
                    command.Parameters.AddWithValue("@GenderId", patient.GenderId);

                    if (patient.PhoneNumber != null)
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", patient.PhoneNumber);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", DBNull.Value);
                    }
                    if (patient.Address != null)
                    {
                        command.Parameters.AddWithValue("@Address", patient.Address);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Address", DBNull.Value);
                    }

                    command.Parameters.AddWithValue("@IsDeleted", patient.IsDeleted);

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; // Returns true if at least one row was updated
                }
            }
        }

        public bool AddPatientToDatabase(PatientModel patient, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO dbo.Patients (FullName, Dob, GenderID,Phone,Address, IsDeleted) VALUES (@FullName, @Dob, @GenderId, @Phone,@Address, @IsDeleted)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", patient.FullName);
                    command.Parameters.AddWithValue("@Dob", patient.BirthDate);
                    command.Parameters.AddWithValue("@GenderId", patient.GenderId);
                    if (patient.PhoneNumber != null)
                    {
                        command.Parameters.AddWithValue("@Phone", patient.PhoneNumber);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Phone", DBNull.Value);
                    }

                    if (patient.Address != null)
                    {
                        command.Parameters.AddWithValue("@Address", patient.Address);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Address", DBNull.Value);
                    }
                    command.Parameters.AddWithValue("@IsDeleted", patient.IsDeleted);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
