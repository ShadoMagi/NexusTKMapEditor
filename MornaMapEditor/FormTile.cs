using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MornaMapEditor
{
    public partial class FormTile : Form
    {

        private static readonly int pixelBuffer = 10;
        private static readonly FormTile FormInstance = new FormTile();

        private readonly List<Point> selectedTiles = new List<Point>();
        private Point focusedTile = Point.Empty;
        private int usableHeight = 0;
        private int tilesPerRow = 0;
        private int tileRows = 0;
        private int sizeModifier;
        private bool showGrid;
        
        public bool ShowGrid
        {
            get { return showGrid; }
            set { showGrid = value; Invalidate(); }
        }

        public static FormTile GetFormInstance()
        {
            return FormInstance;
        }

        private FormTile()
        {
            InitializeComponent();
            sizeModifier = ImageRenderer.Singleton.sizeModifier;
            MouseWheel += frmTile_MouseWheel;
        }

        private void frmTile_Load(object sender, EventArgs e)
        {
            menuStrip.Visible = false;
            MinimumSize = new Size(sizeModifier + pixelBuffer, sizeModifier + sb1.Height + menuStrip.Height + statusStrip.Height + pixelBuffer);
            updateRenderParams();
        }

        private void updateRenderParams()
        {
            usableHeight = Height - sb1.Height - menuStrip.Height - statusStrip.Height - pixelBuffer;
            tileRows = usableHeight / sizeModifier;
            tilesPerRow = TileManager.Epf[0].max / tileRows;
            sb1.Maximum = tilesPerRow + (Width / sizeModifier);
            sb1.LargeChange = (Width / sizeModifier);
            selectedTiles.Clear();
            Invalidate();
        }

        private void sb1_Scroll(object sender, ScrollEventArgs e)
        {
            selectedTiles.Clear();
            //Trigger an 'update' to the focused tile
            updateFocusedTile(focusedTile.X, focusedTile.Y);
            Invalidate();
        }

        void formTile_Paint(object sender, PaintEventArgs e)
        {
            if (BackgroundImage != null)
                BackgroundImage.Dispose();
            if (WindowState == FormWindowState.Minimized)
                return;

            //Bitmap tSet = new Bitmap(360, 360);
            Bitmap tmpBitmap = new Bitmap(Width, usableHeight);
            Graphics graphics = Graphics.FromImage(tmpBitmap);
            graphics.Clear(Color.DarkGreen);
            Pen penGrid = new Pen(Color.LightCyan, 1);

            for (int xIndex = 0; xIndex <= (int) Math.Ceiling(Convert.ToDouble(Width / sizeModifier)); xIndex++)
            {
                for (int yIndex = 0; yIndex <= tileRows; yIndex++)
                {
                    Rectangle tileRectangle = new Rectangle(xIndex * sizeModifier, yIndex * sizeModifier, sizeModifier, sizeModifier);
                    int tileNumber = (sb1.Value + xIndex) + (tilesPerRow * yIndex);
                    if (tileNumber < TileManager.Epf[0].max)
                    {
                        graphics.DrawImage(ImageRenderer.Singleton.GetTileBitmap(tileNumber), tileRectangle);
                    }
                    
                    if (ShowGrid)
                    {
                        graphics.DrawRectangle(penGrid, tileRectangle);
                    }
                }
            }
            
            Pen penSelected = new Pen(Color.Red, 2);
            Pen penFocused = new Pen(Color.Green, 2);
            // Draw selected and focused after the grid so that they show up on top of grid-lines
            if (selectedTiles.Count > 0)
            {
            
                foreach (var selectedTile in selectedTiles)
                {
                    Rectangle tileRectangle = new Rectangle(selectedTile.X * sizeModifier, selectedTile.Y * sizeModifier, sizeModifier, sizeModifier);
                    graphics.DrawRectangle(penSelected, tileRectangle);
                }
            }

            if (!focusedTile.IsEmpty)
            {
                Rectangle tileRectangle = new Rectangle(focusedTile.X * sizeModifier, focusedTile.Y * sizeModifier, sizeModifier, sizeModifier);
                graphics.DrawRectangle(penFocused, tileRectangle);
            }
            
            BackgroundImage = tmpBitmap;
            penGrid.Dispose();
            penSelected.Dispose();
            penFocused.Dispose();
            statusStrip.Update();
            sb1.Update();
        }

        private void frmTile_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (sb1.Value - 1 >= sb1.Minimum) sb1.Value--;
            }
            else if (e.Delta < 0)
            {
                if (sb1.Value + 1 <= (sb1.Maximum - (Width / sizeModifier))) sb1.Value++;
            }

            sb1_Scroll(null, null); 
        }

        private void frmTile_MouseMove(object sender, MouseEventArgs e)
        {
            int xIndex = e.X / sizeModifier;
            int yIndex = e.Y / sizeModifier;
            updateFocusedTile(xIndex, yIndex);
        }
        
        private void updateFocusedTile(int xIndex, int yIndex){
            Point tileUnderMouse = new Point(xIndex, yIndex);
       
            if (!tileUnderMouse.Equals(focusedTile))
            {
                focusedTile = tileUnderMouse;
                
            }
            
            //Check for text chagne because scrolling will trigger a change in text without the 'tile' changing
            int tileNumber = (sb1.Value + xIndex) + (tilesPerRow * yIndex);
            string newText = $"Tile number: {tileNumber}";
            if(!newText.Equals(focusTileLabel.Text)){
                focusTileLabel.Text = newText; 
                //Immediate repaint required here, text doesn't seem to update until you focus the status strip otherwise
                Refresh(); 
            }
        }

        private void frmTile_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            int xIndex = e.X / sizeModifier;
            int yIndex = e.Y / sizeModifier;
            Point selectedTile = new Point(xIndex, yIndex);
            

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
             TileManager.TileSelection = NormalizeSelection();
             TileManager.LastSelection = TileManager.SelectionType.Tile;
             Invalidate();
        }

        public void AdjustSizeModifier(int newModifier)
        {
            sizeModifier = newModifier;
            updateRenderParams();
        }
        
        public Dictionary<Point, int> NormalizeSelection()
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
                int tileNumber = (sb1.Value + selectedTile.X) + (tilesPerRow * selectedTile.Y);
                dictionary.Add(new Point(selectedTile.X - xMin, selectedTile.Y - yMin),  tileNumber);
            }
        
            return dictionary;
        }

        private void findTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        NumberInputForm numberInputForm = new NumberInputForm(@"Enter Tile Number");
        if (numberInputForm.ShowDialog(this) == DialogResult.OK)
        {
            NavigateToTile(numberInputForm.Number);
        }
        }

        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowGrid = showGridToolStripMenuItem.Checked;
        }
        
        public void NavigateToTile(int tileNumber)
        {
            // usableHeight = Height - sb1.Height - menuStrip.Height - statusStrip.Height - pixelBuffer;
            // tileRows = usableHeight / sizeModifier;
            // tilesPerRow = TileManager.Epf[0].max / tileRows;
            // sb1.Maximum = tilesPerRow + (Width / sizeModifier);
            
            if (tileNumber < 0 || tileNumber> TileManager.Epf[0].max) return;
            //int tileNumber = (sb1.Value + xIndex) + (tilesPerRow * yIndex);
            int yIndex = tileNumber / tilesPerRow;
            int indexInRow = tileNumber - (tilesPerRow * yIndex);
            int viewableTilesPerRow = Width / sizeModifier;
            
            //Scroll to a 'window' instead of the direct indexInRow, this is a bit more of a natural interaction
            int scrollBarIndex = (indexInRow / viewableTilesPerRow) * viewableTilesPerRow;
            
            int xIndex = indexInRow - scrollBarIndex;
        
            sb1.Value = scrollBarIndex;
            selectedTiles.Clear();
            selectedTiles.Add(new Point(xIndex, yIndex));
            TileManager.TileSelection = NormalizeSelection();
            TileManager.LastSelection = TileManager.SelectionType.Tile;
            
        }

        private void FormTile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }

        }

        private void FormTile_SizeChanged(object sender, EventArgs e)
        {
            // Do not update the values when minimized, The will be incorrect
            if (WindowState != FormWindowState.Minimized)
            {
                updateRenderParams();
            }
        }

        private void FormTile_MouseLeave(object sender, EventArgs e)
        {
            focusedTile = Point.Empty;
            focusTileLabel.Text = null;
            Refresh();
        }
    }
}
