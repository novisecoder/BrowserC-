using System.Windows.Forms;

namespace InternetArcade
{
    partial class InternetArcade
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InternetArcade));
            this.customTabControl = new System.Windows.Forms.CustomTabControl();
            this.SuspendLayout();
            // 
            // customTabControl
            // 
            this.customTabControl.DisplayStyle = System.Windows.Forms.TabStyle.Chrome;
            // 
            // 
            // 
            this.customTabControl.DisplayStyleProvider.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.customTabControl.DisplayStyleProvider.BorderColorHot = System.Drawing.SystemColors.ControlDark;
            this.customTabControl.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.customTabControl.DisplayStyleProvider.CloserColor = System.Drawing.Color.DarkGray;
            this.customTabControl.DisplayStyleProvider.CloserColorActive = System.Drawing.Color.White;
            this.customTabControl.DisplayStyleProvider.FocusTrack = false;
            this.customTabControl.DisplayStyleProvider.HotTrack = true;
            this.customTabControl.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.customTabControl.DisplayStyleProvider.Opacity = 1F;
            this.customTabControl.DisplayStyleProvider.Overlap = 16;
            this.customTabControl.DisplayStyleProvider.Padding = new System.Drawing.Point(7, 5);
            this.customTabControl.DisplayStyleProvider.Radius = 16;
            this.customTabControl.DisplayStyleProvider.ShowTabCloser = true;
            this.customTabControl.DisplayStyleProvider.TextColor = System.Drawing.SystemColors.ControlText;
            this.customTabControl.DisplayStyleProvider.TextColorDisabled = System.Drawing.SystemColors.ControlDark;
            this.customTabControl.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText;
            this.customTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customTabControl.HotTrack = true;
            this.customTabControl.Location = new System.Drawing.Point(0, 0);
            this.customTabControl.Name = "customTabControl";
            this.customTabControl.SelectedIndex = 0;
            this.customTabControl.Size = new System.Drawing.Size(800, 450);
            this.customTabControl.TabIndex = 0;
            // 
            // InternetArcade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.customTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InternetArcade";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternetArcade_FormClosing);
            this.Load += new System.EventHandler(this.InternetArcade_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InternetArcade_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomTabControl customTabControl;
    }
}

