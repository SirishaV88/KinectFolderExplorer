using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Threading;

namespace FolderExplorer1
{
    public partial class UserControl_Menu_Options : Control
    {
        #region JOSH UPDATE
        //error messages:
        MessageBoxIcon mIcon = new MessageBoxIcon();
        MessageBoxButtons mButtons = new MessageBoxButtons();
        //drawing:
        SolidBrush myOpaqueBrush;//for painting objects you can see through, in this case the menu background overlay ellipse
        SolidBrush mySolidBrush;//for painting solid objects, the menu border ellipse
        Pen myPen;//pen for drawing graphics
        int opacity;//specifies the level of "see through"
        Color myBackColor;//specifies the popup menu back color which is the color of the FillEllipse that overlays the backgroud image
        Color myBorderColor;//specifies the menu border color, the color of the regular ellipse that border the overlay
        public Boolean copyBufferLoaded = false;//helps determine when paste optino should be available
        private string PastePath = "";
        #endregion

        public string OpenPath = "";
        public string DeletePath = "";
        public string CopySourcePath = "";
        UserControl_DirBrowserScreen DBS = new UserControl_DirBrowserScreen();
        BrowserPanel parent;

        public UserControl_Menu_Options()
        {
            InitializeComponent();

            #region JOSH UPDATE
            //error messages:
            mIcon = MessageBoxIcon.Error;
            mButtons = MessageBoxButtons.OK;
            //drawing:
            opacity = 200;//noticeable but still see-through
            myBackColor = Color.White;//back is white if no one changes at runtime with method
            myBorderColor = Color.White;//border is white if no one changes at runtime with method
            this.Size = new Size(300, 300);//set control size
            myOpaqueBrush = new SolidBrush(Color.FromArgb(opacity, 255, 255, 255));//transparency brush settings
            mySolidBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255));//transparency brush settings
            myPen = new Pen(Color.White, 6);//solid pen for borders if desired
            ArrangeButtons();//arrange things within the control
            #endregion
        }

        #region JOSH UPDATE
        public void SetParent(BrowserPanel parent)
        {
            this.parent = parent;
            DBS.SetParent(parent);
        }
        public void SetPastePath(string path)
        {
            this.PastePath = path;
        }
        /// <summary>
        /// Sets the opacity of the menu background. Must be between 0 and 255. 0 is transparent. 255 is not see through.
        /// </summary>
        /// <param name="opacity"></param>
        public void SetBackOpacity(int opacity)
        {
            if (opacity > 255)
            {
                opacity = 255;
            }
            else if (opacity < 0)
            {
                opacity = 0;
            }
            this.opacity = opacity;
            myOpaqueBrush = new SolidBrush(Color.FromArgb(opacity, myBackColor.R, myBackColor.G, myBackColor.B));//transparency brush settings
        }
        public void SetBackColor(Color color)
        {
            myBackColor = color;
            myOpaqueBrush = new SolidBrush(Color.FromArgb(opacity, myBackColor.R, myBackColor.G, myBackColor.B));//transparency brush settings
        }
        public void SetBorderColor(Color color)
        {
            myBorderColor = color;
            myPen = new Pen(myBorderColor, 6);//solid pen for borders if desired
            mySolidBrush = new SolidBrush(Color.FromArgb(255, myBorderColor.R, myBorderColor.G, myBorderColor.B));//solid brush settings
        }
        public void ArrangeButtons()
        {
            //postition buttons:
            this.button_Open.Location = new Point(this.Location.X + (this.Width / 2) - (button_Open.Width / 2), this.Location.Y + 20);
            this.button_Copy.Location = new Point(this.Location.X + 25, this.Location.Y + (this.Height / 2) - (button_Copy.Height / 2) - 20);
            this.button_Paste.Location = new Point(this.Location.X + this.Width - button_Paste.Width - 25, this.Location.Y + (this.Height / 2) - (button_Paste.Height / 2) - 20);
            this.button_Delete.Location = new Point((this.Location.X + (int)(this.Width / 2) - (button_Delete.Width / 2)), (this.Location.Y + (int)(this.Height - button_Delete.Height)) - 20);
            //bring buttons to front:
            this.Controls.SetChildIndex(button_Open, 0);
            this.Controls.SetChildIndex(button_Copy, 0);
            this.Controls.SetChildIndex(button_Paste, 0);
            this.Controls.SetChildIndex(button_Delete, 0);
            //position labels
            this.labelOpen.Location = new Point(this.button_Open.Location.X, this.button_Open.Location.Y + this.button_Open.Height);
            this.labelCopy.Location = new Point(this.button_Copy.Location.X, this.button_Copy.Location.Y + this.button_Copy.Height);
            this.labelPaste.Location = new Point(this.button_Paste.Location.X, this.button_Paste.Location.Y + this.button_Paste.Height);
            this.labelDelete.Location = new Point(this.button_Delete.Location.X, this.button_Delete.Location.Y - this.labelDelete.Height);
            //match label widths to button widths
            this.labelOpen.Width = this.button_Open.Width;
            this.labelCopy.Width = this.button_Copy.Width;
            this.labelPaste.Width = this.button_Paste.Width;
            this.labelDelete.Width = this.button_Delete.Width;
        }
        public void SetPath(String path)
        {
            this.OpenPath = path;
        }
        public void DrawTranparentBackground(PaintEventArgs pe)
        {
            int portion = (int)(this.Width * .15);
            pe.Graphics.FillEllipse(myOpaqueBrush, 0, 0, this.Width, this.Height);//draw the transparent backgroung
            pe.Graphics.DrawEllipse(myPen, 2, 2, this.Width-5, this.Height-5);//draw border
            int myWidth = (int)(this.Width * .1); int myHeight = (int)(this.Height * .1);//get the width and height for the center solid circle
            pe.Graphics.FillEllipse(mySolidBrush, this.Width/2 - myWidth/2, this.Height/2 - myHeight/2, myWidth, myHeight);//draw center solid circle
            pe.Graphics.DrawLine(myPen,portion,portion,this.Width-portion,this.Height-portion);//draw divider line
            pe.Graphics.DrawLine(myPen, portion, this.Height - portion, this.Width - portion, portion);//draw divider line
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            DrawTranparentBackground(pe);
        }
        public void SetLocation(Point location)
        {
            this.Location = location;//set location of the popup menu
        }
        public void SetBackground(Bitmap background)
        {
            this.BackgroundImage = background;
        }
        #endregion

        private void buttonbuttonOpen_Click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlaySound();
            #endregion
            #region JOSH UPDATE
            this.SendToBack();
            this.Visible = false;//hide menu once a selection is made
            #endregion

            if (File.Exists(OpenPath))
            {
                try
                {
                    Process.Start(@"" + OpenPath);
                }
                catch (Exception E)
                {
                    Console.WriteLine("Can't open this file.");
                }
            }
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlaySound();
            #endregion
            #region JOSH UPDATE
            this.SendToBack();
            this.Visible = false;//hide menu once a selection is made
            #endregion
            
            DeletePath = OpenPath;
            DBS.Location = new Point((this.Parent.FindForm().Width - DBS.Width) / 2, (this.Parent.FindForm().Height - DBS.Height) / 2);
            this.Parent.FindForm().Controls.Add(DBS);
            DBS.Visible = false;
            DBS.Visible = true;
            DBS.DPath = DeletePath;
            this.Parent.FindForm().Controls.SetChildIndex(DBS, 2);
        }
        #region SIRISHA UPDATE
        private void button_Copy_Click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlaySound();
            #endregion
            #region JOSH UPDATE
            copyBufferLoaded = true;//this is checked later in order to enable paste option
            this.SendToBack();
            this.Visible = false;//hide menu once a selection is made
            #endregion
            CopySourcePath = OpenPath;
            
        }

        // Code referred from http://stackoverflow.com/questions/1974019/folder-copy-in-c-sharp
        private void copyDirectory(string strSource, string strDestination)
        { 
            if (!Directory.Exists(strDestination))
            {
                Directory.CreateDirectory(strDestination);
            }
         
            DirectoryInfo dirInfo = new DirectoryInfo(strSource);

            Directory.CreateDirectory(strDestination);
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo tempfile in files)
            {
                tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name), true);
            }
            DirectoryInfo[] dirctororys = dirInfo.GetDirectories();
            foreach (DirectoryInfo tempdir in dirctororys)
            {
                copyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
            }

        }

        // code referred from http://msdn.microsoft.com/en-us/library/cc148994.aspx
        private void button_paste_Click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlaySound();
            #endregion
            #region JOSH UPDATE
            this.SendToBack();
            this.Visible = false;//hide menu once a selection is made
            #endregion

            string fileName = Path.GetFileName(CopySourcePath);
            string targetPath = PastePath;
            string destFile = System.IO.Path.Combine(targetPath, fileName);
            try
            {
                System.IO.File.Copy(OpenPath, destFile, true);
                #region JOSH UPDATE
                //BrowserPanel temp = (BrowserPanel)this.Parent.FindForm().GetChildAtPoint(new Point(100, 310));//get access to sub panel from main form
                //temp.CreateItemByPath(destFile);//adding the conrol to the browser panel
                parent.CreateItemByPath(destFile);//adding the conrol to the browser panel
                copyBufferLoaded = false;
                #endregion
            }
            catch (Exception E)
            {
                MessageBox.Show("You cannot paste that file here. There is a file of the same name already in this folder. Paste it somewhere else.","Unauthorized Paste Action",mButtons,mIcon);
            }
        }
        #endregion
        #region JOSH UPDATE
        private void button_Copy_MouseEnter(object sender, EventArgs e)
        {
            this.button_Copy.FlatAppearance.BorderSize = 3;
            this.button_Copy.FlatAppearance.BorderColor = Color.Blue;
            this.labelCopy.BackColor = Color.Blue;
            this.labelCopy.ForeColor = Color.White;
        }

        private void button_Copy_MouseLeave(object sender, EventArgs e)
        {
            this.button_Copy.FlatAppearance.BorderSize = 0;
            this.labelCopy.BackColor = Color.White;
            this.labelCopy.ForeColor = Color.Black;
        }

        private void button_Delete_MouseEnter(object sender, EventArgs e)
        {
            this.button_Delete.FlatAppearance.BorderSize = 3;
            this.button_Delete.FlatAppearance.BorderColor = Color.Blue;
            this.labelDelete.BackColor = Color.Blue;
            this.labelDelete.ForeColor = Color.White;
        }

        private void button_Delete_MouseLeave(object sender, EventArgs e)
        {
            this.button_Delete.FlatAppearance.BorderSize = 0;
            this.labelDelete.BackColor = Color.White;
            this.labelDelete.ForeColor = Color.Black;
        }

        private void button_Paste_MouseEnter(object sender, EventArgs e)
        {
            this.button_Paste.FlatAppearance.BorderSize = 3;
            this.button_Paste.FlatAppearance.BorderColor = Color.Blue;
            this.labelPaste.BackColor = Color.Blue;
            this.labelPaste.ForeColor = Color.White;
        }

        private void button_Paste_MouseLeave(object sender, EventArgs e)
        {
            this.button_Paste.FlatAppearance.BorderSize = 0;
            this.labelPaste.BackColor = Color.White;
            this.labelPaste.ForeColor = Color.Black;
        }

        private void button_Open_MouseEnter(object sender, EventArgs e)
        {
            this.button_Open.FlatAppearance.BorderSize = 3;
            this.button_Open.FlatAppearance.BorderColor = Color.Blue;
            this.labelOpen.BackColor = Color.Blue;
            this.labelOpen.ForeColor = Color.White;
        }

        private void button_Open_MouseLeave(object sender, EventArgs e)
        {
            this.button_Open.FlatAppearance.BorderSize = 0;
            this.labelOpen.BackColor = Color.White;
            this.labelOpen.ForeColor = Color.Black;
        }

        public void ToggleOptions(Boolean open,Boolean copy,Boolean paste,Boolean delete,ItemType type)
        {
            button_Open.Enabled = open;
            button_Copy.Enabled = copy;
            button_Paste.Enabled = paste;
            button_Delete.Enabled = delete;
            if (copyBufferLoaded && (type == ItemType.folder)) { button_Paste.Enabled = true; }

        }
        #endregion
    }
}
