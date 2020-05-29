using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MornaMapEditor
{
    public partial class FormMain : Form
    {
        public int sizeModifier;
        private static readonly FormMain FormInstance = new FormMain();
        private static string startupMapFile = null;

        public static FormMain GetFormInstance(string mapFile)
        {
            startupMapFile = mapFile;
            return FormInstance;
        }


        private FormMain()
        {
            InitializeComponent();
            MdiChildActivate += FormMain_MdiChildActivate;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // Extract files if required
            StartupUtilities.lblStatus = lblStatus;
            StartupUtilities.CreateMapEditorRegistryKey();
            if(!StartupUtilities.ExtractFiles(Application.UserAppDataPath)) return;

            // Load tiles
            TileManager.lblStatus = lblStatus;
            TileManager.Load(Application.UserAppDataPath);

            // Set forms to be MDI and show them
            fTile = FormTile.GetFormInstance();
            fTile.MdiParent = this;
            fObject = new FormObject { MdiParent = this };
            fTile.Show();
            fObject.Show();
            if (startupMapFile != null)
            {
                FormMap mapForm = new FormMap(this);
                mapForm.attemptToOpenMap(startupMapFile);
                mapForm.Show();

            }
        }

        private void FormMain_MdiChildActivate(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormMap)
            {
                FormMap formMap = (FormMap)ActiveMdiChild;
                //formMap.MinimapWindow.Location = new Point(Width - 280, 20);
                formMap.MinimapWindow.SetImage(formMap.pnlImage.Image);
                formMap.MinimapWindow.Visible = formMap.IsMinimapVisible;
                foreach (Form mdiChild in MdiChildren)
                {
                    if (mdiChild is FormMinimap && mdiChild != formMap.MinimapWindow)
                        mdiChild.Visible = false;
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ImageRenderer.Singleton.Dispose();
        }

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormMap(this).Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormAbout().ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you wish to exit?", "Exit Confirmation", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                foreach (Form mdiChild in MdiChildren) mdiChild.Close();
                if (MdiChildren.Length == 0) Close();
            }
        }

        private void x48ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Yes;

            foreach (Form mdiChild in MdiChildren)
            {
                if (mdiChild is FormMap)
                {
                    result = MessageBox.Show("This will cause the map to be rendered again. Proceed?", "Resize Tiles", MessageBoxButtons.YesNo);
                    break;
                }
            }

            if (result == DialogResult.Yes)
            {
                x36ToolStripMenuItem.Checked = false;
                x24ToolStripMenuItem.Checked = false;
                ImageRenderer.Singleton.ClearTileCache();
                ImageRenderer.Singleton.ClearObjectCache();
                ImageRenderer.Singleton.sizeModifier = 48;
                fTile.AdjustSizeModifier(48);
                fObject.Reload(true);

                foreach (Form mdiChild in MdiChildren)
                {
                    if (mdiChild is FormMap)
                    {
                        FormMap map = (FormMap)mdiChild;
                        map.Reload();
                    }
                }
            }
        }

        private void x36ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Yes;

            foreach (Form mdiChild in MdiChildren)
            {
                if (mdiChild is FormMap)
                {
                    result = MessageBox.Show("This will cause the map to be rendered again. Proceed?", "Resize Tiles", MessageBoxButtons.YesNo);
                    break;
                }
            }

            if (result == DialogResult.Yes)
            {
                x48ToolStripMenuItem.Checked = false;
                x24ToolStripMenuItem.Checked = false;
                ImageRenderer.Singleton.ClearTileCache();
                ImageRenderer.Singleton.ClearObjectCache();
                ImageRenderer.Singleton.sizeModifier = 36;
                fTile.AdjustSizeModifier(36);
                fObject.Reload(true);

                foreach (Form mdiChild in MdiChildren)
                {
                    if (mdiChild is FormMap)
                    {
                        FormMap map = (FormMap)mdiChild;
                        map.Reload();
                    }
                }
            }
        }

        private void x24ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Yes;

            foreach (Form mdiChild in MdiChildren)
            {
                if (mdiChild is FormMap)
                {
                    result = MessageBox.Show("This will cause the map to be rendered again. Proceed?", "Resize Tiles", MessageBoxButtons.YesNo);
                    break;
                }
            }

            if (result == DialogResult.Yes)
            {
                x48ToolStripMenuItem.Checked = false;
                x36ToolStripMenuItem.Checked = false;
                ImageRenderer.Singleton.ClearTileCache();
                ImageRenderer.Singleton.ClearObjectCache();
                ImageRenderer.Singleton.sizeModifier = 24;
                fTile.AdjustSizeModifier(24);
                fObject.Reload(true);

                foreach (Form mdiChild in MdiChildren)
                {
                    if (mdiChild is FormMap)
                    {
                        FormMap map = (FormMap)mdiChild;
                        map.Reload();
                    }
                }
            }
        }

        private void functionListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild is FormTile)
                MessageBox.Show("Left-Click: Select Tile\nCtrl + Left-Click: Select Multiple Tiles\nMouse-Wheel Up: Scroll Left\nMouse-Wheel Down: Scroll Right",
                    "Tile Window Help");
            else if (ActiveMdiChild is FormObject)
                MessageBox.Show("Left-Click: Select Object\nCtrl + Left-Click: Select Multiple Objects\nLeft-Click + Drag: Select Contiguous Objects\n" +
                    "Mouse-Wheel Up: Scroll Left\nMouse-Wheel Down: Scroll Right", "Object Window Help");
            else if (ActiveMdiChild is FormMap)
                MessageBox.Show("Left-Click: Paint Tile/Object Selection/Change Pass\nLeft-Click + Drag: Paint Contiguously\nCtrl + Left-Click: Copy Single Tile\n" +
                    "Ctrl + Left-Click + Drag: Copy Contiguous Tiles\nAlt + Left-Click: Copy Single Object\nAlt + Left-Click + Drag: Copy Contiguous Objects\n" +
                    "Shift + Left-Click: Copy Single Tile/Object\nShift + Left-Click + Drag: Copy Contiguous Tiles/Objects\nRight-Click: Fill Area\n" +
                    "Ctrl + Right-Click: Fill Map\nMouse-Wheel Up: Scroll Left\nMouse-Wheel Down: Scroll Right\nCtrl + Mouse-Wheel Up: Scroll Up\n" +
                    "Ctrl + Mouse-Wheel Down: Scroll Down", "Map Window Help");
        }

        private void showTilesMenuItem_Click(object sender, EventArgs e)
        {
            fTile = FormTile.GetFormInstance();
            fTile.WindowState = FormWindowState.Normal;
            fTile.Show();
        }

        private void convertMapsToPNGsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new BatchConverterDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                var converter = new BatchConverter(dialog.SourceFolder.Text, dialog.DestinationFolder.Text);
                var fileCount = converter.NumberToConvert();
                var confirmation =
                    MessageBox.Show($@"{fileCount} Files will be converted, continue?", "Convesion Confirmation", MessageBoxButtons.YesNo);
                if (confirmation != DialogResult.Yes) return;
                converter.ConvertMaps();
                var startInfo = new ProcessStartInfo("explorer.exe", dialog.DestinationFolder.Text);
                Process.Start(startInfo);
                MessageBox.Show("Creation of map images complete.");
            }
        }
    }
}
