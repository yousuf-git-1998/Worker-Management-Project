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
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM projects", con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "projects");
                    da.SelectCommand.CommandText = "SELECT * FROM workers";
                    da.Fill(ds, "workers1");
                   
                    ds.Tables["workers1"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (int i = 0; i < ds.Tables["workers1"].Rows.Count; i++)
                    {
                        ds.Tables["workers1"].Rows[i]["image"] = File.ReadAllBytes(@"..\..\Pictures\" + ds.Tables["workers1"].Rows[i]["picture"]);
                    }
                   Reports.CrystalReport1 rpt = new Reports.CrystalReport1();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
