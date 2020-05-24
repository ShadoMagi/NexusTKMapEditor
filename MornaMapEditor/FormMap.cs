using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MornaMapEditor.MapActions;

namespace MornaMapEditor
{
    public sealed partial class FormMap : Form
    {
        public FormMinimap MinimapWindow { get; private set; }
        public bool IsMinimapVisible { get; private set; }

        private Bitmap bitmap;
        private Bitmap mapBitmap;
        private Bitmap tileBitmap;
        private Bitmap objectBitmap;
        private static Thread renderThread;
        private static int untitledMapIndex;
        private bool mapBitmapIsReady;
        private bool showTiles, showObjects;
        private bool isMouseDown;
        private bool copy;
        private int sizeModifier;
        private const int initialWidth = 17, initialHeight = 15;
        private Point focusedTile = new Point(-1, -1);
        private Point copyStartTile = new Point(-1, -1);
        private Map activeMap;
        private string activeMapPath;
        private int xMinFill;
        private int xMaxFill;
        private int yMinFill;
        private int yMaxFill;
        private int saveCheck = 0;

        private readonly LinkedList<IMapAction> mapUndoActions = new LinkedList<IMapAction>();
        private readonly LinkedList<IMapAction> mapRedoActions = new LinkedList<IMapAction>();

        private bool showGrid;
        public bool ShowGrid
        {
            get { return showGrid; }
            set { showGrid = value; pnlImage.Invalidate(); }
        }

        private Point lastPassToggled = new Point(-1, -1);
        private bool showPass;
        public bool ShowPass
        {
            get { return showPass; }
            set { showPass = value; pnlImage.Invalidate(); }
        }

        public FormMap(Form mdiParent)
        {
            InitializeComponent();
            MdiParent = mdiParent;
            MinimapWindow = new FormMinimap { MdiParent = mdiParent };
            MinimapWindow.Location = new Point(Parent.Width - 280, 20);

            menuStrip.Visible = false;
            //pnlImage.Paint += pnlImage_Paint;

            showMinimapToolStripMenuItem.PerformClick();
            resizeWindowToDefaultToolStripMenuItem.PerformClick();
            showTilesToolStripMenuItem.Checked = true;
            showTiles = showTilesToolStripMenuItem.Checked;
            showObjectsToolStripMenuItem.Checked = true;
            showObjects = showObjectsToolStripMenuItem.Checked;

            Reload(false);
            CreateNewMapCore(initialWidth, initialHeight);

            MinimapWindow.FormClosing += MinimapWindow_FormClosing;
            MinimapWindow.SelectionChanged += MinimapWindow_SelectionChanged;
            this.LostFocus += new EventHandler(FormMap_LostFocus);
            this.MouseWheel += new MouseEventHandler(FormMap_MouseWheel);
            panel1.Scroll += (s, e) => UpdateMinimap(false, false);
            Resize += (s, e) => UpdateMinimap(false, false);
        }

        private void MinimapWindow_SelectionChanged(Point point)
        {
            panel1.AutoScrollPosition = point;
            UpdateMinimap(false, false);
        }

        private void MinimapWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            showMinimapToolStripMenuItem.PerformClick();
            e.Cancel = true;
        }

        private void SetImage(Image image)
        {
            if (pnlImage.Image != null) pnlImage.Image.Dispose();
            pnlImage.Image = image != null ? new Bitmap(image, pnlImage.Width, pnlImage.Height) : null;
        }

        #region Form event handlers

