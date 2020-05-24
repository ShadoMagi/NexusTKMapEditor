using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MornaMapEditor
{
    public partial class FormCopySection : Form
    {
        private Point UpperLeft { get; set; }
        private Point LowerRight { get; set; }
        private bool CopyObjects { get; set; }
        private bool CopyTiles { get; set; }
        private bool CopyPass { get; set; }

        private readonly Map map;

        public FormCopySection(Map map, Point focusedTile)
        {
            InitializeComponent();
            this.map = map;
            
            numericUpDownLowerX.Maximum = numericUpDownUpperX.Maximum = map.Size.Width - 1;
            numericUpDownLowerY.Maximum = numericUpDownUpperY.Maximum = map.Size.Height - 1;
            numericUpDownUpperX.Value = (focusedTile.X < 0 ? 0 : focusedTile.X);
            numericUpDownUpperY.Value = (focusedTile.Y < 0 ? 0 : focusedTile.Y);
            numericUpDownLowerX.Value = map.Size.Width - 1;
            numericUpDownLowerY.Value = map.Size.Height - 1;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (numericUpDownLowerX.Value < numericUpDownUpperX.Value || numericUpDownLowerY.Value < numericUpDownUpperY.Value)
            {
                MessageBox.Show(@"Upper left and lower right tile are not correct", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpperLeft = new Point((int)numericUpDownUpperX.Value, (int)numericUpDownUpperY.Value);
            LowerRight = new Point((int)numericUpDownLowerX.Value, (int)numericUpDownLowerY.Value);
            CopyObjects = checkBoxCopyObjects.Checked;
            CopyTiles = checkBoxCopyTiles.Checked;
            CopyPass = checkBoxCopyPass.Checked;

            CopySelection();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CopySelection()
        {
            TileManager.ObjectSelection.Clear();
            TileManager.PassSelection.Clear();
            TileManager.TileSelection.Clear();

            TileManager.LastSelection = TileManager.SelectionType.None;
            if (!CopyObjects && !CopyTiles && !CopyPass) return;

            var selection = GetSelection();

            if (CopyObjects)
            {
                TileManager.LastSelection |= TileManager.SelectionType.Object;
                foreach (KeyValuePair<Point, Tile> keyValuePair in selection)
                    TileManager.ObjectSelection.Add(keyValuePair.Key, keyValuePair.Value.ObjectNumber);
            }
            if (CopyPass)
            {
                TileManager.LastSelection |= TileManager.SelectionType.Pass;
                foreach (KeyValuePair<Point, Tile> keyValuePair in selection)
                    TileManager.PassSelection.Add(keyValuePair.Key, keyValuePair.Value.Passable ? 0 : 1);
            }
            if (CopyTiles)
            {
                TileManager.LastSelection |= TileManager.SelectionType.Tile;
                foreach (KeyValuePair<Point, Tile> keyValuePair in selection)
                    TileManager.TileSelection.Add(keyValuePair.Key, keyValuePair.Value.TileNumber);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public Dictionary<Point, Tile> GetSelection()
        {
            Dictionary<Point, Tile> dictionary = new Dictionary<Point, Tile>();

            for (int x = UpperLeft.X; x <= LowerRight.X; x++)
            {
                for (int y = UpperLeft.Y; y <= LowerRight.Y; y++)
                {
                    dictionary.Add(new Point(x - UpperLeft.X, y - UpperLeft.Y), map[x, y] ?? Tile.DefaultTile);
                }
            }

            return dictionary;
        }

    }
}
