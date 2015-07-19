using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ROMDataLib;
using System.Collections.Generic;

namespace ROMDataWin
{

    public enum SaveDialogSetupType
    {
        RDIFile, TextExport, HTMLExport
    }

    public partial class frmMain : Form
    {
        Game game;
        bool changed = false;
        string xmlfilename = string.Empty;
        ApplicationConfig config;
        string executablePath;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            executablePath = Path.GetDirectoryName(Application.ExecutablePath);

            this.SetEnabledStatus(false);
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            config = new ApplicationConfig(Path.Combine(executablePath, "config.xml"));
            config.LoadConfiguration();
            mnuViewCategories.Checked = config.GroupByCategory;

            if (!Directory.Exists(config.DefaultDataDirectory))
            {
                if (MessageBox.Show("The default data directory does not exist. Do you want create it?", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Directory.CreateDirectory(config.DefaultDataDirectory);
                }
            }
            PopulateRecentMenu();
        }

        private void mnuNew_Click(object sender, EventArgs e)
        {
            using (frmNew newfrm = new frmNew())
            {
                // Open the new dialog, and retrieve the name of the game, and the file size.
                if (newfrm.ShowDialog() == DialogResult.OK)
                {
                    this.game = new Game();
                    this.game.Name = newfrm.GameName;
                    this.game.Size = newfrm.FileSize;
                    this.changed = true;
                    this.SetEnabledStatus(true);
                    this.SetTitleCaption();
                }
            }
        }

        private void SetEnabledStatus(bool enabled)
        {
            lvwDataLocations.Enabled = enabled;
            mnuSave.Enabled = enabled;
            mnuClose.Enabled = enabled;
            mnuViewCategories.Enabled = enabled;
            mnuNewData.Enabled = enabled;
            mnuManageCategories.Enabled = enabled;
            mnuExport.Enabled = enabled;
            mnuExportHTML.Enabled = enabled;
            mnuExportText.Enabled = enabled;
            mnuAnalyse.Enabled = enabled;
            mnuAnalyseStatistics.Enabled = enabled;
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            SetupSaveDialog(SaveDialogSetupType.RDIFile);

            string currentfilename = string.Empty;

            if (string.IsNullOrEmpty(this.xmlfilename))
            {
                if (Directory.Exists(config.DefaultDataDirectory))
                {
                    sfdXML.InitialDirectory = config.DefaultDataDirectory;
                }

                if (sfdXML.ShowDialog() == DialogResult.OK)
                {
                    this.xmlfilename = sfdXML.FileName;
                }
            }

            currentfilename = this.xmlfilename;

            ROMDataManager romMan = new ROMDataManager();
            romMan.SaveROMData(ref this.game, currentfilename);
            changed = false;
            this.SetTitleCaption();
            config.AddToRecentFiles(currentfilename);
        }

        private void mnuNewData_Click(object sender, EventArgs e)
        {
            using (frmNewData datafrm = new frmNewData(ref game, null))
            {
                if (datafrm.ShowDialog() == DialogResult.OK)
                {
                    this.changed = true;
                    this.PopulateListview();
                    this.SetTitleCaption();
                }
            }
        }

        private void PopulateListview()
        {
            lvwDataLocations.BeginUpdate();

            SetupListviewColumns(config.GroupByCategory);

            lvwDataLocations.Items.Clear();

            lvwDataLocations.ShowGroups = config.GroupByCategory;

            Dictionary<string, int> groupIndexes = new Dictionary<string, int>();
            lvwDataLocations.Groups.Clear();

            // Make sure that the data locations are sorted on population.
            game.SortDataLocations();

            // Add a None category.
            lvwDataLocations.Groups.Add(new ListViewGroup("None"));

            for (int i = 0; i < game.Categories.Count; i++)
            {
                groupIndexes.Add(game.Categories[i], i + 1);
                lvwDataLocations.Groups.Add(new ListViewGroup(game.Categories[i]));
            }

            for (int i = 0; i < game.DataLocations.Count; i++)
            {
                ListViewItem item = new ListViewItem(game.DataLocations[i].Name);

                item.SubItems.Add(HelperMethods.PadHex(game.DataLocations[i].StartOffset));
                if (game.DataLocations[i].EndOffset != null)
                {
                    item.SubItems.Add(HelperMethods.PadHex(Convert.ToInt32(game.DataLocations[i].EndOffset)));
                }
                else
                {
                    item.SubItems.Add("Unknown");
                }

                if (string.IsNullOrEmpty(game.DataLocations[i].Category))
                {
                    item.Group = lvwDataLocations.Groups[0];
                }
                else
                {
                    item.Group = lvwDataLocations.Groups[groupIndexes[game.DataLocations[i].Category]];
                }

                if (!config.GroupByCategory)
                {
                    item.SubItems.Add(item.Group.Header);
                }
                item.Tag = i;

                lvwDataLocations.Items.Add(item);
            }
            lvwDataLocations.EndUpdate();
        }

