using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;


namespace Drugstore
{
    class EditListView : ListView
    {
        private ListViewItem li;
        private int X = 0;
        private int Y = 0;
        private string subItemText;
        private int subItemSelected = 0;
      //  private System.Windows.Forms.TextBox editBox = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.ComboBox cmbBox = new System.Windows.Forms.ComboBox();
        private List<Group> Groups = null;
        private List<ProductWithGroup> Products = null;


        public EditListView(List<Group> g, List<ProductWithGroup> p)
        {
            Groups = g;
            Products = p;
            
            foreach (Group group in Groups)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Text = group.GroupName;
                item.Value = group.GroupId;
                cmbBox.Items.Add( item );
            }
            int k = 0;
            foreach(ProductWithGroup product in Products)
            {
                ListViewItem item = new ListViewItem(product.ProductName);
                item.Tag = k; 
                item.SubItems.Add("Не указан");
                
                this.Items.Add(item);
                k++;
            }
                                    
            cmbBox.Size = new System.Drawing.Size(0, 0);
            cmbBox.Location = new System.Drawing.Point(0, 0);
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.cmbBox });
            cmbBox.SelectedIndexChanged += new System.EventHandler(this.CmbSelected);
            cmbBox.LostFocus += new System.EventHandler(this.CmbFocusOver);
            cmbBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.CmbKeyPress);
            cmbBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F,
                                                       System.Drawing.FontStyle.Regular,
                                                       System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            cmbBox.BackColor = Color.Yellow;
            cmbBox.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBox.Hide();
            cmbBox.SelectedIndexChanged += new EventHandler( ComboBoxSelectedIndexChanged );      
                

            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F,
                                                       System.Drawing.FontStyle.Regular,
                                                       System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.FullRowSelect = true;

            this.Size = new System.Drawing.Size(520, 300);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SMKMouseDown);
            this.Click += new System.EventHandler(this.EditListViewClick);
            this.GridLines = true;
            System.Windows.Forms.ColumnHeader columnProduct = new System.Windows.Forms.ColumnHeader();
            System.Windows.Forms.ColumnHeader columnProductGroup = new System.Windows.Forms.ColumnHeader();
  
            this.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
				columnProduct,
				columnProductGroup });

            this.FullRowSelect = true;
            this.GridLines = true;
            this.Location = new System.Drawing.Point(12, 12);
            this.Name = "ProductGroupsListView";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            //this.Size = new System.Drawing.Size(542, 337);
            this.TabIndex = 1;
            this.UseCompatibleStateImageBehavior = false;
            this.View = System.Windows.Forms.View.Details;
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Alignment = ListViewAlignment.SnapToGrid;
            // 
            // columnProduct
            // 
            columnProduct.Text = "Товар";
            columnProduct.Name = "Product";
            // 
            // columnProductGroup
            // 
            columnProductGroup.Text = "Группа продуктов";
            columnProductGroup.Name = "ProductGroup";
            
            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].Width = this.Width / Columns.Count - 12;
            }

            //editBox.Size = new System.Drawing.Size(0, 0);
            //editBox.Location = new System.Drawing.Point(0, 0);
            //this.Controls.AddRange(new System.Windows.Forms.Control[] { this.editBox });
            //editBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditOver);
            //editBox.LostFocus += new System.EventHandler(this.FocusOver);
            //editBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F,
            //                    System.Drawing.FontStyle.Regular,
            //                    System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            //editBox.BackColor = Color.LightYellow;
            //editBox.BorderStyle = BorderStyle.Fixed3D;
            //editBox.Hide();
            //editBox.Text = "";
        }

        private void CmbKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 || e.KeyChar == 27)
            {
                cmbBox.Hide();
            }
        }

        void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            cmbBox.Hide();
        }

        private void CmbSelected(object sender, System.EventArgs e)
        {
            int sel = cmbBox.SelectedIndex;
            if (sel >= 0)
            {
                string itemSel = cmbBox.Items[sel].ToString();
                li.SubItems[subItemSelected].Text = itemSel;

                int p = (int)li.Tag;
                int g = (int)((ComboBoxItem)cmbBox.Items[sel]).Value;
                Products[p].GroupId = g;
            }
        }

        private void CmbFocusOver(object sender, System.EventArgs e)
        {
            cmbBox.Hide();
        }

        //private void EditOver(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    if (e.KeyChar == 13)
        //    {
        //        li.SubItems[subItemSelected].Text = editBox.Text;
        //        editBox.Hide();
        //    }

        //    if (e.KeyChar == 27)
        //        editBox.Hide();
        //}

        //private void FocusOver(object sender, System.EventArgs e)
        //{
        //    li.SubItems[subItemSelected].Text = editBox.Text;
        //    editBox.Hide();
        //}

        public void EditListViewClick(object sender, System.EventArgs e)
        {
            // Check the subitem clicked .
            int nStart = X;
            int spos = 0;
            int epos = this.Columns[0].Width;
            for (int i = 0; i < this.Columns.Count; i++)
            {
                if (nStart > spos && nStart < epos)
                {
                    subItemSelected = i;
                    break;
                }

                spos = epos;
                epos += this.Columns[i].Width;
            }

            Console.WriteLine("SUB ITEM SELECTED = " + li.SubItems[subItemSelected].Text);
            subItemText = li.SubItems[subItemSelected].Text;

            string colName = this.Columns[subItemSelected].Name;
            
            if (colName == "ProductGroup")
            {
                Rectangle r = new Rectangle(spos, li.Bounds.Y, epos, li.Bounds.Bottom);
                cmbBox.Size = new System.Drawing.Size(epos - spos - 1, li.Bounds.Bottom - li.Bounds.Top);
                cmbBox.Location = new System.Drawing.Point(spos + 1, li.Bounds.Y);
                cmbBox.Show();
                cmbBox.Text = subItemText;
                cmbBox.SelectAll();
                cmbBox.Focus();
            }
        }

        public void SMKMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Подозреваю, что тут кроется косяк, но как исправить не знаю
            li = this.GetItemAt(e.X, e.Y);
            X = e.X;
            Y = e.Y;
        }
    }

}
