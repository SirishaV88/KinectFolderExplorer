namespace FolderExplorer1
{
    partial class UserControl_Menu_Options
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserControl_Menu_Options));
            this.labelCopy = new System.Windows.Forms.Label();
            this.labelDelete = new System.Windows.Forms.Label();
            this.labelPaste = new System.Windows.Forms.Label();
            this.labelOpen = new System.Windows.Forms.Label();
            this.button_Open = new System.Windows.Forms.Button();
            this.button_Paste = new System.Windows.Forms.Button();
            this.button_Delete = new System.Windows.Forms.Button();
            this.button_Copy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelCopy
            // 
            this.labelCopy.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelCopy.Location = new System.Drawing.Point(106, 160);
            this.labelCopy.Name = "labelCopy";
            this.labelCopy.Size = new System.Drawing.Size(54, 24);
            this.labelCopy.TabIndex = 8;
            this.labelCopy.Text = "Copy";
            this.labelCopy.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelDelete
            // 
            this.labelDelete.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelDelete.Location = new System.Drawing.Point(19, 322);
            this.labelDelete.Name = "labelDelete";
            this.labelDelete.Size = new System.Drawing.Size(69, 24);
            this.labelDelete.TabIndex = 9;
            this.labelDelete.Text = "Delete";
            this.labelDelete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelPaste
            // 
            this.labelPaste.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelPaste.Location = new System.Drawing.Point(113, 260);
            this.labelPaste.Name = "labelPaste";
            this.labelPaste.Size = new System.Drawing.Size(59, 24);
            this.labelPaste.TabIndex = 10;
            this.labelPaste.Text = "Paste";
            this.labelPaste.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelOpen
            // 
            this.labelOpen.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelOpen.Location = new System.Drawing.Point(17, 91);
            this.labelOpen.Name = "labelOpen";
            this.labelOpen.Size = new System.Drawing.Size(57, 24);
            this.labelOpen.TabIndex = 12;
            this.labelOpen.Text = "Open";
            this.labelOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button_Open
            // 
            this.button_Open.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Open.FlatAppearance.BorderSize = 0;
            this.button_Open.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.button_Open.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Open.ForeColor = System.Drawing.Color.Transparent;
            this.button_Open.Image = global::FolderExplorer1.Properties.Resources.open2;
            this.button_Open.Location = new System.Drawing.Point(14, 28);
            this.button_Open.Name = "button_Open";
            this.button_Open.Size = new System.Drawing.Size(68, 68);
            this.button_Open.TabIndex = 11;
            this.button_Open.UseVisualStyleBackColor = true;
            this.button_Open.Click += new System.EventHandler(this.buttonbuttonOpen_Click);
            this.button_Open.MouseEnter += new System.EventHandler(this.button_Open_MouseEnter);
            this.button_Open.MouseLeave += new System.EventHandler(this.button_Open_MouseLeave);
            // 
            // button_Paste
            // 
            this.button_Paste.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Paste.FlatAppearance.BorderSize = 0;
            this.button_Paste.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.button_Paste.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Paste.Image = ((System.Drawing.Image)(resources.GetObject("button_Paste.Image")));
            this.button_Paste.Location = new System.Drawing.Point(104, 199);
            this.button_Paste.Name = "button_Paste";
            this.button_Paste.Size = new System.Drawing.Size(64, 64);
            this.button_Paste.TabIndex = 4;
            this.button_Paste.UseVisualStyleBackColor = true;
            this.button_Paste.Click += new System.EventHandler(this.button_paste_Click);
            this.button_Paste.MouseEnter += new System.EventHandler(this.button_Paste_MouseEnter);
            this.button_Paste.MouseLeave += new System.EventHandler(this.button_Paste_MouseLeave);
            // 
            // button_Delete
            // 
            this.button_Delete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button_Delete.FlatAppearance.BorderSize = 0;
            this.button_Delete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.button_Delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Delete.Image = global::FolderExplorer1.Properties.Resources.delete2;
            this.button_Delete.Location = new System.Drawing.Point(23, 274);
            this.button_Delete.Name = "button_Delete";
            this.button_Delete.Size = new System.Drawing.Size(64, 64);
            this.button_Delete.TabIndex = 3;
            this.button_Delete.UseVisualStyleBackColor = true;
            this.button_Delete.Click += new System.EventHandler(this.button_Delete_Click);
            this.button_Delete.MouseEnter += new System.EventHandler(this.button_Delete_MouseEnter);
            this.button_Delete.MouseLeave += new System.EventHandler(this.button_Delete_MouseLeave);
            // 
            // button_Copy
            // 
            this.button_Copy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_Copy.FlatAppearance.BorderSize = 0;
            this.button_Copy.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.button_Copy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Copy.Image = global::FolderExplorer1.Properties.Resources.copy2;
            this.button_Copy.Location = new System.Drawing.Point(100, 106);
            this.button_Copy.Name = "button_Copy";
            this.button_Copy.Size = new System.Drawing.Size(64, 64);
            this.button_Copy.TabIndex = 0;
            this.button_Copy.UseVisualStyleBackColor = true;
            this.button_Copy.Click += new System.EventHandler(this.button_Copy_Click);
            this.button_Copy.MouseEnter += new System.EventHandler(this.button_Copy_MouseEnter);
            this.button_Copy.MouseLeave += new System.EventHandler(this.button_Copy_MouseLeave);
            // 
            // UserControl_Menu_Options
            // 
            this.Controls.Add(this.labelOpen);
            this.Controls.Add(this.button_Open);
            this.Controls.Add(this.labelPaste);
            this.Controls.Add(this.labelDelete);
            this.Controls.Add(this.labelCopy);
            this.Controls.Add(this.button_Paste);
            this.Controls.Add(this.button_Delete);
            this.Controls.Add(this.button_Copy);
            this.Size = new System.Drawing.Size(200, 380);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Copy;
        private System.Windows.Forms.Button button_Delete;
        private System.Windows.Forms.Button button_Paste;
        private System.Windows.Forms.Label labelCopy;
        private System.Windows.Forms.Label labelDelete;
        private System.Windows.Forms.Label labelPaste;
        private System.Windows.Forms.Button button_Open;
        private System.Windows.Forms.Label labelOpen;
    }
}
