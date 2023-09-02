using HospitalApp.DBHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HospitalApp
{
    public partial class Form1 : Form
    {
        private DateTimePicker dateTimePicker;

        public Form1()
        {
            InitializeComponent();
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AllowUserToAddRows = false;
            LoadData();
            dataGridView1.Rows.Add();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dataGridView1.Rows.Count > 0)
            {
                int lastIndex = dataGridView1.Rows.Count - 1;
                DataGridViewRow lastRow = dataGridView1.Rows[lastIndex];

                var phoneNumberString = lastRow.Cells["PhoneNumber"].Value?.ToString();

                if (lastRow.Cells["BirthDate"].Value == null || DateTime.TryParse(lastRow.Cells["BirthDate"].Value.ToString(), out DateTime birthDate) == false)
                {
                    MessageBox.Show($"დაბადების თარიღი არ არის შეყვანილი ან არასწორად არის შეყვანილი");
                    return;
                }
                else if (lastRow.Cells["Gender"].Value == null)
                {
                    MessageBox.Show($"გთხოვთ არიჩიოთ სქესი");
                    return;
                }
                else if (lastRow.Cells["PatientNameAndSurname"].Value == null || lastRow.Cells["PatientNameAndSurname"].Value.ToString() == " ")
                {
                    MessageBox.Show($"სახელი და გვარი არ არის შეყვანილი ან არასწორად არის შეყვანილი");
                    return;
                }
                else if (phoneNumberString != null &&( int.TryParse(phoneNumberString, out int phonenumber) == false ||
        phoneNumberString.Length != 9 ||
        phoneNumberString[0] != '5'))
                {
                    MessageBox.Show("ტელეფონის ველი არავალიდურია");
                    return;
                }

                PatientHelper patientHelper = new PatientHelper();
                PatientModel newPatient = new PatientModel
                {
                    FullName = lastRow.Cells["PatientNameAndSurname"].Value.ToString(),
                    BirthDate = Convert.ToDateTime(lastRow.Cells["BirthDate"].Value),
                };

                if (lastRow.Cells["Gender"].Value.ToString() == "მამრობითი")
                {
                    newPatient.GenderId = 1;
                }
                else if (lastRow.Cells["Gender"].Value.ToString() == "მდედრობითი")
                {
                    newPatient.GenderId = 2;
                }
                newPatient.PhoneNumber = lastRow.Cells["PhoneNumber"].Value?.ToString();
                newPatient.Address = lastRow.Cells["PhysicalAddres"].Value?.ToString();

                bool success = patientHelper.AddPatientToDatabase(newPatient, DbHelperClass.GetConnectioinString());
                if (success)
                {
                    dataGridView1.Rows.Clear();
                    LoadData();
                    dataGridView1.Rows.Add();
                }
            }
        }



        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int lastIndex = dataGridView1.SelectedRows.Count;
            if (dataGridView1.Rows.Count > 0 && lastIndex > 0)
            {
                var DialogResult = MessageBox.Show(" ნამდვილად გსურთ აღნიშნულის განახლება  ? ", "confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (DialogResult.Yes == DialogResult)
                {

                    DataGridViewRow lastRow = dataGridView1.Rows[lastIndex -1];
                    var phoneNumberString = lastRow.Cells["PhoneNumber"].Value?.ToString();

                    if (lastRow.Cells["ID"].Value == null || int.TryParse(lastRow.Cells["ID"].Value.ToString(), out int PatientID) == false)
                    {
                        MessageBox.Show($"აღნიშნულის განახლება შეუძლებელია");
                        return;
                    }
                    if (lastRow.Cells["BirthDate"].Value == null || DateTime.TryParse(lastRow.Cells["BirthDate"].Value.ToString(), out DateTime birthDate) == false)
                    {
                        MessageBox.Show($"დაბადების თარიღი არ არის შეყვანილი ან არასწორად არის შეყვანილი");
                        return;
                    }
                    else if (lastRow.Cells["Gender"].Value == null)
                    {
                        MessageBox.Show($"გთხოვთ არიჩიოთ სქესი");
                        return;
                    }
                    else if (lastRow.Cells["PatientNameAndSurname"].Value == null || lastRow.Cells["PatientNameAndSurname"].Value.ToString() == " ")
                    {
                        MessageBox.Show($"სახელი და გვარი არ არის შეყვანილი ან არასწორად არის შეყვანილი");
                        return;
                    }
                    else if (phoneNumberString != null && (int.TryParse(phoneNumberString, out int phonenumber) == false ||
                            phoneNumberString.Length != 9 ||
                            phoneNumberString[0] != '5'))
                    {
                        MessageBox.Show("ტელეფონის ველი არავალიდურია");
                        return;
                    }

                    PatientHelper patientHelper = new PatientHelper();
                    var patient = patientHelper.GetPatientByIdFromDatabase(DbHelperClass.GetConnectioinString(), PatientID);

                    if (patient != null && patient.IsDeleted == false)
                    {
                        int? Genderid = null;
                        if (lastRow.Cells["Gender"].Value.ToString() == "მამრობითი")
                        {
                            Genderid = 1;
                        }
                        else if (lastRow.Cells["Gender"].Value.ToString() == "მდედრობითი")
                        {
                            Genderid = 2;
                        }
                        patientHelper.UpdatePatientInDatabase(DbHelperClass.GetConnectioinString(), new PatientModel()
                        {
                            Id = PatientID,
                            IsDeleted = patient.IsDeleted,
                            Address = lastRow.Cells["PhysicalAddres"].Value?.ToString(),
                            BirthDate = birthDate,
                            FullName = lastRow.Cells["PatientNameAndSurname"].Value.ToString(),
                            GenderId = (int)Genderid,
                            PhoneNumber = lastRow.Cells["PhoneNumber"].Value?.ToString()
                        });
                        dataGridView1.Rows.Clear();
                        LoadData();
                        dataGridView1.Rows.Add();
                    }
                    else
                    {
                        MessageBox.Show($"პაციენტი იდენტიფიკატორით {PatientID} ვერ მოიძებნა");
                        return;
                    }
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int lastIndex = dataGridView1.SelectedRows.Count;
            if (dataGridView1.Rows.Count > 0 && lastIndex > 0)
            {
                DataGridViewRow lastRow = dataGridView1.Rows[lastIndex - 1];

                var DialogResult = MessageBox.Show(" ნამდვილად გსურთ წაშლა ? ", "confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (DialogResult.Yes == DialogResult)
                {
                    if (lastRow.Cells["ID"].Value == null || int.TryParse(lastRow.Cells["ID"].Value.ToString(), out int PatientID) == false)
                    {
                        MessageBox.Show($"აღნიშნულის წაშლა შეუძლებელია");
                        return;
                    }

                    PatientHelper patientHelper = new PatientHelper();
                    var patient = patientHelper.GetPatientByIdFromDatabase(DbHelperClass.GetConnectioinString(), PatientID);

                    if (patient != null && patient.IsDeleted == false)
                    {
                        patient.IsDeleted = true;
                        patientHelper.UpdatePatientInDatabase(DbHelperClass.GetConnectioinString(), patient);
                        dataGridView1.Rows.Clear();
                        LoadData();
                        dataGridView1.Rows.Add();
                    }
                    else
                    {
                        MessageBox.Show($"პაციენტი იდენტიფიკატორით {PatientID} ვერ მოიძებნა");
                        return;
                    }

                }
            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && dataGridView1.Rows.Count != 0)
            {
                dateTimePicker = new DateTimePicker();
                dataGridView1.Controls.Add(dateTimePicker);
                dateTimePicker.Format = DateTimePickerFormat.Short;
                Rectangle rectangle = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                dateTimePicker.Size = new Size(rectangle.Width, rectangle.Height);
                dateTimePicker.Location = new Point(rectangle.X, rectangle.Y + rectangle.Height);
                dateTimePicker.CloseUp += new EventHandler(dateTimePicker_closeup);
                dateTimePicker.TextChanged += new EventHandler(dateTimePicker_textchanged);
                dateTimePicker.Visible = true;
            }
        }


        private void dateTimePicker_textchanged(object sender, EventArgs e)
        {
            dataGridView1.CurrentCell.Value = dateTimePicker.Text.ToString();

        }
        private void dateTimePicker_closeup(object sender, EventArgs e)
        {
            dateTimePicker.Visible = false;
        }








        private void LoadData()
        {
            dataGridView1.AllowUserToAddRows = true;
            var genderheler = new GenderHelper();
            var genders = genderheler.LoadGenderFromDatabase(DbHelperClass.GetConnectioinString());

            PatientHelper patientHelper = new PatientHelper();
            var patientInfo = patientHelper.LoadPatientsFromDatabase(DbHelperClass.GetConnectioinString());
            foreach (var patient in patientInfo)
            {
                if (patient.IsDeleted == false)
                {
                    dataGridView1.Rows.Add(patient.Id, patient.FullName, patient.BirthDate.Date.ToString("dd/MM/yyyy"), GetGenderName(patient.GenderId, genders), patient.PhoneNumber, patient.Address);
                }
            }
            dataGridView1.AllowUserToAddRows = false;
        }

        private string GetGenderName(int id, List<GenderModel> genders)
        {
            if (genders.First(x => x.Id == id).GenderName == "მამრობითი")
            {
                return "მამრობითი";
            }
            return "მდედრობითი";
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != 2)
            {
                dataGridView1.Controls.Remove(dateTimePicker);
            }

        }

        private void menuStrip1_MouseClick(object sender, MouseEventArgs e)
        {
            dataGridView1.Controls.Remove(dateTimePicker);
        }
    }
}
