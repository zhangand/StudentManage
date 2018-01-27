﻿using System;
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

        public frmMain()
        {
            InitializeComponent();
        }

        // select->import->show dgv->show detail-update detail by selecting in dgv
        
        // toolbox event method
        private void btnImport_Click(object sender, EventArgs e)// import data
        {
            //select file
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "CSV File(*.csv)|*.csv|TXT File(*.txt)|*.txt|All Files(*.*)|*.txt";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                fileName = openfile.FileName;//send the filename to global Variable
            }
            try
            {
                //read file
                objListStudent = readFileToList(fileName);
            }
            catch (Exception ex)
            {

                MessageBox.Show("Read File Error"+ex.Message,"System Message",MessageBoxButtons.OK,MessageBoxIcon.Information);

            }
        }


        //user define method
        private List<string> readFileToList(string filepath)// read file return student list
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


    }
}
