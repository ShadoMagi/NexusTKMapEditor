namespace MornaMapEditor
{
    partial class FormCopySection
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
            this.labelUpperLeft = new System.Windows.Forms.Label();
            this.numericUpDownUpperX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownUpperY = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownLowerX = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownLowerY = new System.Windows.Forms.NumericUpDown();
            this.labelLowerRight = new System.Windows.Forms.Label();
            this.checkBoxCopyObjects = new System.Windows.Forms.CheckBox();
            this.checkBoxCopyTiles = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxCopyPass = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize) (this.numericUpDownUpperX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.numericUpDownUpperY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.numericUpDownLowerX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.numericUpDownLowerY)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelUpperLeft
            // 
            this.labelUpperLeft.AutoSize = true;
            this.labelUpperLeft.Location = new System.Drawing.Point(12, 9);
            this.labelUpperLeft.Name = "labelUpperLeft";
            this.labelUpperLeft.Size = new System.Drawing.Size(72, 13);
            this.labelUpperLeft.TabIndex = 0;
            this.labelUpperLeft.Text = "Upper left tile:";
            // 
            // numericUpDownUpperX
            // 
            this.numericUpDownUpperX.Location = new System.Drawing.Point(99, 7);
            this.numericUpDownUpperX.Name = "numericUpDownUpperX";
            this.numericUpDownUpperX.Size = new System.Drawing.Size(72, 20);
            this.numericUpDownUpperX.TabIndex = 1;
            // 
            // numericUpDownUpperY
            // 
            this.numericUpDownUpperY.Location = new System.Drawing.Point(177, 7);
            this.numericUpDownUpperY.Name = "numericUpDownUpperY";
            this.numericUpDownUpperY.Size = new System.Drawing.Size(72, 20);
            this.numericUpDownUpperY.TabIndex = 2;
            // 
            // numericUpDownLowerX
            // 
            this.numericUpDownLowerX.Location = new System.Drawing.Point(99, 35);
            this.numericUpDownLowerX.Name = "numericUpDownLowerX";
            this.numericUpDownLowerX.Size = new System.Drawing.Size(72, 20);
            this.numericUpDownLowerX.TabIndex = 3;
            // 
            // numericUpDownLowerY
            // 
            this.numericUpDownLowerY.Location = new System.Drawing.Point(177, 35);
            this.numericUpDownLowerY.Name = "numericUpDownLowerY";
            this.numericUpDownLowerY.Size = new System.Drawing.Size(72, 20);
            this.numericUpDownLowerY.TabIndex = 4;
            // 
            // labelLowerRight
            // 
            this.labelLowerRight.AutoSize = true;
            this.labelLowerRight.Location = new System.Drawing.Point(12, 35);
            this.labelLowerRight.Name = "labelLowerRight";
            this.labelLowerRight.Size = new System.Drawing.Size(78, 13);
            this.labelLowerRight.TabIndex = 5;
            this.labelLowerRight.Text = "Lower right tile:";
            // 
            // checkBoxCopyObjects
            // 
            this.checkBoxCopyObjects.AutoSize = true;
            this.checkBoxCopyObjects.Location = new System.Drawing.Point(6, 19);
            this.checkBoxCopyObjects.Name = "checkBoxCopyObjects";
            this.checkBoxCopyObjects.Size = new System.Drawing.Size(87, 17);
            this.checkBoxCopyObjects.TabIndex = 6;
            this.checkBoxCopyObjects.Text = "Copy objects";
            this.checkBoxCopyObjects.UseVisualStyleBackColor = true;
            // 
            // checkBoxCopyTiles
            // 
            this.checkBoxCopyTiles.AutoSize = true;
            this.checkBoxCopyTiles.Location = new System.Drawing.Point(6, 42);
            this.checkBoxCopyTiles.Name = "checkBoxCopyTiles";
            this.checkBoxCopyTiles.Size = new System.Drawing.Size(71, 17);
            this.checkBoxCopyTiles.TabIndex = 7;
            this.checkBoxCopyTiles.Text = "Copy tiles";
            this.checkBoxCopyTiles.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxCopyPass);
            this.groupBox1.Controls.Add(this.checkBoxCopyObjects);
            this.groupBox1.Controls.Add(this.checkBoxCopyTiles);
            this.groupBox1.Location = new System.Drawing.Point(12, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(237, 104);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Copy options:";
            // 
            // checkBoxCopyPass
            // 
            this.checkBoxCopyPass.AutoSize = true;
            this.checkBoxCopyPass.Location = new System.Drawing.Point(6, 65);
            this.checkBoxCopyPass.Name = "checkBoxCopyPass";
            this.checkBoxCopyPass.Size = new System.Drawing.Size(75, 17);
            this.checkBoxCopyPass.TabIndex = 8;
            this.checkBoxCopyPass.Text = "Copy pass";
            this.checkBoxCopyPass.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(93, 171);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(174, 171);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormCopySection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 206);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelLowerRight);
            this.Controls.Add(this.numericUpDownLowerY);
            this.Controls.Add(this.numericUpDownLowerX);
            this.Controls.Add(this.numericUpDownUpperY);
            this.Controls.Add(this.numericUpDownUpperX);
            this.Controls.Add(this.labelUpperLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormCopySection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copy Section";
            ((System.ComponentModel.ISupportInitialize) (this.numericUpDownUpperX)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.numericUpDownUpperY)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.numericUpDownLowerX)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.numericUpDownLowerY)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkBoxCopyObjects;
        private System.Windows.Forms.CheckBox checkBoxCopyPass;
        private System.Windows.Forms.CheckBox checkBoxCopyTiles;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label labelLowerRight;
        private System.Windows.Forms.Label labelUpperLeft;
        private System.Windows.Forms.NumericUpDown numericUpDownLowerX;
        private System.Windows.Forms.NumericUpDown numericUpDownLowerY;
        private System.Windows.Forms.NumericUpDown numericUpDownUpperX;
        private System.Windows.Forms.NumericUpDown numericUpDownUpperY;

        #endregion
    }
}