        private void pnlImage_Paint(object sender, PaintEventArgs e)
        {
            //OnPaint(e);

            if (ShowGrid)
            {
                Pen penGrid = new Pen(Color.LightCyan, 1);
                for (int i = 0; i < activeMap.Size.Width; i++)
                {
                    for (int j = 0; j < activeMap.Size.Height; j++)
                    {
                        e.Graphics.DrawRectangle(penGrid, i * sizeModifier, j * sizeModifier, sizeModifier, sizeModifier);
                    }
                }
                penGrid.Dispose();
            }

            if (ShowPass)
            {
                Pen penRed = new Pen(Color.Red, 2);
                Pen penGreen = new Pen(Color.Green, 2);
                for (int i = 0; i < activeMap.Size.Width; i++)
                {
                    for (int j = 0; j < activeMap.Size.Height; j++)
                    {
                        Tile mapTile = activeMap[i, j];
                        e.Graphics.DrawRectangle((mapTile == null || mapTile.Passable) ? penRed : penGreen, i * sizeModifier + (sizeModifier * 5 / 12), j * sizeModifier + (sizeModifier * 5 / 12), (sizeModifier * 1 / 6), (sizeModifier * 1 / 6));
                    }
                }
                penGreen.Dispose();
                penRed.Dispose();
            }

            if (focusedTile.X >= 0 && focusedTile.Y >= 0 &&
                focusedTile.X < activeMap.Size.Width && focusedTile.Y < activeMap.Size.Height && copy)
            {
                Pen pen = new Pen(Color.Yellow, 2);
                if (copy) pen = new Pen(Color.Yellow, 2);
                e.Graphics.DrawRectangle(pen, copyStartTile.X * sizeModifier, copyStartTile.Y * sizeModifier, sizeModifier + sizeModifier * (focusedTile.X - copyStartTile.X), sizeModifier + sizeModifier * (focusedTile.Y - copyStartTile.Y));
                pen.Dispose();
            }

            else if (focusedTile.X >= 0 && focusedTile.Y >= 0 &&
                focusedTile.X < activeMap.Size.Width && focusedTile.Y < activeMap.Size.Height)
            {
                Pen pen = new Pen(Color.Green, 2);
                if (copy) pen = new Pen(Color.Yellow, 2);
                e.Graphics.DrawRectangle(pen, focusedTile.X * sizeModifier, focusedTile.Y * sizeModifier, sizeModifier, sizeModifier);
                pen.Dispose();
            }


            if (pnlImage.Image == null && mapBitmapIsReady)
            {
                pnlImage.Image = mapBitmap;
                pnlImage.Invalidate();
                UpdateMinimap(true, true);
            }


        }

