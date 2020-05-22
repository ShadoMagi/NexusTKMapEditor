using System;
using System.Drawing;
using System.Windows.Forms;

namespace Aesir5
{
    public partial class FormMinimap : Form
    {
        private Size mapWindowSize, mapSize;
        private Point autoScrollPosition;
        private bool isMouseDown;
        private Point oldLocation;
        private readonly Pen pen;

        public event Action<Point> SelectionChanged;

        public FormMinimap()
        {
            InitializeComponent();
            pen = new Pen(Color.Green, 2);
            //pictureBoxMinimap.Paint += pictureBoxMinimap_Paint;
        }

        public void SetImage(Image image)
        {
            if (image != null)
            {
                pnlImage.Image = image;
            }
            else
            {
                pnlImage.Image = new Bitmap(pnlImage.Width, pnlImage.Height);
            }
            //if (pnlImage.Image != null) pnlImage.Image.Dispose();
            //pnlImage.Image = image != null ? new Bitmap(image, pnlImage.Width, pnlImage.Height) : null;
        }

        public void SetPositionData(Size map_WindowSize, Size map_Size, Point auto_ScrollPosition)
        {
            mapWindowSize = map_WindowSize;
            mapSize = map_Size;
            autoScrollPosition = auto_ScrollPosition;
            //pictureBoxMinimap.Invalidate();
        }

        void pnlImage_Paint(object sender, PaintEventArgs e)
        {
            //OnPaint(e);

            int mapWidthInPixels = mapSize.Width * ImageRenderer.SizeModifier;
            int mapHeightInPixels = mapSize.Height * ImageRenderer.SizeModifier;

            int rectWidth = (int)(pnlImage.Width * (mapWindowSize.Width / (float)mapWidthInPixels));
            if (rectWidth > pnlImage.Width) rectWidth = pnlImage.Width;

            int rectHeight = (int)(pnlImage.Height * (mapWindowSize.Height / (float)mapHeightInPixels));
            if (rectHeight > pnlImage.Height) rectHeight = pnlImage.Height;

            int centerX = (int)(pnlImage.Width * (((float)mapWindowSize.Width / 2 - autoScrollPosition.X) / mapWidthInPixels));
            if (centerX + rectWidth / 2 > pnlImage.Width) centerX = pnlImage.Width - rectWidth / 2;

            int centerY = (int)(pnlImage.Height * (((float)mapWindowSize.Height / 2 - autoScrollPosition.Y) / mapHeightInPixels));
            if (centerY + rectHeight / 2 > pnlImage.Height) centerY = pnlImage.Height - rectHeight / 2;

            e.Graphics.DrawRectangle(pen, centerX - rectWidth / 2, centerY - rectHeight / 2, rectWidth, rectHeight);
        }

        private void pnlImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (oldLocation == e.Location) return;
            oldLocation = e.Location;
            isMouseDown = true;

            int mapWidthInPixels = mapSize.Width * ImageRenderer.SizeModifier;
            int mapHeightInPixels = mapSize.Height * ImageRenderer.SizeModifier;

            int mapX = (int)(mapWidthInPixels * ((float)e.X / pnlImage.Width));
            int mapY = (int)(mapHeightInPixels * ((float)e.Y / pnlImage.Height));

            if (mapX < mapWindowSize.Width && mapX < mapWidthInPixels && mapWidthInPixels < mapWindowSize.Width &&
                mapY < mapWindowSize.Height && mapY < mapHeightInPixels && mapHeightInPixels < mapWindowSize.Height) return;

            int autoScrollX = mapX - mapWindowSize.Width / 2;
            int autoScrollY = mapY - mapWindowSize.Height / 2;

            if (SelectionChanged != null)
                SelectionChanged(new Point(autoScrollX, autoScrollY));
        }

        private void pnlImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            isMouseDown = false;
            pnlImage.Invalidate();
        }

        private void pnlImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown) pnlImage_MouseDown(sender, e);
        }
    }
}