        private void SetupListviewColumns(bool showGroups)
        {
            lvwDataLocations.Columns.Clear();

            ColumnHeader column = new ColumnHeader();

            // Name column.
            column.Text = "Name.";
            column.Width = 300;
            lvwDataLocations.Columns.Add(column);

            // Start offset column.
            column = new ColumnHeader();
            column.Text = "Start Offset.";
            column.Width = 100;
            lvwDataLocations.Columns.Add(column);

            // end offset column
            column = new ColumnHeader();
            column.Text = "End Offset.";
            column.Width = 100;
            lvwDataLocations.Columns.Add(column);

            if (!showGroups)
            {
                column = new ColumnHeader();
                column.Text = "Category.";
                column.Width = 100;
                lvwDataLocations.Columns.Add(column);
            }
        }

        private void SetTitleCaption()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ROM Data Tool");

            if (string.IsNullOrEmpty(this.xmlfilename))
            {
                sb.Append(" [Untitled Document]");
            }
            else
            {
                sb.Append(" [");
                FileInfo fi = new FileInfo(this.xmlfilename);
                sb.Append(fi.Name);
                sb.Append("]");
            }

            if (this.changed == true)
            {
                sb.Append(" *");
            }

            this.Text = sb.ToString();
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(config.DefaultDataDirectory))
            {
                ofdXML.InitialDirectory = config.DefaultDataDirectory;
            }

