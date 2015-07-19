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
    public partial class frmDataFormats : Form
    {
        Game game;
        string dataFormat;

        public frmDataFormats(ref Game game, string dataFormatName)
        {
            InitializeComponent();

            this.dataFormat = dataFormatName;
            this.game = game;
        }

        private void frmDataFormats_Load(object sender, EventArgs e)
        {
            lblDataFormatName.Text = dataFormat;

            if (this.game.DataFormats.ContainsKey(dataFormat))
            {
                txtDataFormatDescription.Text = game.DataFormats[dataFormat];
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.game.DataFormats.ContainsKey(dataFormat))
            {
                this.game.DataFormats[dataFormat] = txtDataFormatDescription.Text;
            }
            else
            {
                this.game.DataFormats.Add(dataFormat, txtDataFormatDescription.Text);
            }
        }
    }
}