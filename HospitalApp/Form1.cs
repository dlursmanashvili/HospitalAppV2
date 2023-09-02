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

        public Form1()
        {
            InitializeComponent();
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.AllowUserToAddRows = false;
            LoadData();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AddEditForm addEditForm = new AddEditForm())
            {
                addEditForm.DataUpdated += () =>
                {
                    dataGridView1.Rows.Clear();
                    LoadData();
                    Refresh();
                };
                addEditForm.changeButttonText("დამატება");
                addEditForm.ShowDialog();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {        
            int lastIndex = dataGridView1.SelectedRows.Count;
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var DialogResult = MessageBox.Show(" ნამდვილად გსურთ აღნიშნულის განახლება  ? ", "confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (DialogResult.Yes == DialogResult)
                {

                    DataGridViewRow lastRow = dataGridView1.SelectedRows[0];

                    if (lastRow.Cells["ID"].Value == null || int.TryParse(lastRow.Cells["ID"].Value.ToString(), out int PatientID) == false)
                    {
                        MessageBox.Show($"აღნიშნულის განახლება შეუძლებელია");
                        return;
                    }                   

                    PatientHelper patientHelper = new PatientHelper();
                    var patient = patientHelper.GetPatientByIdFromDatabase(DbHelperClass.GetConnectioinString(), PatientID);
                  
                    if (patient == null && patient.IsDeleted == true)
                    {
                        MessageBox.Show($"პაციენტი იდენტიფიკატორით {PatientID} ვერ მოიძებნა");
                        return;                       
                    }
                    else
                    {
                        using (AddEditForm addEditForm = new AddEditForm( patient))
                        {
                            addEditForm.DataUpdated += () =>
                            {
                                dataGridView1.Rows.Clear();
                                LoadData();
                                Refresh();
                            };
                            addEditForm.changeButttonText("ედიტირება");
                            addEditForm.ShowDialog();
                        }                       
                    }
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                var DialogResult = MessageBox.Show("გსურთ მონიშნული ჩანაწერის წაშლა?", "confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (DialogResult == DialogResult.Yes)
                {
                    if (selectedRow.Cells["ID"].Value == null || !int.TryParse(selectedRow.Cells["ID"].Value.ToString(), out int PatientID))
                    {
                        MessageBox.Show($"აღნიშნულის წაშლა შეუძლებელია");
                        return;
                    }

                    PatientHelper patientHelper = new PatientHelper();
                    var patient = patientHelper.GetPatientByIdFromDatabase(DbHelperClass.GetConnectioinString(), PatientID);

                    if (patient != null && !patient.IsDeleted)
                    {
                        patient.IsDeleted = true;
                        patientHelper.UpdatePatientInDatabase(DbHelperClass.GetConnectioinString(), patient);
                        dataGridView1.Rows.Remove(selectedRow); 
                    }
                    else
                    {
                        MessageBox.Show($"პაციენტი იდენტიფიკატორით {PatientID} ვერ მოიძებნა");
                    }
                }
            }
        }

        public void LoadData()
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
    }
}
