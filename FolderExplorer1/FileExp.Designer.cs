namespace FolderExplorer1
{
    partial class FileExp
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
            this.components = new System.ComponentModel.Container();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label_Directory = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.FilePath = new System.Windows.Forms.TextBox();
            this.timerHidePopup = new System.Windows.Forms.Timer(this.components);
            this.timerUpdateCursorImage = new System.Windows.Forms.Timer(this.components);
            this.titleImage = new System.Windows.Forms.PictureBox();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.subScrollRight = new System.Windows.Forms.Button();
            this.subScrollLeft = new System.Windows.Forms.Button();
            this.mainScrollRight = new System.Windows.Forms.Button();
            this.mainScrollLeft = new System.Windows.Forms.Button();
            this.buttonBack = new System.Windows.Forms.Button();
            this.pictureBox_DirIndecator = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.titleImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DirIndecator)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(0, 213);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(941, 5);
            this.panel2.TabIndex = 15;
            // 
            // label_Directory
            // 
            this.label_Directory.AutoSize = true;
            this.label_Directory.BackColor = System.Drawing.Color.White;
            this.label_Directory.Font = new System.Drawing.Font("Tahoma", 20F);
            this.label_Directory.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_Directory.Location = new System.Drawing.Point(228, 3);
            this.label_Directory.Name = "label_Directory";
            this.label_Directory.Size = new System.Drawing.Size(0, 33);
            this.label_Directory.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 20F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(229, 33);
            this.label1.TabIndex = 21;
            this.label1.Text = "Current Location: ";
            this.label1.Visible = false;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(941, 2);
            this.panel1.TabIndex = 22;
            this.panel1.Visible = false;
            // 
            // FilePath
            // 
            this.FilePath.Cursor = System.Windows.Forms.Cursors.No;
            this.FilePath.Enabled = false;
            this.FilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FilePath.Location = new System.Drawing.Point(76, 3);
            this.FilePath.Name = "FilePath";
            this.FilePath.ReadOnly = true;
            this.FilePath.Size = new System.Drawing.Size(377, 53);
            this.FilePath.TabIndex = 23;
            // 
            // timerHidePopup
            // 
            this.timerHidePopup.Tick += new System.EventHandler(this.timerHidePopup_Tick);
            // 
            // timerUpdateCursorImage
            // 
            this.timerUpdateCursorImage.Enabled = true;
            this.timerUpdateCursorImage.Tick += new System.EventHandler(this.timerUpdateCursorImage_Tick);
            // 
            // titleImage
            // 
            this.titleImage.BackColor = System.Drawing.Color.Black;
            this.titleImage.BackgroundImage = global::FolderExplorer1.Properties.Resources.title;
            this.titleImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.titleImage.Location = new System.Drawing.Point(100, 27);
            this.titleImage.Name = "titleImage";
            this.titleImage.Size = new System.Drawing.Size(100, 50);
            this.titleImage.TabIndex = 32;
            this.titleImage.TabStop = false;
            // 
            // buttonHelp
            // 
            this.buttonHelp.BackgroundImage = global::FolderExplorer1.Properties.Resources.questionMark;
            this.buttonHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonHelp.Location = new System.Drawing.Point(0, 0);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(75, 23);
            this.buttonHelp.TabIndex = 31;
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.BackgroundImage = global::FolderExplorer1.Properties.Resources.close_button_T;
            this.buttonClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonClose.Location = new System.Drawing.Point(84, 11);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 29;
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // subScrollRight
            // 
            this.subScrollRight.BackgroundImage = global::FolderExplorer1.Properties.Resources.blueArrowRightImage;
            this.subScrollRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.subScrollRight.Location = new System.Drawing.Point(0, 128);
            this.subScrollRight.Name = "subScrollRight";
            this.subScrollRight.Size = new System.Drawing.Size(100, 100);
            this.subScrollRight.TabIndex = 28;
            this.subScrollRight.UseVisualStyleBackColor = true;
            this.subScrollRight.MouseEnter += new System.EventHandler(this.subScrollRight_MouseEnter);
            this.subScrollRight.MouseLeave += new System.EventHandler(this.subScrollRight_MouseLeave);
            // 
            // subScrollLeft
            // 
            this.subScrollLeft.BackgroundImage = global::FolderExplorer1.Properties.Resources.blueArrowLeftImage;
            this.subScrollLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.subScrollLeft.Location = new System.Drawing.Point(0, 99);
            this.subScrollLeft.Name = "subScrollLeft";
            this.subScrollLeft.Size = new System.Drawing.Size(100, 100);
            this.subScrollLeft.TabIndex = 27;
            this.subScrollLeft.UseVisualStyleBackColor = true;
            this.subScrollLeft.MouseEnter += new System.EventHandler(this.subScrollLeft_MouseEnter);
            this.subScrollLeft.MouseLeave += new System.EventHandler(this.subScrollLeft_MouseLeave);
            // 
            // mainScrollRight
            // 
            this.mainScrollRight.BackgroundImage = global::FolderExplorer1.Properties.Resources.blueArrowRightImage;
            this.mainScrollRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mainScrollRight.Location = new System.Drawing.Point(0, 70);
            this.mainScrollRight.Name = "mainScrollRight";
            this.mainScrollRight.Size = new System.Drawing.Size(100, 100);
            this.mainScrollRight.TabIndex = 26;
            this.mainScrollRight.UseVisualStyleBackColor = true;
            this.mainScrollRight.MouseEnter += new System.EventHandler(this.mainScrollRight_MouseEnter);
            this.mainScrollRight.MouseLeave += new System.EventHandler(this.mainScrollRight_MouseLeave);
            // 
            // mainScrollLeft
            // 
            this.mainScrollLeft.BackgroundImage = global::FolderExplorer1.Properties.Resources.blueArrowLeftImage;
            this.mainScrollLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.mainScrollLeft.Location = new System.Drawing.Point(0, 41);
            this.mainScrollLeft.Name = "mainScrollLeft";
            this.mainScrollLeft.Size = new System.Drawing.Size(100, 100);
            this.mainScrollLeft.TabIndex = 25;
            this.mainScrollLeft.UseVisualStyleBackColor = true;
            this.mainScrollLeft.MouseEnter += new System.EventHandler(this.mainScrollLeft_MouseEnter);
            this.mainScrollLeft.MouseLeave += new System.EventHandler(this.mainScrollLeft_MouseLeave);
            // 
            // buttonBack
            // 
            this.buttonBack.BackgroundImage = global::FolderExplorer1.Properties.Resources.blueBackImage;
            this.buttonBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonBack.Location = new System.Drawing.Point(0, 1);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(75, 23);
            this.buttonBack.TabIndex = 24;
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // pictureBox_DirIndecator
            // 
            this.pictureBox_DirIndecator.Location = new System.Drawing.Point(10, 149);
            this.pictureBox_DirIndecator.Name = "pictureBox_DirIndecator";
            this.pictureBox_DirIndecator.Size = new System.Drawing.Size(165, 29);
            this.pictureBox_DirIndecator.TabIndex = 20;
            this.pictureBox_DirIndecator.TabStop = false;
            this.pictureBox_DirIndecator.Visible = false;
            // 
            // FileExp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(941, 605);
            this.Controls.Add(this.titleImage);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.subScrollRight);
            this.Controls.Add(this.subScrollLeft);
            this.Controls.Add(this.mainScrollRight);
            this.Controls.Add(this.mainScrollLeft);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.FilePath);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox_DirIndecator);
            this.Controls.Add(this.label_Directory);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "FileExp";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileExp_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FileExp_KeyDown_1);
            ((System.ComponentModel.ISupportInitialize)(this.titleImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_DirIndecator)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label_Directory;
        private System.Windows.Forms.PictureBox pictureBox_DirIndecator;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox FilePath;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Button mainScrollLeft;
        private System.Windows.Forms.Button mainScrollRight;
        private System.Windows.Forms.Timer timer_updatePanelScroll;
        private System.Windows.Forms.Button subScrollLeft;
        private System.Windows.Forms.Button subScrollRight;
        private System.Windows.Forms.Timer timerHidePopup;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.Timer timerUpdateCursorImage;
        private System.Windows.Forms.PictureBox titleImage;
    }
}

