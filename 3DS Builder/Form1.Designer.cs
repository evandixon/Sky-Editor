namespace _3DS_Builder
{
    partial class Form1
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
            this.PB_Show = new System.Windows.Forms.ProgressBar();
            this.TB_Romfs = new System.Windows.Forms.TextBox();
            this.B_Go = new System.Windows.Forms.Button();
            this.B_Romfs = new System.Windows.Forms.Button();
            this.TB_Exefs = new System.Windows.Forms.TextBox();
            this.TB_Exheader = new System.Windows.Forms.TextBox();
            this.B_Exefs = new System.Windows.Forms.Button();
            this.B_Exheader = new System.Windows.Forms.Button();
            this.CB_Logo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.B_SavePath = new System.Windows.Forms.Button();
            this.TB_SavePath = new System.Windows.Forms.TextBox();
            this.CHK_Card2 = new System.Windows.Forms.CheckBox();
            this.TB_Serial = new System.Windows.Forms.TextBox();
            this.CHK_PrebuiltRomfs = new System.Windows.Forms.CheckBox();
            this.CHK_PrebuiltExefs = new System.Windows.Forms.CheckBox();
            this.RTB_Progress = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // PB_Show
            // 
            this.PB_Show.Location = new System.Drawing.Point(8, 331);
            this.PB_Show.Name = "PB_Show";
            this.PB_Show.Size = new System.Drawing.Size(434, 23);
            this.PB_Show.TabIndex = 10;
            // 
            // TB_Romfs
            // 
            this.TB_Romfs.Location = new System.Drawing.Point(10, 11);
            this.TB_Romfs.Name = "TB_Romfs";
            this.TB_Romfs.ReadOnly = true;
            this.TB_Romfs.Size = new System.Drawing.Size(306, 20);
            this.TB_Romfs.TabIndex = 8;
            // 
            // B_Go
            // 
            this.B_Go.Location = new System.Drawing.Point(322, 303);
            this.B_Go.Name = "B_Go";
            this.B_Go.Size = new System.Drawing.Size(120, 22);
            this.B_Go.TabIndex = 7;
            this.B_Go.Text = "Go";
            this.B_Go.UseVisualStyleBackColor = true;
            this.B_Go.Click += new System.EventHandler(this.B_Go_Click);
            // 
            // B_Romfs
            // 
            this.B_Romfs.Location = new System.Drawing.Point(322, 10);
            this.B_Romfs.Name = "B_Romfs";
            this.B_Romfs.Size = new System.Drawing.Size(120, 22);
            this.B_Romfs.TabIndex = 6;
            this.B_Romfs.Text = "Open Romfs";
            this.B_Romfs.UseVisualStyleBackColor = true;
            this.B_Romfs.Click += new System.EventHandler(this.B_Romfs_Click);
            // 
            // TB_Exefs
            // 
            this.TB_Exefs.Location = new System.Drawing.Point(10, 37);
            this.TB_Exefs.Name = "TB_Exefs";
            this.TB_Exefs.ReadOnly = true;
            this.TB_Exefs.Size = new System.Drawing.Size(306, 20);
            this.TB_Exefs.TabIndex = 11;
            // 
            // TB_Exheader
            // 
            this.TB_Exheader.Location = new System.Drawing.Point(10, 63);
            this.TB_Exheader.Name = "TB_Exheader";
            this.TB_Exheader.ReadOnly = true;
            this.TB_Exheader.Size = new System.Drawing.Size(306, 20);
            this.TB_Exheader.TabIndex = 12;
            // 
            // B_Exefs
            // 
            this.B_Exefs.Location = new System.Drawing.Point(322, 36);
            this.B_Exefs.Name = "B_Exefs";
            this.B_Exefs.Size = new System.Drawing.Size(120, 22);
            this.B_Exefs.TabIndex = 13;
            this.B_Exefs.Text = "Open Exefs";
            this.B_Exefs.UseVisualStyleBackColor = true;
            this.B_Exefs.Click += new System.EventHandler(this.B_Exefs_Click);
            // 
            // B_Exheader
            // 
            this.B_Exheader.Location = new System.Drawing.Point(322, 62);
            this.B_Exheader.Name = "B_Exheader";
            this.B_Exheader.Size = new System.Drawing.Size(120, 22);
            this.B_Exheader.TabIndex = 14;
            this.B_Exheader.Text = "Open Exheader";
            this.B_Exheader.UseVisualStyleBackColor = true;
            this.B_Exheader.Click += new System.EventHandler(this.B_Exheader_Click);
            // 
            // CB_Logo
            // 
            this.CB_Logo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Logo.FormattingEnabled = true;
            this.CB_Logo.Location = new System.Drawing.Point(42, 114);
            this.CB_Logo.Name = "CB_Logo";
            this.CB_Logo.Size = new System.Drawing.Size(113, 21);
            this.CB_Logo.TabIndex = 15;
            this.CB_Logo.SelectedIndexChanged += new System.EventHandler(this.CB_Logo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Logo:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Serial:";
            // 
            // B_SavePath
            // 
            this.B_SavePath.Location = new System.Drawing.Point(322, 88);
            this.B_SavePath.Name = "B_SavePath";
            this.B_SavePath.Size = new System.Drawing.Size(120, 22);
            this.B_SavePath.TabIndex = 20;
            this.B_SavePath.Text = "Choose Save Loc.";
            this.B_SavePath.UseVisualStyleBackColor = true;
            this.B_SavePath.Click += new System.EventHandler(this.B_SavePath_Click);
            // 
            // TB_SavePath
            // 
            this.TB_SavePath.Location = new System.Drawing.Point(10, 89);
            this.TB_SavePath.Name = "TB_SavePath";
            this.TB_SavePath.ReadOnly = true;
            this.TB_SavePath.Size = new System.Drawing.Size(306, 20);
            this.TB_SavePath.TabIndex = 19;
            // 
            // CHK_Card2
            // 
            this.CHK_Card2.AutoSize = true;
            this.CHK_Card2.Location = new System.Drawing.Point(322, 160);
            this.CHK_Card2.Name = "CHK_Card2";
            this.CHK_Card2.Size = new System.Drawing.Size(126, 17);
            this.CHK_Card2.TabIndex = 21;
            this.CHK_Card2.Text = "Media Device: Card2";
            this.CHK_Card2.UseVisualStyleBackColor = true;
            this.CHK_Card2.CheckedChanged += new System.EventHandler(this.CHK_Card2_CheckedChanged);
            // 
            // TB_Serial
            // 
            this.TB_Serial.Location = new System.Drawing.Point(200, 115);
            this.TB_Serial.MaxLength = 10;
            this.TB_Serial.Name = "TB_Serial";
            this.TB_Serial.Size = new System.Drawing.Size(100, 20);
            this.TB_Serial.TabIndex = 22;
            this.TB_Serial.Text = "CTR-P-XXXX";
            this.TB_Serial.TextChanged += new System.EventHandler(this.TB_Serial_TextChanged);
            // 
            // CHK_PrebuiltRomfs
            // 
            this.CHK_PrebuiltRomfs.AutoSize = true;
            this.CHK_PrebuiltRomfs.Location = new System.Drawing.Point(322, 119);
            this.CHK_PrebuiltRomfs.Name = "CHK_PrebuiltRomfs";
            this.CHK_PrebuiltRomfs.Size = new System.Drawing.Size(120, 17);
            this.CHK_PrebuiltRomfs.TabIndex = 23;
            this.CHK_PrebuiltRomfs.Text = "Use Pre-Built Romfs";
            this.CHK_PrebuiltRomfs.UseVisualStyleBackColor = true;
            this.CHK_PrebuiltRomfs.CheckedChanged += new System.EventHandler(this.CHK_PrebuiltRomfs_CheckedChanged);
            // 
            // CHK_PrebuiltExefs
            // 
            this.CHK_PrebuiltExefs.AutoSize = true;
            this.CHK_PrebuiltExefs.Location = new System.Drawing.Point(322, 139);
            this.CHK_PrebuiltExefs.Name = "CHK_PrebuiltExefs";
            this.CHK_PrebuiltExefs.Size = new System.Drawing.Size(116, 17);
            this.CHK_PrebuiltExefs.TabIndex = 24;
            this.CHK_PrebuiltExefs.Text = "Use Pre-Built Exefs";
            this.CHK_PrebuiltExefs.UseVisualStyleBackColor = true;
            this.CHK_PrebuiltExefs.CheckedChanged += new System.EventHandler(this.CHK_PrebuiltExefs_CheckedChanged);
            // 
            // RTB_Progress
            // 
            this.RTB_Progress.BackColor = System.Drawing.SystemColors.Control;
            this.RTB_Progress.Location = new System.Drawing.Point(8, 139);
            this.RTB_Progress.Name = "RTB_Progress";
            this.RTB_Progress.ReadOnly = true;
            this.RTB_Progress.Size = new System.Drawing.Size(308, 186);
            this.RTB_Progress.TabIndex = 25;
            this.RTB_Progress.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 361);
            this.Controls.Add(this.RTB_Progress);
            this.Controls.Add(this.CHK_PrebuiltExefs);
            this.Controls.Add(this.CHK_PrebuiltRomfs);
            this.Controls.Add(this.TB_Serial);
            this.Controls.Add(this.CHK_Card2);
            this.Controls.Add(this.B_SavePath);
            this.Controls.Add(this.TB_SavePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CB_Logo);
            this.Controls.Add(this.B_Exheader);
            this.Controls.Add(this.B_Exefs);
            this.Controls.Add(this.TB_Exheader);
            this.Controls.Add(this.TB_Exefs);
            this.Controls.Add(this.PB_Show);
            this.Controls.Add(this.TB_Romfs);
            this.Controls.Add(this.B_Go);
            this.Controls.Add(this.B_Romfs);
            this.MaximumSize = new System.Drawing.Size(460, 400);
            this.MinimumSize = new System.Drawing.Size(460, 400);
            this.Name = "Form1";
            this.Text = "3DS Builder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar PB_Show;
        private System.Windows.Forms.TextBox TB_Romfs;
        private System.Windows.Forms.Button B_Go;
        private System.Windows.Forms.Button B_Romfs;
        private System.Windows.Forms.TextBox TB_Exefs;
        private System.Windows.Forms.TextBox TB_Exheader;
        private System.Windows.Forms.Button B_Exefs;
        private System.Windows.Forms.Button B_Exheader;
        private System.Windows.Forms.ComboBox CB_Logo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button B_SavePath;
        private System.Windows.Forms.TextBox TB_SavePath;
        private System.Windows.Forms.CheckBox CHK_Card2;
        private System.Windows.Forms.TextBox TB_Serial;
        private System.Windows.Forms.CheckBox CHK_PrebuiltRomfs;
        private System.Windows.Forms.CheckBox CHK_PrebuiltExefs;
        private System.Windows.Forms.RichTextBox RTB_Progress;
    }
}

