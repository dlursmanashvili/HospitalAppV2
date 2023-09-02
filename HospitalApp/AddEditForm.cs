using HospitalApp.DBHelper;
using System;

using System.Windows.Forms;

namespace HospitalApp
{
    public partial class AddEditForm : Form
    {
        PatientHelper patientHelper = new PatientHelper();

        public event Action DataUpdated;
        public PatientModel _patient { get; set; }

        public AddEditForm(PatientModel paatient)
        {
            _patient = paatient;
            InitializeComponent();

            textBoxPhpneNumber.Text = _patient.PhoneNumber.ToString();
            textBoxAddress.Text = _patient.Address.ToString();
            textBoxFullName.Text = _patient.FullName.ToString();
            dateTimePicker.Text = _patient.BirthDate.ToString();
            if (_patient.GenderId == 1)
            {
                comboBoxGender.Text = "მამრობითი";
            }
            else
            {
                comboBoxGender.Text = "მდედრობითი";
            }
        }

        public AddEditForm()
        {
            InitializeComponent();
        }


        private void button1_Add(object sender, EventArgs e)
        {
            var phoneNumberString = textBoxPhpneNumber.Text.ToString();
            var address = textBoxAddress.Text.ToString();
            var Gender = comboBoxGender.Text.ToString();
            var fullName = textBoxFullName.Text.ToString();
            var birthDate = dateTimePicker.Text.ToString();

            #region  validation
            if (birthDate == null || DateTime.TryParse(birthDate, out DateTime Dob) == false)
            {
                MessageBox.Show($"დაბადების თარიღი არ არის შეყვანილი ან არასწორად არის შეყვანილი");
                return;
            }
            else if (Gender == null)
            {
                MessageBox.Show($"გთხოვთ არიჩიოთ სქესი");
                return;
            }
            else if (fullName == null || fullName == " ")
            {
                MessageBox.Show($"სახელი და გვარი არ არის შეყვანილი ან არასწორად არის შეყვანილი");
                return;
            }
            else if (phoneNumberString == null || (int.TryParse(phoneNumberString, out int phonenumber) == false || phoneNumberString.Length != 9 || phoneNumberString[0] != '5'))
            {
                MessageBox.Show("ტელეფონის ველი არავალიდურია");
                return;
            }
            PatientModel newPatient = new PatientModel
            {
                FullName = fullName,
                BirthDate = Dob.Date,
                PhoneNumber = phoneNumberString,
                Address = address,
            };
            if (Gender == "მამრობითი")
            {
                newPatient.GenderId = 1;
            }
            else if (Gender == "მდედრობითი")
            {
                newPatient.GenderId = 2;
            }
            else
            {               
                MessageBox.Show("სქესის ველი არავალიდურია");
                return;
            }
            #endregion

            if (button1.Text == "დამატება")
            {
                newPatient.IsDeleted = false;
                if (patientHelper.AddPatientToDatabase(newPatient, DbHelperClass.GetConnectioinString()))
                {
                    DataUpdated?.Invoke();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("მომხმარებელი ვერ დაემატდა");
                    return;
                }
            }
            else
            {
                newPatient.Id = _patient.Id;
                newPatient.IsDeleted = _patient.IsDeleted;
                if (patientHelper.UpdatePatientInDatabase(DbHelperClass.GetConnectioinString(), newPatient))
                {
                    DataUpdated?.Invoke();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("მომხმარებელი ვერ დაედიტდა");
                    return;
                }

            }
        }

        public void changeButttonText(string text)
        {
            button1.Text = text;
        }
    }
}
