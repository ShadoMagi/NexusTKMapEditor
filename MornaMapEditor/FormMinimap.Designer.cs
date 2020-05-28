namespace MornaMapEditor
{
    partial class FormMinimap
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize) (this.pnlImage)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlImage
            // 
            this.pnlImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pnlImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImage.Location = new System.Drawing.Point(0, 0);
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Size = new System.Drawing.Size(244, 226);
            this.pnlImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pnlImage.TabIndex = 0;
            this.pnlImage.TabStop = false;
            this.pnlImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlImage_Paint);
            this.pnlImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlImage_MouseDown);
            this.pnlImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlImage_MouseMove);
            this.pnlImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlImage_MouseUp);
            // 
            // FormMinimap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(244, 226);
            this.Controls.Add(this.pnlImage);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMinimap";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Minimap";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize) (this.pnlImage)).EndInit();
            this.ResumeLayout(false);
        }

        internal System.Windows.Forms.PictureBox pnlImage;

        #endregion
    }
}