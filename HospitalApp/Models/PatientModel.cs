using System;

namespace HospitalApp
{
    public class PatientModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int GenderId { get; set; }
        public string PhoneNumber { get; set; } = null;
        public string Address { get; set; } = null;
        public bool IsDeleted { get; set; } = false;

    }
}
