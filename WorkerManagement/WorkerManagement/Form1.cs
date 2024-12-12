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
    public partial class Form1 : Form
    {
        BindingSource bsP = new BindingSource();
        BindingSource bsS = new BindingSource();
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormAddProject { TheForm = this }.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindDataToForm();
        }

        public void BindDataToForm()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using(SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM projects", con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "projects");
                    da.SelectCommand.CommandText = "SELECT * FROM workers";
                    da.Fill(ds, "workers");
                    ds.Relations.Add(new DataRelation("FK_P_W", ds.Tables["projects"].Columns["projectid"], ds.Tables["workers"].Columns["projectid"]));
                    ds.Tables["workers"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (int i = 0; i< ds.Tables["workers"].Rows.Count ; i++) 
                    {
                        ds.Tables["workers"].Rows[i]["image"] = File.ReadAllBytes(@"..\..\Pictures\" + ds.Tables["workers"].Rows[i]["picture"]);
                    }
                    bsP.DataSource = ds;
                    bsP.DataMember = "projects";
                    bsS.DataSource = bsP;
                    bsS.DataMember = "FK_P_W";
                    dataGridView1.DataSource = bsS;
                    lblname.DataBindings.Clear();
                    lblname.DataBindings.Add(new Binding("Text", bsP, "projectname"));
                    Binding bst = new Binding("Text", bsP, "startdate");
                    bst.Format += Bst_Format;
                    lblstart.DataBindings.Clear();
                    lblstart.DataBindings.Add(bst);
                }
            }
        }

        private void Bst_Format(object sender, ConvertEventArgs e)
        {
            DateTime dt = (DateTime)e.Value;
            e.Value= dt.ToString("yyyy-MM-dd");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(bsP.Position< bsP.Count-1)
            {
                bsP.MoveNext();
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bsP.MoveLast();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (bsP.Position > 0)
            {
                bsP.MovePrevious();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bsP.MoveFirst();
        }
    }
}
