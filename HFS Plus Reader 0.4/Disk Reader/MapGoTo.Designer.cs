namespace Disk_Reader
{
    partial class MapGoTo
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
            this.GoToBox_Go = new System.Windows.Forms.Button();
            this.GoToBox_Cancel = new System.Windows.Forms.Button();
            this.GoToBox_TextBox = new System.Windows.Forms.TextBox();
            this.GoToBox_Hex = new System.Windows.Forms.RadioButton();
            this.GoToBox_Decimal = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // GoToBox_Go
            // 
            this.GoToBox_Go.Location = new System.Drawing.Point(12, 39);
            this.GoToBox_Go.Name = "GoToBox_Go";
            this.GoToBox_Go.Size = new System.Drawing.Size(75, 23);
            this.GoToBox_Go.TabIndex = 0;
            this.GoToBox_Go.Text = "Go";
            this.GoToBox_Go.UseVisualStyleBackColor = true;
            this.GoToBox_Go.Click += new System.EventHandler(this.GoToBox_Go_Click);
            // 
            // GoToBox_Cancel
            // 
            this.GoToBox_Cancel.Location = new System.Drawing.Point(93, 39);
            this.GoToBox_Cancel.Name = "GoToBox_Cancel";
            this.GoToBox_Cancel.Size = new System.Drawing.Size(75, 23);
            this.GoToBox_Cancel.TabIndex = 1;
            this.GoToBox_Cancel.Text = "Cancel";
            this.GoToBox_Cancel.UseVisualStyleBackColor = true;
            // 
            // GoToBox_TextBox
            // 
            this.GoToBox_TextBox.Location = new System.Drawing.Point(13, 13);
            this.GoToBox_TextBox.Name = "GoToBox_TextBox";
            this.GoToBox_TextBox.Size = new System.Drawing.Size(155, 20);
            this.GoToBox_TextBox.TabIndex = 2;
            // 
            // GoToBox_Hex
            // 
            this.GoToBox_Hex.AutoSize = true;
            this.GoToBox_Hex.Location = new System.Drawing.Point(184, 15);
            this.GoToBox_Hex.Name = "GoToBox_Hex";
            this.GoToBox_Hex.Size = new System.Drawing.Size(44, 17);
            this.GoToBox_Hex.TabIndex = 3;
            this.GoToBox_Hex.TabStop = true;
            this.GoToBox_Hex.Text = "Hex";
            this.GoToBox_Hex.UseVisualStyleBackColor = true;
            // 
            // GoToBox_Decimal
            // 
            this.GoToBox_Decimal.AutoSize = true;
            this.GoToBox_Decimal.Location = new System.Drawing.Point(184, 39);
            this.GoToBox_Decimal.Name = "GoToBox_Decimal";
            this.GoToBox_Decimal.Size = new System.Drawing.Size(63, 17);
            this.GoToBox_Decimal.TabIndex = 4;
            this.GoToBox_Decimal.TabStop = true;
            this.GoToBox_Decimal.Text = "Decimal";
            this.GoToBox_Decimal.UseVisualStyleBackColor = true;
            // 
            // MapGoTo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.GoToBox_Cancel;
            this.ClientSize = new System.Drawing.Size(253, 71);
            this.ControlBox = false;
            this.Controls.Add(this.GoToBox_Decimal);
            this.Controls.Add(this.GoToBox_Hex);
            this.Controls.Add(this.GoToBox_TextBox);
            this.Controls.Add(this.GoToBox_Cancel);
            this.Controls.Add(this.GoToBox_Go);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MapGoTo";
            this.Text = "Go To Sector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GoToBox_Go;
        private System.Windows.Forms.Button GoToBox_Cancel;
        private System.Windows.Forms.TextBox GoToBox_TextBox;
        private System.Windows.Forms.RadioButton GoToBox_Hex;
        private System.Windows.Forms.RadioButton GoToBox_Decimal;
    }
}