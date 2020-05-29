using System.ComponentModel;

namespace MornaMapEditor
{
    partial class BatchConverterDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.sourceFolder = new System.Windows.Forms.TextBox();
            this.sourceButton = new System.Windows.Forms.Button();
            this.destinationButton = new System.Windows.Forms.Button();
            this.destinationFolder = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.sourceLabel = new System.Windows.Forms.Label();
            this.destinationLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sourceFolder
            // 
            this.sourceFolder.Location = new System.Drawing.Point(144, 61);
            this.sourceFolder.Name = "sourceFolder";
            this.sourceFolder.ReadOnly = true;
            this.sourceFolder.Size = new System.Drawing.Size(461, 20);
            this.sourceFolder.TabIndex = 0;
            // 
            // sourceButton
            // 
            this.sourceButton.Location = new System.Drawing.Point(611, 61);
            this.sourceButton.Name = "sourceButton";
            this.sourceButton.Size = new System.Drawing.Size(82, 19);
            this.sourceButton.TabIndex = 1;
            this.sourceButton.Text = "Browse";
            this.sourceButton.UseVisualStyleBackColor = true;
            this.sourceButton.Click += new System.EventHandler(this.sourceButton_Click);
            // 
            // destinationButton
            // 
            this.destinationButton.Location = new System.Drawing.Point(611, 102);
            this.destinationButton.Name = "destinationButton";
            this.destinationButton.Size = new System.Drawing.Size(82, 19);
            this.destinationButton.TabIndex = 3;
            this.destinationButton.Text = "Browse\r\n";
            this.destinationButton.UseVisualStyleBackColor = true;
            this.destinationButton.Click += new System.EventHandler(this.destinationButton_Click);
            // 
            // destinationFolder
            // 
            this.destinationFolder.Location = new System.Drawing.Point(144, 102);
            this.destinationFolder.Name = "destinationFolder";
            this.destinationFolder.ReadOnly = true;
            this.destinationFolder.Size = new System.Drawing.Size(461, 20);
            this.destinationFolder.TabIndex = 2;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.descriptionLabel.Location = new System.Drawing.Point(12, 11);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(492, 46);
            this.descriptionLabel.TabIndex = 4;
            this.descriptionLabel.Text = "Select Source and Destination Folders:";
            // 
            // sourceLabel
            // 
            this.sourceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.sourceLabel.Location = new System.Drawing.Point(25, 61);
            this.sourceLabel.Name = "sourceLabel";
            this.sourceLabel.Size = new System.Drawing.Size(113, 20);
            this.sourceLabel.TabIndex = 5;
            this.sourceLabel.Text = "Source:";
            this.sourceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // destinationLabel
            // 
            this.destinationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.destinationLabel.Location = new System.Drawing.Point(25, 102);
            this.destinationLabel.Name = "destinationLabel";
            this.destinationLabel.Size = new System.Drawing.Size(113, 20);
            this.destinationLabel.TabIndex = 6;
            this.destinationLabel.Text = "Destination:";
            this.destinationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.okButton.Location = new System.Drawing.Point(267, 135);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.cancelButton.Location = new System.Drawing.Point(348, 135);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // BatchConverterDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(705, 170);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.destinationLabel);
            this.Controls.Add(this.sourceLabel);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.destinationButton);
            this.Controls.Add(this.destinationFolder);
            this.Controls.Add(this.sourceButton);
            this.Controls.Add(this.sourceFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BatchConverterDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Convert Maps to png";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Button destinationButton;
        private System.Windows.Forms.TextBox destinationFolder;
        private System.Windows.Forms.Label destinationLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button sourceButton;
        private System.Windows.Forms.TextBox sourceFolder;
        private System.Windows.Forms.Label sourceLabel;

        #endregion
    }
}