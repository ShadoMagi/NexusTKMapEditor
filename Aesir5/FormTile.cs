using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Aesir5
{
    public partial class FormTile : Form
    {
        private readonly List<Point> selectedTiles = new List<Point>();
        private Point focusedTile = new Point(-1,-1);
        private int sizeModifier;
        private bool showGrid;
        public bool ShowGrid
        {
            get { return showGrid; }
            set { showGrid = value; this.Invalidate(); }
        }

        public FormTile()
        {
            InitializeComponent();
            this.MouseWheel += new MouseEventHandler(frmTile_MouseWheel);
            //this.Paint += frmTile_Paint;
        }

        private void frmTile_Load(object sender, EventArgs e)
        {
            Reload(false);
            MinimumSize = new Size(MinimumSize.Width + 10, MinimumSize.Height + 10);
            MaximumSize = new Size(MaximumSize.Width + 10, MaximumSize.Height + 10);
            if (this.BackgroundImage == null) this.BackgroundImage = new Bitmap(10 * sizeModifier, 10 * sizeModifier);
            menuStrip.Visible = false;
            sb1.Maximum = (TileManager.Epf[0].max / 100) + 9;
            RenderTileset();
        }

        private int GetTileNumber(int x, int y)
        {
            return sb1.Value*100 + (y*10) + x;
        }

        private void RenderTileset()
        {
            //Bitmap tSet = new Bitmap(360, 360);
            if (this.BackgroundImage == null) this.BackgroundImage = new Bitmap(10 * sizeModifier, 10 * sizeModifier);
            Graphics g = Graphics.FromImage(this.BackgroundImage);
            g.Clear(Color.DarkGreen);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    int tile = GetTileNumber(x, y);

                    if (tile < TileManager.Epf[0].max)
                    {
                        //Bitmap bitmap = ImageRenderer.Singleton.GetTileBitmap(tile);
                        g.DrawImage(new ImageRenderer().GetTileBitmap(tile), x * sizeModifier, y * sizeModifier);//, 36, 36);
                        //bitmap = null;
                    }
                }
            }

            g.Dispose();
            this.Invalidate();
            //this.BackgroundImage = tSet;
            //picTileset.Image = tSet;
            //Application.DoEvents();
            //tSet = null;
        }

        private void sb1_Scroll(object sender, ScrollEventArgs e)
        {
            selectedTiles.Clear();
            RenderTileset();
        }

        void frmTile_Paint(object sender, PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (ShowGrid)
            {
                Pen penGrid = new Pen(Color.LightCyan, 1);
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        e.Graphics.DrawRectangle(penGrid, i*sizeModifier, j*sizeModifier, sizeModifier, sizeModifier);
                    }
                }
                penGrid.Dispose();
            }

            if (selectedTiles.Count > 0)
            {
                Pen pen = new Pen(Color.Red, 2);
                foreach (var selectedTile in selectedTiles)
                {
                    e.Graphics.DrawRectangle(pen, selectedTile.X * sizeModifier, selectedTile.Y * sizeModifier, sizeModifier, sizeModifier);
                }
                pen.Dispose();
            }

            if (focusedTile.X >= 0 && focusedTile.Y >= 0)
            {
                Pen pen = new Pen(Color.Green, 2);
                e.Graphics.DrawRectangle(pen, focusedTile.X * sizeModifier, focusedTile.Y * sizeModifier, sizeModifier, sizeModifier);
                pen.Dispose();
            }
        }

        private void frmTile_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (sb1.Value - 1 >= sb1.Minimum) sb1.Value--;
            }
            else if (e.Delta < 0)
            {
                if (sb1.Value + 1 <= sb1.Maximum) sb1.Value++;
            }

            sb1_Scroll(null, null);
        }

        private void frmTile_MouseMove(object sender, MouseEventArgs e)
        {
            int newFocusedTileX = e.X / sizeModifier;
            int newFocusedTileY = e.Y / sizeModifier;
            bool refresh = (newFocusedTileX != focusedTile.X || newFocusedTileY != focusedTile.Y);

            if (refresh)
            {
                focusedTile = new Point(newFocusedTileX, newFocusedTileY);
                this.Invalidate();
                int tileNumber = GetTileNumber(newFocusedTileX, newFocusedTileY);
                toolStripStatusLabel.Text = string.Format("Tile number: {0}", tileNumber);
            }
        }

        private void frmTile_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            int newSelectedTileX = e.X / sizeModifier;
            int newSelectedTileY = e.Y / sizeModifier;

            Point selectedTile = new Point(newSelectedTileX, newSelectedTileY);
            if (ModifierKeys == Keys.Control)
            {
                if (!selectedTiles.Contains(selectedTile))
                    selectedTiles.Add(selectedTile);
            }
            else
            {
                selectedTiles.Clear();
                selectedTiles.Add(selectedTile);
            }
            TileManager.TileSelection = GetSelection();
            TileManager.LastSelection = TileManager.SelectionType.Tile;
            //RenderTileset();
        }

        public Dictionary<Point, int> GetSelection()
        {
            Dictionary<Point, int> dictionary = new Dictionary<Point, int>();
            if (selectedTiles.Count == 0) return dictionary;

            int xMin = selectedTiles[0].X, yMin = selectedTiles[0].Y;

            foreach (Point selectedTile in selectedTiles)
            {
                if (xMin > selectedTile.X) xMin = selectedTile.X;
                if (yMin > selectedTile.Y) yMin = selectedTile.Y;
            }

            foreach (Point selectedTile in selectedTiles)
            {
                dictionary.Add(new Point(selectedTile.X - xMin, selectedTile.Y - yMin), 
                    GetTileNumber(selectedTile.X, selectedTile.Y));
            }

            return dictionary;
        }

        /*public void ClearSelection()
        {
            selectedTiles.Clear();
            this.Invalidate();
        }*/

        private void findTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NumberInputForm numberInputForm = new NumberInputForm(@"Enter object number");
            if (numberInputForm.ShowDialog(this) == DialogResult.OK)
            {
                NavigateToTile(numberInputForm.Number);
            }
        }

        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowGrid = showGridToolStripMenuItem.Checked;
        }

        public void NavigateToTile(int number)
        {
            if (number < 0 || number>(sb1.Maximum*100 + 99)) return;

            int sbIndex = number / 100;
            int y = (number - sbIndex * 100) / 10;
            int x = number - sbIndex * 100 - y * 10;

            sb1.Value = sbIndex;
            selectedTiles.Clear();
            selectedTiles.Add(new Point(x, y));
            TileManager.TileSelection = GetSelection();
            TileManager.LastSelection = TileManager.SelectionType.Tile;
            RenderTileset();
        }

        public void Reload(bool render)
        {
            sizeModifier = ImageRenderer.SizeModifier;
            SetClientSizeCore((10 * sizeModifier) - 1, (10 * sizeModifier) + 39);
            MinimumSize = new Size(ClientSize.Width + 6, ClientSize.Height + 24);
            MaximumSize = new Size(ClientSize.Width + 6, ClientSize.Height + 24);
            this.BackgroundImage = null;

            if (render) RenderTileset();
        }
    }
}
