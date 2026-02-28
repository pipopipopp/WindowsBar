namespace WDZ
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Hubris = new NotifyIcon(components);
            SuspendLayout();
            // 
            // Hubris
            // 
            Hubris.Icon = (Icon)resources.GetObject("Hubris.Icon");
            Hubris.Text = "notifyIcon1";
            Hubris.Visible = true;
            Hubris.MouseDoubleClick += notifyIcon1_MouseDoubleClick;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            BackgroundImage = Properties.Resources.BG;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(1280, 40);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Opacity = 0.85D;
            StartPosition = FormStartPosition.Manual;
            Text = "Form1";
            TopMost = true;
            TransparencyKey = Color.DimGray;
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon Hubris;
    }
}
