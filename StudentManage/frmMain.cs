using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManage
{
    public partial class frmMain : Form
    {
        private string fileName=string.Empty;
        private List<string> objListStudent = new List<string>(); //define student info variable
        private List<string> objQueryList = new List<string>();//actionflag=1 add, =2 update
        private int ActionFlag = 0;
        private string photoName = string.Empty;

        public frmMain()
        {
            InitializeComponent();
            DisableButton();
        }

        // select->import->show dgv->show detail-update detail by selecting in dgv
        
        // toolbox event method
        private void btnImport_Click(object sender, EventArgs e)// import data
        {
            //1.select file
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "CSV File(*.csv)|*.csv|TXT File(*.txt)|*.txt|All Files(*.*)|*.txt";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                fileName = openfile.FileName;//send the filename to global Variable
            }
            else return;
            //2.record the student data to list
            try
            {
                //read file
                objListStudent.Clear();
                dgvStudent.Rows.Clear();
                objListStudent = ReadFileToList(fileName);
                
            }
            catch (Exception ex)
            {

                MessageBox.Show("Read File Error, "+ex.Message,"System Message",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            //3.show student info in datagridview
            LoadDataToDataGrid(objListStudent);

            //4. show student detail info
            string currentSNO = dgvStudent.Rows[0].Cells[0].Value.ToString();
            string detailStudent = GetStudentBySNO(currentSNO);
            LoadDataToDetail(detailStudent);

        }
        private void dgvStudent_SelectionChanged(object sender, EventArgs e)//update selection info to detail
        {
            if (dgvStudent.RowCount == 0) return;
            if (dgvStudent.CurrentRow.Selected == false) return;
            else
            {
                string currentSNO = dgvStudent.CurrentRow.Cells[0].Value.ToString();
                string detailStudent = GetStudentBySNO(currentSNO);
                LoadDataToDetail(detailStudent);
            }
        }
        private void txtQuerySNO_TextChanged(object sender, EventArgs e)//Query by sno when value changed
        {
            if (txtQuerySNO.Text != "")
            {
                QueryStudentByStartString(txtQuerySNO.Text);
                LoadDataToDataGrid(objQueryList);
            }
            else LoadDataToDataGrid(objListStudent); 

        }
        private void txtQueryName_TextChanged(object sender, EventArgs e)//Query by name when value changed
        {
            if (txtQueryName.Text != "")
            {
                QueryStudentByContainString(txtQueryName.Text);
                LoadDataToDataGrid(objQueryList);
            }
            else LoadDataToDataGrid(objListStudent); ;
        }
        private void txtQueryMobile_TextChanged(object sender, EventArgs e)//Query by mobile when value changed
        {
            if (txtQueryMobile.Text != "")
            {
                QueryStudentByContainString(txtQueryMobile.Text);
                LoadDataToDataGrid(objQueryList);
            }
            else LoadDataToDataGrid(objListStudent); 
        }

        //enable gboxstudentdetail->enter info->check info->commit info->write to file
        private void btnAdd_Click(object sender, EventArgs e)//add student info
        {
            EnableButton();
            //clear datagridview
            LoadDataToDetail("");
            //focus on enter SNO
            txtSNO.Focus();
            ActionFlag = 1;
        }
        private void btnUpdate_Click(object sender, EventArgs e)//update student info
        {
            if (dgvStudent.Rows.Count >0 )
            {
                //disbale button
                EnableButton();
                btnChoosePath.Visible = true;
                txtSNO.Enabled = false;

                //focus sno
                txtName.Focus();

                //modify action flag
                ActionFlag = 2;
            }
            else
            {
                MessageBox.Show("Must Have One Student Info", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
        }
        private void btnDelete_Click(object sender, EventArgs e)//delete student info
        {
            if (dgvStudent.Rows.Count>0)
            {
                //comfirm delete message
                DialogResult result = MessageBox.Show("Comfirm Delete Student No " + txtSNO.Text + " Information",
                                                       "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //delete
                    for (int i = 0; i < objListStudent.Count; i++)
                    {
                        if (objListStudent[i].StartsWith(txtSNO.Text) == true)
                        {
                            objListStudent.RemoveAt(i);
                            LoadDataToDataGrid(objListStudent);
                            MessageBox.Show("Student Info Delete Successful", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Must Select One Row", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

        }
        private void btnCommit_Click(object sender, EventArgs e)//commit student info
        {
            string photoPath = DateTime.Now.ToString("yyyyMMddHHmmss");
            Random objRandom = new Random();
            photoPath += objRandom.Next(0,100).ToString("00");
            photoPath += photoName.Substring(photoName.Length - 4);
            photoPath = ".\\image\\" + photoPath;

            string sGender = string.Empty;
            if (rbMale.Checked == true)
            {
                sGender = "male";
            }
            else sGender = "female";

            string StudentInfo = txtSNO.Text.Trim() + "," + txtName.Text.Trim() + "," + sGender + ","
                                + dtpBirthday.Value.ToShortDateString() + "," + txtMobile.Text.Trim() + ","
                                + txtEmail.Text.Trim() + "," + txtHomeAddress.Text.Trim() + ","+
                                photoPath;
            //SNO and name cannot be blank, sno cannot be duplicated
            if (ValidationStudentInfo(txtSNO.Text.Trim(), txtName.Text.Trim()))
            {
                // store picture
                Bitmap objBitmap = new Bitmap(pbCurrentPhoto.BackgroundImage);
                objBitmap.Save(photoPath,pbCurrentPhoto.BackgroundImage.RawFormat);
                objBitmap.Dispose();

                switch (ActionFlag)
                {
                    case 1:
                        objListStudent.Add(StudentInfo);
                        LoadDataToDataGrid(objListStudent);
                        DisableButton();
                        MessageBox.Show("Student Info Add Successful", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case 2:
                        for (int i = 0; i < objListStudent.Count; i++)
                        {
                            if (objListStudent[i].StartsWith(txtSNO.Text) == true)
                            {
                                objListStudent.RemoveAt(i);
                                objListStudent.Insert(i, StudentInfo);
                                LoadDataToDataGrid(objListStudent);
                                DisableButton();
                                MessageBox.Show("Student Info Update Successful", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }
                        }
                        txtSNO.Enabled = true;
                        break;
                    default:
                        break;
                }
            }
            else return;
        }
        private void btnCancel_Click(object sender, EventArgs e)//cancel modify
        {
            if (dgvStudent.RowCount!=0)
            {
                string currentSNO = dgvStudent.CurrentRow.Cells[0].Value.ToString();
                string detailStudent = GetStudentBySNO(currentSNO);
                LoadDataToDetail(detailStudent);
            }
            DisableButton();
        }
        private void btnChoosePath_Click(object sender, EventArgs e)//choose picture path
        {
            //1.choose picture, dispaly
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|" + "All files (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                photoName = openFile.FileName;
                pbCurrentPhoto.BackgroundImage = Image.FromFile(photoName);//display selected pic
            }
            else return;

        }
        private void btnClose_Click(object sender, EventArgs e)//save list to file
        {
            //comfirm exit message
            DialogResult result = MessageBox.Show("Comfirm Save Student Information" ,
                                                   "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (objListStudent.Count>0)
                {
                    //write objliststudent to file for each file
                    File.WriteAllText(fileName, string.Empty);
                    StreamWriter sw = new StreamWriter(fileName, true, Encoding.Default);
                    foreach (string item in objListStudent)
                    {
                        sw.WriteLine(item);
                    }
                    sw.Close();
                    MessageBox.Show("Student Information Saved", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
   
            }
            Close();
        }

        //user define method
        private List<string> ReadFileToList(string filepath)// read file return student list
        {
            List<string> objList = new List<string>();
            string line = string.Empty;
            try
            {
                StreamReader file= new StreamReader(filepath,Encoding.Default);
                line = file.ReadLine();
                while (line!= null)
                {
                    objList.Add(line);
                    line = file.ReadLine();
                }
                file.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return objList;
        }
        private void LoadDataToDataGrid(List<string> objlist)//load student string to datagridview
        {
            dgvStudent.Rows.Clear();
            foreach (string item in objlist)
            {
                int Length = 5;
                string[] studentArray = item.Split(',');
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvStudent);
                if (studentArray.Length > 5) Length = 5;
                else Length = studentArray.Length;
                for (int i = 0; i < Length; i++)
                {
                    row.Cells[i].Value = studentArray[i];
                }
                dgvStudent.Rows.Add(row);
            }
        }
        private void LoadDataToDetail(string currentStudent)//load student detail in group box
        {
            if (currentStudent != string.Empty)
            {
                string[] studentArray = currentStudent.Split(',');
                txtSNO.Text = studentArray[0];
                txtName.Text = studentArray[1];
                dtpBirthday.Text = studentArray[3];
                if (studentArray[2] == "male")
                {
                    rbMale.Checked = true;

                }
                else if (studentArray[2] == "female")
                {
                    rbFemale.Checked = true;
                }
                if (studentArray.Length >=7)
                {
                    txtMobile.Text = studentArray[4];
                    txtEmail.Text = studentArray[5];
                    txtHomeAddress.Text = studentArray[6];
                    try
                    {
                        btnChoosePath.Visible = false;
                        pbCurrentPhoto.BackgroundImage = Image.FromFile(studentArray[7]);
                    }
                    catch (Exception)
                    {
                        pbCurrentPhoto.BackgroundImage = null;
                        btnChoosePath.Visible = true;
                        return;
                    }
                        
                }
            }
            else
            {
                txtSNO.Text = string.Empty;
                txtName.Text = string.Empty;
                dtpBirthday.Text = DateTime.Now.ToString();
                rbMale.Checked = false;
                rbFemale.Checked = false;
                txtMobile.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtHomeAddress.Text = string.Empty;
                pbCurrentPhoto.BackgroundImage = null;
            }

        }
        private string GetStudentBySNO(string SNO)//Query student info by SNO
        {
            string currentStudent = string.Empty;
            foreach (string item in objListStudent)
            {
                if (item.StartsWith(SNO))
                {
                    currentStudent = item;
                    break;
                }
            }
            return currentStudent;

        }
        private List<string> QueryStudentByStartString(string StartString)//Query student info by Starting string
        {
            objQueryList.Clear();
            foreach (string item in objListStudent)
            {
                if (item.StartsWith(StartString))
                {
                    objQueryList.Add(item);
                }
            }
            return objQueryList;
        }
        private List<string> QueryStudentByContainString(string ContainString)//Query student info by Containing string
        {
            objQueryList.Clear();
            foreach (string item in objListStudent)
            {
                if (item.Contains(ContainString))
                {
                    objQueryList.Add(item);
                }
            }
            return objQueryList;
        }
        private void DisableButton()//disable gbox student detail info and enable import add update delete button
        {
            gboxStudentDetail.Enabled = false;
            btnImport.Enabled = true;
            btnAdd.Enabled = true;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
        }
        private void EnableButton()//enable gbox student detail info and disable import add update delete button
        {
            gboxStudentDetail.Enabled = true;
            btnImport.Enabled = false;
            btnAdd.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }
        private Boolean ValidationStudentInfo(string sno, string sname)
        {
            if (string.IsNullOrWhiteSpace(sno) || string.IsNullOrWhiteSpace(sname))
            {
                MessageBox.Show("Student No or Name cannot be blank", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else if (ActionFlag==1)
            {
                if (!string.IsNullOrWhiteSpace(GetStudentBySNO(sno)))
                {
                    MessageBox.Show("Student No is existed", "System Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                else return true;
            }
            else return true;
        }
    }
}
