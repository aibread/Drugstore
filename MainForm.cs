using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;
using Drugstore;

namespace Drugstore
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        
        private void TranslateForm()
        {
/*            Translator tr = new Translator();

            foreach(DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderText = tr.Translate( column.Name );   
            }

            this.Text = tr.Translate( this.Text );
*/ 
        }

        private void InitMainForm()
        {
/*            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            } 
*/
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //DrugstoreData data = new DrugstoreData();

            //dataGridView.DataSource = data.GetAbcTable(0);   //data.GetDataSource();
            //TranslateForm();
            //InitMainForm();          
        }


        private void groupReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DrugstoreData data = new DrugstoreData();
            //data.CalcGroupReport();
            List<AbcItem> list = data.GetAbcList();

            foreach (AbcItem group in list)
            {
                ListViewItem item = new ListViewItem(group.GroupName);
                item.SubItems.Add(group.SalesVolume.ToString());
                item.SubItems.Add(group.NaturalVolume.ToString());
                item.SubItems.Add(group.AbcGroup);
                item.SubItems.Add(group.Percentage.ToString());

                listView1.Items.Add(item);
            }
        }

        private void TestProductsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DrugstoreData data = new DrugstoreData();

            //List<ProductWithGroup> list = data.GetUnsetProductList();
            GroupsManageDialog d = new GroupsManageDialog();
            d.ShowDialog();
            // UpdateProductList
        }

        private void AdditionalToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
