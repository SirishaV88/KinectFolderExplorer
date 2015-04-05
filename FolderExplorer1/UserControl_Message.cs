using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderExplorer1
{
    public partial class UserControl_Message : UserControl
    {

        public static string message = "";
        public UserControl_Message()
        {
            InitializeComponent();
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
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //empty implementation
        }

        private void UserControl_Message_Load(object sender, EventArgs e)
        {
            label_Msg.Text = message; label_Msg.Invalidate();
        }
        private void UserControl_DirBrowserScreen_VisibleChanged(object sender, EventArgs e)
        {
           

        }

        private void UserControl_Message_VisibleChanged(object sender, EventArgs e)
        {
            label_Msg.Text = message;
            label_Msg.Invalidate();
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

        private void pictureBox_Close_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            #region Sirisha update
            PointerFeedback.PlaySound();
            #endregion
        }
    }
}
