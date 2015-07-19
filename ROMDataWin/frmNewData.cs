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
    public partial class frmNewData : Form
    {
        Game game;
        int? editDataIndex;

        public frmNewData(ref Game game, int? dataIndex)
        {
            InitializeComponent();
            this.game = game;
            editDataIndex = dataIndex;
        }

        private void frmNewData_Load(object sender, EventArgs e)
        {
            PopulateCategories();
            PopulateDataFormats(string.Empty);

            if (editDataIndex != null)
            {
                DataLocation data = null;
                data = game.DataLocations[Convert.ToInt32(editDataIndex)];
                txtDescription.Text = data.Description;
                if (data.EndOffset != null)
                {
                    txtEndOffset.Text = HelperMethods.PadHex(Convert.ToInt32(data.EndOffset));
                }
                txtStartOffset.Text = HelperMethods.PadHex(data.StartOffset);
                txtName.Text = data.Name;

                if (data.Category != string.Empty)
                {
                    cbCategory.Text = data.Category;
                }

                if (data.DataFormat != string.Empty)
                {
                    cbDataFormat.Text = data.DataFormat;
                }

                this.Text = "Edit Data Location";

            }
        }

        private void PopulateDataFormats(string defaultValue)
        {
            cbDataFormat.BeginUpdate();

            cbDataFormat.Items.Clear();

            foreach (KeyValuePair<string, string> pair in game.DataFormats)
            {
                cbDataFormat.Items.Add(pair.Key);
            }

            cbDataFormat.EndUpdate();

            if (defaultValue != string.Empty)
            {
                cbDataFormat.Text = defaultValue;
            }
        }

        private void PopulateCategories()
        {
            cbCategory.BeginUpdate();

            cbCategory.Items.Clear();

            foreach (string category in game.Categories)
            {
                cbCategory.Items.Add(category);
            }

            cbCategory.EndUpdate();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            DataLocation data = null;
            if (editDataIndex == null)
            {
                data = new DataLocation();
                game.DataLocations.Add(data);
            }
            else
            {
                data = game.DataLocations[Convert.ToInt32(editDataIndex)];
            }

            if (cbCategory.Text != string.Empty)
            {
                if (!game.Categories.Contains(cbCategory.Text))
                {
                    game.Categories.Add(cbCategory.Text);
                }
            }

            data.Description = txtDescription.Text;
            if (chkOneByte.Checked)
            {
                data.EndOffset = Convert.ToInt32(txtStartOffset.Text, 16);
            }
            else
            {
                if (!string.IsNullOrEmpty(txtEndOffset.Text))
                {
                    data.EndOffset = Convert.ToInt32(txtEndOffset.Text, 16);
                }
            }

            data.StartOffset = Convert.ToInt32(txtStartOffset.Text, 16);
            data.Name = txtName.Text;
            data.Category = cbCategory.Text;
        }

        private void btnEditDataFormat_Click(object sender, EventArgs e)
        {
            if (cbDataFormat.Text != string.Empty)
            {
                frmDataFormats frmDataForm = new frmDataFormats(ref game, cbDataFormat.Text);

                if (frmDataForm.ShowDialog(this) == DialogResult.OK)
                {
                    PopulateDataFormats(cbDataFormat.Text);
                }
            }
            else
            {
                MessageBox.Show("Please enter the name of the data format.");
            }
        }

        private void chkOneByte_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOneByte.Checked)
            {
                txtEndOffset.ReadOnly = true;
            }
            else
            {
                txtEndOffset.ReadOnly = false;
            }
        }
    }
}