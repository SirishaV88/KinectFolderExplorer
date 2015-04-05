using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;

namespace FolderExplorer1
{
    
    public partial class UserControl_DirBrowserScreen : UserControl
    {
        #region JOSH UPDATE
        BrowserPanel parent;
        #endregion
        public  string DPath = "";
        UserControl_Message UCMsg = new UserControl_Message();

        public UserControl_DirBrowserScreen()
        {
            InitializeComponent();
            this.Disposed += UserControl_DirBrowserScreen_Disposed;

          
        }
        #region JOSH UPDATE
        public void SetParent(BrowserPanel parent)
        {
            this.parent = parent;
        }
        #endregion
        void UserControl_DirBrowserScreen_Disposed(object sender, EventArgs e)
        {
            timer_To_Send_Option_Menu_to_Back.Stop();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            this.BackColor = Color.Transparent;
            Bitmap bmBack = new Bitmap("DirBrowserBackground.png");
   
            e.Graphics.DrawImage(bmBack, 0, 50, this.Width, this.Height - 50);
            
            //Bitmap bmCloseButton = new Bitmap("close_button_T.png");
  
            //e.Graphics.DrawImage(bmCloseButton, this.Right-100, this.Top-100, 150, 150);
            

            base.OnPaint(e);
        }
       
        private void pictureBox_Close_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void UserControl_DirBrowserScreen_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == true)
            {
                foreach (Control c in this.Parent.FindForm().Controls) //assuming this is a Form
                {
                    if (c.Name == "UserControl_Menu_Options")
                    {
                        this.Parent.FindForm().Controls.SetChildIndex(c, -1);
                    }
                }
            }
            else
            {
                foreach (Control c in this.Parent.FindForm().Controls) //assuming this is a Form
                {
                    if (c.Name == "UserControl_Menu_Options")
                    {
                        this.Parent.FindForm().Controls.SetChildIndex(c, 2);
                    }
                }
            }

        }

        private void Button_Yes_Click(object sender, EventArgs e)
        {

            if (File.Exists(DPath))
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(DPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                this.Parent.FindForm().Update();
                this.Parent.FindForm().Invalidate();
            }
            else if (Directory.Exists(DPath))
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(DPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                this.Parent.FindForm().Update();
                this.Parent.FindForm().Invalidate();
            }

            this.Visible = false;

            
            UCMsg.Location = new Point((this.Parent.FindForm().Width - UCMsg.Width) / 2, (this.Parent.FindForm().Height - UCMsg.Height) / 2);
            this.Parent.FindForm().Controls.Add(UCMsg);
            UCMsg.Visible = false;
            UserControl_Message.message = "The file was moved to Recycle Bin seccessfully!!!";
            UCMsg.Visible = true;
            
            this.Parent.FindForm().Controls.SetChildIndex(UCMsg, 2);

            #region JOSH UPDATE
            //BrowserPanel temp = (BrowserPanel)this.Parent.FindForm().GetChildAtPoint(new Point(100, 310));//get access to sub panel from main form
            //Console.WriteLine(temp.GetSelection().Name.ToString());//delete control from subpanel
            //temp.RemoveSelection();//remove the Item from the panel that corresponds to the delete path. This item was identified when it was clicked.         
            parent.RemoveSelection();//remove the Item from the panel that corresponds to the delete path. This item was identified when it was clicked.         
            #endregion
            
            
            //UCMsg.Visible = false;

            //foreach (Control c in this.Parent.FindForm().Controls) //assuming this is a Form
            //{
            //    if (c.Name == "UserControl_Message")
            //    {
            //        c.Visible = true;

            //        this.Parent.FindForm().Controls.SetChildIndex(c, 2);
                    
            //        UserControl_Message.message = "The file was moved to Recycle Bin seccessfully!!!";

                    
            //    }
            //}

            #region Sirisha update
            PointerFeedback.PlaySound();
            #endregion

        }

        private void Button_No_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            #region Sirisha update
            PointerFeedback.PlaySound();
            #endregion
        }
    }
}