            if (ofdXML.ShowDialog() == DialogResult.OK)
            {
                LoadDataDocument(ofdXML.FileName);
            }
        }

        private void LoadDataDocument(string filename)
        {
            ROMDataManager romMan = new ROMDataManager();
            this.game = romMan.LoadROMData(filename);
            this.xmlfilename = filename;
            this.PopulateListview();
            this.SetTitleCaption();
            this.SetEnabledStatus(true);
            config.AddToRecentFiles(filename);
        }

        private void lvwDataLocations_DoubleClick(object sender, EventArgs e)
        {
            if (lvwDataLocations.SelectedItems.Count > 0)
            {
                int id = Convert.ToInt32(lvwDataLocations.SelectedItems[0].Tag);

                using (frmNewData datafrm = new frmNewData(ref game, id))
                {
                    if (datafrm.ShowDialog() == DialogResult.OK)
                    {
                        this.changed = true;
                        this.PopulateListview();
                        this.SetTitleCaption();
                    }
                }
            }
        }

        private void PopulateRecentMenu()
        {
            if (config.RecentFiles != null && config.RecentFiles.Count > 0)
            {
                mnuRecent.Visible = true;
                mnuRecent.DropDownItems.Clear();

                foreach (string recentfile in config.RecentFiles)
                {
                    ToolStripMenuItem menu = new ToolStripMenuItem(recentfile);
                    menu.Tag = recentfile;
                    menu.Click += new EventHandler(mnuRecentItemClick);
                    mnuRecent.DropDownItems.Add(menu);
                }
            }
            else
            {
                mnuRecent.Visible = false;
            }
        }

        private void mnuRecentItemClick(object sender, EventArgs e)
        {
            LoadDataDocument(Convert.ToString((sender as ToolStripMenuItem).Tag));
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            config.SaveConfiguration();
            if (changed == true && game != null)
            {
                DialogResult dr = MessageBox.Show("The game information has not been saved. Do you wish to save it?", Application.ProductName, MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    SaveFile();
                }
                else if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void addNewCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCategories categoryfrm = new frmCategories(ref game);
            if (categoryfrm.ShowDialog() == DialogResult.OK)
            {
                PopulateListview();
                changed = true;
                this.SetTitleCaption();
            }
        }

        private void mnuViewCategories_Click(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;
            config.GroupByCategory = (sender as ToolStripMenuItem).Checked;

            PopulateListview();
        }

        private void descriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;
            //config.GroupByCategory = (sender as ToolStripMenuItem).Checked;

            splitContainer1.Panel2Collapsed = !(sender as ToolStripMenuItem).Checked;
        }

        private void lvwDataLocations_Click(object sender, EventArgs e)
        {
            if (lvwDataLocations.SelectedItems.Count > 0)
            {
                int id = Convert.ToInt32(lvwDataLocations.SelectedItems[0].Tag);
                txtDescription.Text = game.DataLocations[id].Description;
            }
        }

        private void txtDescription_KeyUp(object sender, KeyEventArgs e)
        {
            if (lvwDataLocations.SelectedItems.Count > 0)
            {
                int id = Convert.ToInt32(lvwDataLocations.SelectedItems[0].Tag);
                if (game.DataLocations[id].Description != txtDescription.Text)
                {
                    game.DataLocations[id].Description = txtDescription.Text;
                    changed = true;
                    SetTitleCaption();
                }
            }
        }

        private void mnuPreferences_Click(object sender, EventArgs e)
        {
            frmPreferences pref = new frmPreferences(ref config);
            pref.ShowDialog();
        }

        private void exportAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ROMDataLib.Export.TextExport export = new ROMDataLib.Export.TextExport();
            SetupSaveDialog(SaveDialogSetupType.TextExport);

            if (sfdXML.ShowDialog() == DialogResult.OK)
            {
                export.Export(ref game, sfdXML.FileName);
                MessageBox.Show("Game data exported to a text file.");
            }
        }

        private void SetupSaveDialog(SaveDialogSetupType type)
        {
            if (type == SaveDialogSetupType.RDIFile)
            {
                sfdXML.DefaultExt = "rdi";
                sfdXML.Filter = "ROM Data Information (*.rdi)|*.rdi";
                sfdXML.Title = "Please enter a filename for your game data";
            }
            else if (type == SaveDialogSetupType.TextExport)
            {
                sfdXML.DefaultExt = "txt";
                sfdXML.Filter = "Text File (*.txt)|*.txt";
                sfdXML.Title = "Please enter a filename for your exported game data";
            }
            else if (type == SaveDialogSetupType.HTMLExport)
            {
                sfdXML.DefaultExt = "html";
                sfdXML.Filter = "HTML File (*.html)|*.html";
                sfdXML.Title = "Please enter a filename for your exported game data";
            }
        }

        private void mnuExportHTML_Click(object sender, EventArgs e)
        {
            ROMDataLib.Export.HTMLExport export = new ROMDataLib.Export.HTMLExport();
            SetupSaveDialog(SaveDialogSetupType.HTMLExport);

            if (sfdXML.ShowDialog() == DialogResult.OK)
            {
                export.Export(ref game, sfdXML.FileName);
                MessageBox.Show("Game data exported to an HTML file.");
            }
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // First, look up the size of the ROM.
            int size = game.Size;

            // Now, create a bool array that is of the size of the ROM.
            bool[] mappedData = new bool[size];

            // Now, loop through the data locations, and set to true, the items in the boolean array
            // that are mapped out.
            foreach (DataLocation data in game.DataLocations)
            {
                int startoffset = data.StartOffset;
                int endoffset;

                if (data.EndOffset.HasValue == false || data.EndOffset < data.StartOffset)
                {
                    endoffset = startoffset;
                }
                else
                {
                    endoffset = data.EndOffset.Value;
                }

                for (int i = startoffset; i < endoffset; i++)
                {
                    mappedData[i] = true;
                }

            }

            int mappedDataCount = 0;
            int unmappedDataCount = 0;

            // Now iterate through the mappedData array, work out how many bytes are mapped and work out how many bytes are not mapped.
            for (int i = 0; i < mappedData.Length; i++)
            {
                if (mappedData[i] == true)
                {
                    mappedDataCount++;
                }
                else
                {
                    unmappedDataCount++;
                }
            }

            MessageBox.Show(string.Format("Mapped Data Count = {0}{1}Unmapped Data Count = {2}", mappedDataCount.ToString(), Environment.NewLine, unmappedDataCount.ToString()));

        }
    }
}