using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ROMDataLib;

namespace ROMDataWin
{
    public partial class frmCategories : Form
    {
        Game game;

        public frmCategories(ref Game game)
        {
            InitializeComponent();
            this.game = game;
        }

        private void frmCategories_Load(object sender, EventArgs e)
        {
            PopulateListView();
        }

        private void PopulateListView()
        {
            lvwCategories.BeginUpdate();
            lvwCategories.Items.Clear();
            foreach (string category in game.Categories)
            {
                ListViewItem item = new ListViewItem(category);
                item.Tag = category;
                lvwCategories.Items.Add(category);
            }
            lvwCategories.EndUpdate();
        }

        private void lvwCategories_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == string.Empty)
            {
                e.CancelEdit = true;
            }
            PopulateListView();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Loop through all the listview items, comparing the text to the tags.
            // any that don't match, should be updated.
            foreach (ListViewItem item in lvwCategories.Items)
            {
                if (Convert.ToString(item.Tag) == item.Text)
                {
                    game.UpdateGameCategories(Convert.ToString(item.Tag), item.Text);
                }
            }
        }

        private void lvwCategories_Click(object sender, EventArgs e)
        {

        }

        private void btnAddEdit_Click(object sender, EventArgs e)
        {
            game.Categories.Add(txtCategory.Text);
            PopulateListView();
        }
    }
}
