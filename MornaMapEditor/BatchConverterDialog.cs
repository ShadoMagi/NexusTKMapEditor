using System;
using System.IO;
using System.Windows.Forms;

namespace MornaMapEditor
{
    public partial class BatchConverterDialog : Form
    {
        public BatchConverterDialog()
        {
            InitializeComponent();
            var defaultMapPath =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\NexusTK\\Maps";
            sourceFolder.Text = destinationFolder.Text =
                Directory.Exists(defaultMapPath) ? defaultMapPath : Application.UserAppDataPath;
        }

        public TextBox SourceFolder => sourceFolder;
        public TextBox DestinationFolder => destinationFolder;

        private void sourceButton_Click(object sender, EventArgs e)
        {
            var browseDialog = new FolderBrowserDialog();
            browseDialog.Description = "Select Source Folder";
            browseDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            browseDialog.SelectedPath = sourceFolder.Text;
            browseDialog.ShowNewFolderButton = false;
            DialogResult browseResult = browseDialog.ShowDialog();
            if (browseResult == DialogResult.OK)
            {
                sourceFolder.Text = browseDialog.SelectedPath;
            }
        }

        private void destinationButton_Click(object sender, EventArgs e)
        {
            var browseDialog = new FolderBrowserDialog();
            browseDialog.Description = "Select Destination Folder";
            browseDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            browseDialog.SelectedPath = destinationFolder.Text;
            browseDialog.ShowNewFolderButton = true;
            DialogResult browseResult = browseDialog.ShowDialog();
            if (browseResult == DialogResult.OK)
            {
                destinationFolder.Text = browseDialog.SelectedPath;
            }
        }
    }
}