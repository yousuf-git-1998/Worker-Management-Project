using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkerManagement
{
    public partial class FormAddProject : Form
    {
        List<Worker> workers = new List<Worker>();
        string currentFile = "";
        public FormAddProject()
        {
            InitializeComponent();
        }
        public Form1 TheForm { get; set; }
        private void button2_Click(object sender, EventArgs e)
        {
            Worker w = new Worker { Name = textBox1.Text, Phone = textBox2.Text, Skill = textBox3.Text, PayRate = numericUpDown1.Value, PictureFullPath = currentFile, Picture = Path.GetFileName(currentFile) };
            w.Image = File.ReadAllBytes(currentFile);
            workers.Add(w);
            currentFile = "";
            label5.Text = "";
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            numericUpDown1.Value = 0;
            BindDataToGrid();
        }

        private void BindDataToGrid()
        {
            this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = workers;
        }

        private void FormAddProject_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            BindDataToGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                label5.Text = Path.GetFileName(currentFile);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    string sql = "INSERT INTO projects (projectname, startdate, iscompleted) VALUES(@n, @s, @i); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand cmd = new SqlCommand(sql, con, trx))
                    {
                        cmd.Parameters.AddWithValue("@n", textBox4.Text);
                        cmd.Parameters.AddWithValue("@s", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@i", checkBox1.Checked);
                        try
                        {
                            object pid = cmd.ExecuteScalar();
                            foreach (var w in workers)
                            {

                                string ext = Path.GetExtension(w.Picture);
                                string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                                string savePath = @"..\..\Pictures\" + f;
                                FileStream fs = new FileStream(savePath, FileMode.Create);
                                fs.Write(w.Image, 0, w.Image.Length);
                                fs.Close();
                                cmd.CommandText = "INSERT INTO workers (workername, phone, payrate, skill, picture, projectid) VALUES (@n, @p, @pr, @s, @pic, @pi)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@n", w.Name);
                                cmd.Parameters.AddWithValue("@p", w.Phone);
                                cmd.Parameters.AddWithValue("@pr", w.PayRate);
                                cmd.Parameters.AddWithValue("@s", w.Skill);
                                cmd.Parameters.AddWithValue("@pic", f);
                                cmd.Parameters.AddWithValue("@pi", pid);
                                cmd.ExecuteNonQuery();

                            }
                            trx.Commit();
                            MessageBox.Show("Data saved", "Success");
                            workers.Clear();
                            BindDataToGrid();
                            TheForm.BindDataToForm();
                        }
                        catch (Exception ex)
                        {
                            trx.Rollback();
                            MessageBox.Show("ERR: " + ex.Message, "Error");
                        }
                    }
                }
            }
        }
        public class Worker
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public decimal PayRate { get; set; }
            public string Skill { get; set; }
            public string Picture { get; set; }
            public string PictureFullPath { get; set; }
            public byte[] Image { get; set; }
        }
    }
}