        private void FormMap_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (ModifierKeys == Keys.Control)
                {
                    if (panel1.VerticalScroll.Value - 45 >= 0)
                    {
                        panel1.VerticalScroll.Value -= 45;
                    }
                    else
                    {
                        panel1.VerticalScroll.Value = 0;
                    }
                }
                else
                {
                    if (panel1.HorizontalScroll.Value - 45 >= 0)
                    {
                        panel1.HorizontalScroll.Value -= 45;
                    }
                    else
                    {
                        panel1.HorizontalScroll.Value = 0;
                    }
                }
            }
            else if (e.Delta < 0)
            {
                if (ModifierKeys == Keys.Control)
                {
                    if (panel1.VerticalScroll.Value + 45 <= panel1.VerticalScroll.Maximum)
                    {
                        panel1.VerticalScroll.Value += 45;
                    }
                    else
                    {
                        panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
                    }
                }
                else
                {
                    if (panel1.HorizontalScroll.Value + 45 <= panel1.HorizontalScroll.Maximum)
                    {
                        panel1.HorizontalScroll.Value += 45;
                    }
                    else
                    {
                        panel1.HorizontalScroll.Value = panel1.HorizontalScroll.Maximum;
                    }
                }
            }

            UpdateMinimap(false, false);
        }

        private void pnlImage_MouseMove(object sender, MouseEventArgs e)
        {
            int newFocusedTileX = e.X / sizeModifier;
            int newFocusedTileY = e.Y / sizeModifier;

            if (newFocusedTileX >= activeMap.Size.Width || newFocusedTileY >= activeMap.Size.Height)
            {
                toolStripStatusLabel.Text = @"Outside of map";
            }

            bool refresh = (newFocusedTileX != focusedTile.X || newFocusedTileY != focusedTile.Y);

            if (refresh)
            {
                // Paint-like painting
                if (isMouseDown) pnlImage_MouseDown(sender, e);

                focusedTile = new Point(newFocusedTileX, newFocusedTileY);
                pnlImage.Refresh();

                Tile mapTile = activeMap[newFocusedTileX, newFocusedTileY];

                string message = string.Format("Focused tile: ({0}, {1})", newFocusedTileX, newFocusedTileY);
                if (mapTile == null) message += "    Pass = False";
                else message += string.Format("Tile number: {0}    Object number: {1}    Pass: {2}", mapTile.TileNumber, mapTile.ObjectNumber, !mapTile.Passable);

                toolStripStatusLabel.Text = message;
            }
        }

        private void pnlImage_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && TileManager.TileSelection.Count == 1 && activeMap.IsEditable && ModifierKeys == Keys.Control)
            {
                DialogResult result = MessageBox.Show("Would you like to fill the entire map with tile " + TileManager.TileSelection[new Point(0, 0)] + "?", "Tile Fill", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {

                    for (int x = 0; x < activeMap.Size.Width; x++)
                    {
                        for (int y = 0; y < activeMap.Size.Height; y++)
                        {
                            activeMap[x, y] = activeMap[x, y] ?? Tile.DefaultTile;
                            AddMapAction(new MapActionPasteTile(new Point(x, y), activeMap[x, y].TileNumber, TileManager.TileSelection[new Point(0, 0)]));
                            activeMap[x, y].TileNumber = TileManager.TileSelection[new Point(0, 0)];
                        }
                    }

                    SetImage(null);
                    RenderMap();

                }
            }

            else if (e.Button == MouseButtons.Right && TileManager.TileSelection.Count == 1 && activeMap.IsEditable)
            {
                DialogResult result = MessageBox.Show("Would you like to fill this area with tile " + TileManager.TileSelection[new Point(0, 0)] + "?", "Tile Fill", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {

                    int tileX = e.X / sizeModifier;
                    int tileY = e.Y / sizeModifier;

                    xMinFill = tileX - 25;
                    xMaxFill = tileX + 25;
                    yMinFill = tileY - 25;
                    yMaxFill = tileY + 25;
                    
                    if (xMinFill < 0) xMinFill = 0;
                    if (xMaxFill >= activeMap.Size.Width) xMaxFill = activeMap.Size.Width;
                    if (yMinFill < 0) yMinFill = 0;
                    if (yMaxFill >= activeMap.Size.Height) yMaxFill = activeMap.Size.Height;
                    
                    activeMap[tileX, tileY] = activeMap[tileX, tileY] ?? Tile.DefaultTile;

                    if (activeMap[tileX, tileY].TileNumber != TileManager.TileSelection[new Point(0, 0)])
                    {
                        floodFill(tileX, tileY, activeMap[tileX, tileY].TileNumber, TileManager.TileSelection[new Point(0, 0)]);
                    }
                    //SetImage(null);
                    //RenderMap();
                }

            }


        }

        private void pnlImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (ModifierKeys == Keys.Control || ModifierKeys == Keys.Alt || ModifierKeys == Keys.Shift)
            {
                copyStartTile.X = e.X / sizeModifier;
                copyStartTile.Y = e.Y / sizeModifier;
                copy = true;
                return;
            }

            if (!activeMap.IsEditable)
            {
                MessageBox.Show(@"You can enable editability in the Edit menu or by pressing Ctrl+E.",
                                @"Map not editable.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            isMouseDown = true;

            int tileX = e.X / sizeModifier;
            int tileY = e.Y / sizeModifier;

            if (ShowPass)
                TogglePass(tileX, tileY);
            else
            {
                if ((TileManager.LastSelection & TileManager.SelectionType.Tile) == TileManager.SelectionType.Tile)
                    Paste(tileX, tileY, TileManager.SelectionType.Tile);
                if ((TileManager.LastSelection & TileManager.SelectionType.Pass) == TileManager.SelectionType.Pass)
                    Paste(tileX, tileY, TileManager.SelectionType.Pass);
                if ((TileManager.LastSelection & TileManager.SelectionType.Object) == TileManager.SelectionType.Object)
                    Paste(tileX, tileY, TileManager.SelectionType.Object);
            }

        }

        private void pnlImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            
            isMouseDown = false;
            copy = false;

            if (copyStartTile != new Point(-1, -1))
            {
                toolStripStatusLabel.Text = string.Format("Upper Left: ({0}, {1}) Lower Right: ({2}, {3})", copyStartTile.X, copyStartTile.Y, focusedTile.X, focusedTile.Y);
                
                if (ModifierKeys == Keys.Alt)
                {
                    CopySelection(copyStartTile, focusedTile, true, false);
                }
                else if (ModifierKeys == Keys.Control)
                {
                    CopySelection(copyStartTile, focusedTile, false, true);
                }
                else if (ModifierKeys == Keys.Shift)
                {
                    CopySelection(copyStartTile, focusedTile, true, true);
                }
            }

            copyStartTile = new Point(-1, -1);
            UpdateMinimap(true, false);
        }

        private void FormMap_LostFocus(object sender, EventArgs e)
        {
            if (ActiveForm is FormMap)
                this.MinimapWindow.Visible = false;
        }

        private void FormMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = SaveCheck();
            if (dialogResult == DialogResult.Cancel) e.Cancel = true;
            else
            {
                MinimapWindow.FormClosing -= MinimapWindow_FormClosing;
                MinimapWindow.SelectionChanged -= MinimapWindow_SelectionChanged;
                MinimapWindow.SetImage(null);
                SetImage(null);
                MinimapWindow.Close();
                GC.Collect();
            }
        }

        #endregion

        #region Menu click event handlers

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = SaveCheck();
            if (dialogResult == DialogResult.Cancel) return;
            CreateNewMap();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = SaveCheck();
            if (dialogResult == DialogResult.Cancel) return;
            OpenExistingMap();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveCheck = 0;
            if (string.IsNullOrEmpty(activeMapPath)) saveAsToolStripMenuItem.PerformClick();
            if (string.IsNullOrEmpty(activeMapPath)) return;

            try
            {
                // Extension might me .mape -> must be .map
                if (activeMapPath.ToLower().EndsWith(".mape"))
                    activeMapPath = activeMapPath.Remove(activeMapPath.Length - 1);
                activeMap.Save(activeMapPath);
                Text = string.Format(@"Map [{0}]", activeMap.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error saving map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = @"Compressed Tile NexusTK Map Files|*.cmp|NexusTK Map Files|*.map", FileName = activeMap.Name };
            var dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                activeMapPath = saveFileDialog.FileName;
                saveToolStripMenuItem.PerformClick();
            }
        }

        private void savePngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = @"PNG|*.png", FileName = activeMap.Name };
            var dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    pnlImage.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
                    Process.Start(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mapUndoActions.Count == 0) return;
            IMapAction lastAction = mapUndoActions.Last.Value;
            mapUndoActions.RemoveLast();

            mapRedoActions.AddLast(lastAction); // to be able to redo what has been undone
            lastAction.Undo(activeMap);
            RefreshMapActionUI(lastAction);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mapRedoActions.Count == 0) return;
            IMapAction lastAction = mapRedoActions.Last.Value;
            mapRedoActions.RemoveLast();

            mapUndoActions.AddLast(lastAction); // to be able to undo what has been redone
            lastAction.Redo(activeMap);
            RefreshMapActionUI(lastAction);
        }

        private void editableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activeMap.IsEditable = editableToolStripMenuItem.Checked;
        }

        private void resizeMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!activeMap.IsEditable)
            {
                MessageBox.Show(@"You can enable editability in the Edit menu or by pressing Ctrl+E.",
                                @"Map not editable.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var mapSizeDialog = new MapSizeDialog();
            DialogResult dialogResult = mapSizeDialog.ShowDialog(this);
            if (dialogResult == DialogResult.Cancel) return;

            AddMapAction(new MapActionResize(activeMap.Size, mapSizeDialog.MapSize));

            activeMap.Size = mapSizeDialog.MapSize;
            activeMap.IsModified = true;
            SetImage(null);
            RenderMap();
            //UpdateMinimap(true, true);
        }

        private void copySectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormCopySection(activeMap, focusedTile).ShowDialog();
        }

        private void showMinimapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IsMinimapVisible = showMinimapToolStripMenuItem.Checked;
            MinimapWindow.Visible = IsMinimapVisible;
            Focus();
        }

        private void showTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showTiles = showTilesToolStripMenuItem.Checked;
            SetImage(null);
            RenderMap();
            //UpdateMinimap(true, false);
        }

        private void showObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showObjects = showObjectsToolStripMenuItem.Checked;
            SetImage(null);
            RenderMap();
            //UpdateMinimap(true, false);
        }

        private void showPassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPass = showPassToolStripMenuItem.Checked;
        }

        private void showGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowGrid = showGridToolStripMenuItem.Checked;
        }

        private void resizeWindowToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClientSizeCore(initialWidth * sizeModifier, initialHeight * sizeModifier + 22);
        }

        #endregion

        #region Helper methods

        private DialogResult SaveCheck()
        {
            if (!activeMap.IsModified) return DialogResult.OK;

            DialogResult dialogResult = MessageBox.Show(
                string.Format(@"Do you want to save changes to the current map ({0})?", activeMap.Name ?? string.Empty),
                @"Save changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);

            if (dialogResult == DialogResult.Yes) saveToolStripMenuItem.PerformClick();
            return dialogResult;
        }

        private void CreateNewMap()
        {
            var mapSizeDialog = new MapSizeDialog();
            DialogResult dialogResult = mapSizeDialog.ShowDialog(this);
            if (dialogResult == DialogResult.Cancel) return;

            CreateNewMapCore(mapSizeDialog.MapSize.Width, mapSizeDialog.MapSize.Height);
        }

        private void CreateNewMapCore(int width, int height)
        {
            activeMap = new Map(width, height);
            string mapName = "UntitledMap" + untitledMapIndex++;
            activeMap.Name = mapName;
            Text = string.Format(@"Map [{0}]", mapName);
            SetImage(null);
            editableToolStripMenuItem.Checked = activeMap.IsEditable;
            RenderMap();
            mapUndoActions.Clear();
            mapRedoActions.Clear();
            //UpdateMinimap(true, true);
        }

        private void OpenExistingMap()
        {

            string filter = @"Compressed Tile NexusTK Map Files|*.cmp|NexusTK Map Files|*.map";
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = filter };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                activeMapPath = openFileDialog.FileName;
                if (!File.Exists(activeMapPath)) return;

                try
                {
                    activeMap = new Map(activeMapPath);
                    Text = string.Format("Map [{0}]", activeMap.Name);
                    SetImage(null);
                    editableToolStripMenuItem.Checked = activeMap.IsEditable;
                    RenderMap();
                    mapUndoActions.Clear();
                    mapRedoActions.Clear();
                    //UpdateMinimap(true, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"Error loading map", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void UpdateMinimap(bool assignImage, bool resizeMinimapWindow)
        {
            if (resizeMinimapWindow)
            {
                int imgWidth = activeMap.Size.Width * sizeModifier;
                int imgHeight = activeMap.Size.Height * sizeModifier;
                if ((float)imgWidth / imgHeight > 1.0f)
                {
                    MinimapWindow.Width = 250;
                    MinimapWindow.Height = (int)(250 * ((float)imgHeight / imgWidth)) + 20;
                }
                else
                {
                    MinimapWindow.Height = 250 + 20;
                    MinimapWindow.Width = (int)(250 * ((float)imgWidth / imgHeight));
                }
            }
            if (assignImage) MinimapWindow.SetImage(pnlImage.Image);
            MinimapWindow.SetPositionData(panel1.Size, activeMap.Size, panel1.AutoScrollPosition);
        }

        private void CopySelection(Point UpperLeft, Point LowerRight, bool CopyObjects, bool CopyTiles)
        {
            TileManager.ObjectSelection.Clear();
            TileManager.PassSelection.Clear();
            TileManager.TileSelection.Clear();

            TileManager.LastSelection = TileManager.SelectionType.None;


            var selection = GetSelection(UpperLeft, LowerRight);

            if (CopyObjects)
            {
                TileManager.LastSelection |= TileManager.SelectionType.Object;
                foreach (KeyValuePair<Point, Tile> keyValuePair in selection)
                    TileManager.ObjectSelection.Add(keyValuePair.Key, keyValuePair.Value.ObjectNumber);
            }

            if (CopyTiles)
            {
                TileManager.LastSelection |= TileManager.SelectionType.Tile;
                foreach (KeyValuePair<Point, Tile> keyValuePair in selection)
                    TileManager.TileSelection.Add(keyValuePair.Key, keyValuePair.Value.TileNumber);
            }
        }

        public Dictionary<Point, Tile> GetSelection(Point UpperLeft, Point LowerRight)
        {
            Dictionary<Point, Tile> dictionary = new Dictionary<Point, Tile>();

            for (int x = UpperLeft.X; x <= LowerRight.X; x++)
            {
                for (int y = UpperLeft.Y; y <= LowerRight.Y; y++)
                {
                    dictionary.Add(new Point(x - UpperLeft.X, y - UpperLeft.Y), activeMap[x, y] ?? Tile.DefaultTile);
                }
            }

            return dictionary;
        }

        private void replaceTile(Tile oldTile, Tile newTile)
        {
            TileManager.SelectionType pasteSelection = new TileManager.SelectionType();

            for (int x = 0; x <= activeMap.Size.Width; x++)
            {
                for (int y = 0; y <= activeMap.Size.Width; y++)
                {
                    if ((activeMap[x, y] ?? Tile.DefaultTile) == oldTile)
                    {
                        Paste(x, y, pasteSelection);
                    }
                }
            }

        }

        private void Paste(int tileX, int tileY, TileManager.SelectionType selectionType)
        {
            Dictionary<Point, int> selection;
            if (selectionType == TileManager.SelectionType.Object) selection = TileManager.ObjectSelection;
            else if (selectionType == TileManager.SelectionType.Pass) selection = TileManager.PassSelection;
            else if (selectionType == TileManager.SelectionType.Tile) selection = TileManager.TileSelection;
            else return;

            foreach (KeyValuePair<Point, int> keyValuePair in selection)
            {
                int mapTileX = keyValuePair.Key.X + tileX;
                int mapTileY = keyValuePair.Key.Y + tileY;

                if (mapTileX < activeMap.Size.Width && mapTileY < activeMap.Size.Height)
                {
                    activeMap[mapTileX, mapTileY] = activeMap[mapTileX, mapTileY] ?? Tile.DefaultTile;

                    // Go to next step of the loop if old value equals new value
                    int oldValue;
                    if (selectionType == TileManager.SelectionType.Object) oldValue = activeMap[mapTileX, mapTileY].ObjectNumber;
                    else if (selectionType == TileManager.SelectionType.Pass) oldValue = activeMap[mapTileX, mapTileY].Passable ? 0 : 1;
                    else oldValue = activeMap[mapTileX, mapTileY].TileNumber;

                    if (oldValue == keyValuePair.Value) continue;

                    // Paste new value
                    Point point = new Point(mapTileX, mapTileY);
                    activeMap.IsModified = true;

                    if (selectionType == TileManager.SelectionType.Object)
                    {
                        AddMapAction(new MapActionPasteObject(point, activeMap[mapTileX, mapTileY].ObjectNumber, keyValuePair.Value));
                        activeMap[mapTileX, mapTileY].ObjectNumber = keyValuePair.Value;
                    }

                    if (selectionType == TileManager.SelectionType.Pass)
                    {
                        AddMapAction(new MapActionPastePass(point, keyValuePair.Value));
                        activeMap[mapTileX, mapTileY].Passable = (keyValuePair.Value == 0 ? true : false);
                    }

                    if (selectionType == TileManager.SelectionType.Tile)
                    {
                        AddMapAction(new MapActionPasteTile(point, activeMap[mapTileX, mapTileY].TileNumber, keyValuePair.Value));
                        activeMap[mapTileX, mapTileY].TileNumber = keyValuePair.Value;
                    }

                    if (selectionType == TileManager.SelectionType.Object)
                    {
                        for (int i = 0; i < 12; i++)
                        {
                            if (mapTileY - i >= 0) RenderSingleMapTile(mapTileX, mapTileY - i, activeMap[mapTileX, mapTileY].ObjectNumber == 0);
                        }
                        pnlImage.Invalidate();
                    }
                    else
                    {
                        RenderSingleMapTile(mapTileX, mapTileY, activeMap[mapTileX, mapTileY].TileNumber == 0);
                        pnlImage.Invalidate();
                    }
                }
            }
        }

        private void TogglePass(int tileX, int tileY)
        {
            Point passTile = new Point(tileX, tileY);
            if (lastPassToggled == passTile) return;
            lastPassToggled = passTile;

            if (activeMap[tileX, tileY] == null) activeMap[tileX, tileY] = new Tile(0, false, 0);
            else activeMap[tileX, tileY].Passable = !activeMap[tileX, tileY].Passable;

            activeMap.IsModified = true;
            AddMapAction(new MapActionPastePass(new Point(tileX, tileY), (activeMap[tileX, tileY].Passable ? 0 : 1)));
            pnlImage.Invalidate();
            //RenderSingleMapTile(tileX, tileY, activeMap[tileX, tileY].TileNumber == 0);
        }

        public void floodFill(int fillX, int fillY, int findTile, int replaceTile)
        {
            activeMap[fillX, fillY] = activeMap[fillX, fillY] ?? Tile.DefaultTile;
            if (activeMap[fillX, fillY].TileNumber != findTile) return;

            Paste(fillX, fillY, TileManager.SelectionType.Tile);


            if (fillX + 1 < xMaxFill) { floodFill(fillX + 1, fillY, findTile, replaceTile); }
            if (fillX - 1 >= xMinFill) { floodFill(fillX - 1, fillY, findTile, replaceTile); }
            if (fillY + 1 < yMaxFill) { floodFill(fillX, fillY + 1, findTile, replaceTile); }
            if (fillY - 1 >= yMinFill) { floodFill(fillX, fillY - 1, findTile, replaceTile); }
            return;

        }

        private void AddMapAction(IMapAction mapAction)
        {
            if (mapUndoActions.Count > 1000) for (int i = 0; i < 100; i++) mapUndoActions.RemoveFirst();
            mapUndoActions.AddLast(mapAction);
            mapRedoActions.Clear();
        }

        public void Reload(bool render)
        {
            sizeModifier = ImageRenderer.Singleton.sizeModifier;
            SetImage(null);
            resizeWindowToDefaultToolStripMenuItem.PerformClick();
            if (render) RenderMap();
        }

        private void tmrSave_Tick(object sender, EventArgs e)
        {
            if (autoSaveToolStripMenuItem.Checked) CheckSave();
        }

        private void CheckSave()
        {
            saveCheck++;

            if (string.IsNullOrEmpty(activeMapPath) && saveCheck == 15) saveCheck = 0;
            else if (saveCheck == 15) saveToolStripMenuItem.PerformClick();
        }

        #endregion

        #region Rendering

        private void RefreshMapActionUI(IMapAction lastAction)
        {
            activeMap.IsModified = true;

            if (lastAction is MapActionResize)
            {
                SetImage(null);
                RenderMap();
                //UpdateMinimap(true, true);
            }
            if (lastAction is MapActionPasteTile || lastAction is MapActionPastePass)
            {
                Tile tile = activeMap[lastAction.Tile.X, lastAction.Tile.Y];
                RenderSingleMapTile(lastAction.Tile.X, lastAction.Tile.Y, tile == null || tile.TileNumber == 0);
                pnlImage.Invalidate();
                UpdateMinimap(true, false);
            }
            else if (lastAction is MapActionPasteObject)
            {
                Tile tile = activeMap[lastAction.Tile.X, lastAction.Tile.Y];
                for (int i = 0; i < 12; i++)
                {
                    if (lastAction.Tile.Y - i >= 0)
                        RenderSingleMapTile(lastAction.Tile.X, lastAction.Tile.Y - i,
                                            tile == null || tile.ObjectNumber == 0);
                }
                pnlImage.Invalidate();
                UpdateMinimap(true, false);
            }
        }

        private void RenderMap()
        {
            //Bitmap mapBitmap;
            if (pnlImage.Image == null)
            {
                int imageWidth = activeMap.Size.Width * sizeModifier;
                int imageHeight = activeMap.Size.Height * sizeModifier;
                mapBitmap = new Bitmap(imageWidth, imageHeight);
            }
            //else mapBitmap = (Bitmap) pnlImage.Image;

            if (!showObjects && !showTiles)
            {
                mapBitmap = new Bitmap(activeMap.Size.Width * sizeModifier, activeMap.Size.Height * sizeModifier);
                pnlImage.Image = mapBitmap;
                return;
            }

            renderThread = new Thread(new ThreadStart(RenderLoop));
            renderThread.Start();
            renderThread.Join();
            //ThreadPool.QueueUserWorkItem(o => RenderLoop());
            //for (int x = 0; x < activeMap.Size.Width; x++)
            //{
            //    for (int y = 0; y < activeMap.Size.Height; y++)
            //    {
            //        // Debug.WriteLine("x,y:  " + x + "," + y); // testing
            //        new Thread(o => RenderSingleMapTile(x, y, g, false)).Start();
            //    }
            //}

            //g.Dispose();
            //panel1.AutoScrollMinSize = pnlImage.Image.Size;
            //pnlImage.Invalidate();
            //picMap.Image = mapBitmap;
            //Application.DoEvents();
            //tSet.Dispose();
        }

        private void RenderLoop()
        {
            mapBitmapIsReady = false;
            Graphics g = Graphics.FromImage(mapBitmap);
            g.Clear(Color.DarkGreen);

            for (int x = 0; x < activeMap.Size.Width; x++)
            {
                for (int y = 0; y < activeMap.Size.Height; y++)
                {
                    RenderSingleMapTile(x, y, g, false);
                }
            }

            g.Dispose();
            mapBitmapIsReady = true;
            pnlImage.Invalidate();
            MinimapWindow.pnlImage.Invalidate();

        }

        private void RenderSingleMapTile(int x, int y, bool forceRenderEmpty)
        {
            //Image mapImage = picMap.Image;
            Graphics g = Graphics.FromImage(pnlImage.Image);
            RenderSingleMapTile(x, y, g, forceRenderEmpty);
            g.Dispose();
            //picMap.Image = mapImage;
            //picMap.Refresh();
        }

        private void RenderSingleMapTile(int x, int y, Graphics g, bool forceRenderEmpty)
        {
            tileBitmap = GetTileBitmap(x, y);
            objectBitmap = GetObjectBitmap(x, y);
            if (forceRenderEmpty)
            {
                Bitmap bitmapClear = new Bitmap(sizeModifier, sizeModifier);
                Graphics gClear = Graphics.FromImage(bitmapClear);
                gClear.Clear(Color.DarkGreen);

                if (objectBitmap == null)
                {
                    if (tileBitmap == null) objectBitmap = bitmapClear;
                    if (tileBitmap != null) objectBitmap = new Bitmap(sizeModifier, sizeModifier);
                }
                if (tileBitmap == null) tileBitmap = bitmapClear;
                gClear.Dispose();
                //bitmapClear.Dispose();
            }

            // Only tile
            if (showTiles && !showObjects && tileBitmap != null)
                g.DrawImage(tileBitmap, x * sizeModifier, y * sizeModifier);//, 36, 36);

            // Only object
            else if (!showTiles && showObjects && objectBitmap != null)
                g.DrawImage(objectBitmap, x * sizeModifier, y * sizeModifier);//, 36, 36);

            // Both
            else if (showTiles && showObjects)
            {
                if (objectBitmap == null && tileBitmap != null) g.DrawImage(tileBitmap, x * sizeModifier, y * sizeModifier);//, 36, 36);
                if (objectBitmap != null && tileBitmap == null) g.DrawImage(objectBitmap, x * sizeModifier, y * sizeModifier);//, 36, 36);
                if (objectBitmap != null && tileBitmap != null)
                {
                    Graphics tileGraphics = Graphics.FromImage(tileBitmap);
                    tileGraphics.DrawImage(objectBitmap, 0, 0);
                    //tileGraphics.Dispose();
                    g.DrawImage(tileBitmap, x * sizeModifier, y * sizeModifier);//, 36, 36);
                    tileGraphics.Dispose();
                }
            }
            //if (tileBitmap != null) tileBitmap.Dispose();
            //if (objectBitmap != null) objectBitmap.Dispose();
        }

        private Bitmap GetTileBitmap(int x, int y)
        {
            if (!showTiles) return null;

            bitmap = new Bitmap(sizeModifier, sizeModifier);

            Tile mapTile = activeMap[x, y];
            if (mapTile == null || mapTile.TileNumber <= 0) return null;
            if (mapTile.TileNumber >= TileManager.Epf[0].max) return null;

            Graphics graphics = Graphics.FromImage(bitmap);
            Bitmap tmpBitmap = ImageRenderer.Singleton.GetTileBitmap(mapTile.TileNumber);
            graphics.DrawImage(tmpBitmap, 0, 0);
            graphics.Dispose();
            return bitmap;
        }

        private Bitmap GetObjectBitmap(int x, int y)
        {
            if (!showObjects) return null;

            bitmap = new Bitmap(sizeModifier, sizeModifier);
            Graphics graphics = Graphics.FromImage(bitmap);
            if (tileBitmap == null) graphics.Clear(Color.DarkGreen);

            for (int i = 0; i < 12; i++)
            {
                if ((i + y) >= activeMap.Size.Height) break;

                Tile mapTile = activeMap[x, y + i];
                if (mapTile == null || mapTile.ObjectNumber == 0) continue;

                int objectNumber = mapTile.ObjectNumber;
                if (objectNumber < 0 || objectNumber >= TileManager.ObjectInfos.Length) continue;

                int objectHeight = TileManager.ObjectInfos[objectNumber].Indices.Length;
                if (objectHeight <= i) continue;

                int tile = TileManager.ObjectInfos[objectNumber].Indices[objectHeight - i - 1];
                if (bitmap == null) bitmap = ImageRenderer.Singleton.GetObjectBitmap(tile);
                else
                {
                    //Graphics graphics = Graphics.FromImage(bitmap);
                    Bitmap tmpBitmap = ImageRenderer.Singleton.GetObjectBitmap(tile);
                    graphics.DrawImage(tmpBitmap, 0, 0);
                    //tmpBitmap.Dispose();
                    //graphics.Dispose();
                }
            }

            graphics.Dispose();
            return bitmap;
        }

        #endregion
    }
}
