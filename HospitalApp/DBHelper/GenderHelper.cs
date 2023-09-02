using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalApp.DBHelper
{
    public class GenderHelper
    {
        public List<GenderModel> LoadGenderFromDatabase(string connectionString)
        {
            List<GenderModel> patients = new List<GenderModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT GenderID, GenderName FROM dbo.Gender";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GenderModel patient = new GenderModel
                        {
                            Id = reader.GetInt32(0),
                            GenderName = reader.GetString(1),
                           
                        };
                        patients.Add(patient);
                    }
                }
            }
            return patients;
        }
    }
}
