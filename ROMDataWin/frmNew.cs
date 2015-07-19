using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ROMDataWin
{
    public partial class frmNew : Form
    {

        int fileSize;

        public frmNew()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofdROM.ShowDialog() == DialogResult.OK)
            {
                // Populate the textbox with the filename of the ROM, and 
                // set the part of the filename to be shown, to be the end of the filename.
                txtFilename.Text = ofdROM.FileName;
                txtFilename.SelectionStart = txtFilename.Text.Length;

                // Work out the size of the ROM.
                FileInfo info = new FileInfo(ofdROM.FileName);
                fileSize = Convert.ToInt32(info.Length);
                lblFileSizeTotal.Text = info.Length.ToString() + " bytes";

                btnOK.Enabled = true;
            }
        }

        private void frmNew_Load(object sender, EventArgs e)
        {
            btnOK.Enabled = false;
        }

        public string GameName
        {
            get
            {
                return txtName.Text;
            }
        }

        public Int32 FileSize
        {
            get
            {
                return fileSize;
            }
        }
    }
}
