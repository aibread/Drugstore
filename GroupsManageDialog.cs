using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Drugstore
{
    public partial class GroupsManageDialog : Form
    {
        private EditListView GroupsListView;
        private DrugstoreData data;

        private List<Group> Groups = null;
        private List<ProductWithGroup> Products = null;

        public GroupsManageDialog()
        {
            InitializeComponent();

             
            data = new DrugstoreData();

            Groups = data.GetGroupList();
            
            Products = data.GetUnsetProductList();

            GroupsListView = new EditListView( Groups, Products );
            GroupsListView.Parent = this;      
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            data.UpdateProductList(Products);
            DialogResult = DialogResult.OK;
        }
    }
}
