using System;
using System.Windows.Forms;
using System.IO;

namespace ROMDataWin
{
    public partial class frmPreferences : Form
    {
        ApplicationConfig config;
        public frmPreferences(ref ApplicationConfig config)
        {
            InitializeComponent();
            this.config = config;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtDataDirectory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void frmPreferences_Load(object sender, EventArgs e)
        {
            LoadPreferences();
        }

        private void LoadPreferences()
        {
            txtDataDirectory.Text = config.DefaultDataDirectory;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SavePreferences();
        }

        private void SavePreferences()
        {
            config.DefaultDataDirectory = txtDataDirectory.Text;
        }
    }
}
