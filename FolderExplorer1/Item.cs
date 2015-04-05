using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

public enum ItemType { folder, file, unused }

namespace FolderExplorer1
{
    public partial class Item : System.Windows.Forms.Button
    {
        private string path;//full path of file or folder
        private string extension;//file extension
        private ItemType type;
        
        public Item()
        {
            InitializeComponent();
            this.type = ItemType.unused;
            path = "C:\\";//set the default path to something readable
            this.BackgroundImageLayout = ImageLayout.Stretch;//icon consumes full area of control
            //label1
            label1.Location = new Point(0,this.Height - 5);
            this.Controls.Add(label1);//add the label control so that it is visible
            this.Controls.SetChildIndex(label1, 0);
            //label2
            label2.Location = new Point(0, this.Height / 2);
            this.Controls.Add(label2);
            this.Controls.SetChildIndex(label2, 0);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
        
        //METHODS
        public void SetItemType(ItemType type)
        {
            this.type = type;
        }
        public ItemType GetItemType()
        {
            return type;
        }
        public string GetExtension()
        {
            return extension;
        }
        public void SetPath(String path)
        {
            this.path = path;//changes the path from default to input
            SetName(path);//extracts the file or folder name from the path
            this.Name = Path.GetFileName(path);
            this.extension = Path.GetExtension(path);
            label2.Text = extension.ToUpper();
            SetExtensionColor();
        }
        private void SetExtensionColor()
        {
            if (this.type == ItemType.folder)
            {
                this.label2.Visible = false;
            }
            else
            {
                //label2.ForeColor = Color.White;
                //image
                if (extension == ".png") { label2.BackColor = Color.Red; }
                else if (extension == ".jpg") { label2.BackColor = Color.Green; }
                else if (extension == ".gif") { label2.BackColor = Color.Purple; }
                else if (extension == ".pdf") { label2.BackColor = Color.Blue; }
                else if (extension == ".psd") { label2.BackColor = Color.Magenta; }
                //video
                else if (extension == ".wmv") { label2.BackColor = Color.Maroon; }
                else if (extension == ".mp4") { label2.BackColor = Color.HotPink; }
                else if (extension == ".swf") { label2.BackColor = Color.Orange; }
                else if (extension == ".flv") { label2.BackColor = Color.Indigo; }
                else if (extension == ".avi") { label2.BackColor = Color.DarkTurquoise; }
                //docs
                else if (extension == ".ppt") { label2.BackColor = Color.BlueViolet; }
                else if (extension == ".pptx") { label2.BackColor = Color.BlueViolet; }
                else if (extension == ".doc") { label2.BackColor = Color.Chartreuse; }
                else if (extension == ".docx") { label2.BackColor = Color.Chartreuse; }
                else if (extension == ".csv") { label2.BackColor = Color.Chocolate; }
                else if (extension == ".docx") { label2.BackColor = Color.Coral; }
            }   
        }
        public void SetImage(Bitmap image)
        {
            this.BackgroundImage = image;//sets the image for the item
        }
        public void SetSize(int width,int height)
        {
            this.Width = width;
            this.Height = height;
            label1.Width = width;
            label1.Height = (int)(height * .15);//keeps the label at 10% of the total heigh
            //change label font size according to control size
            Font temp = new Font(label1.Font.Name,(int)(label1.Height*.5),label1.Font.Style);
            label1.Font = temp;
            
            //label2.Size = new Size(this.Width / 3, this.Height / 5);
            temp = new Font(label1.Font.Name, (int)(label2.Height), label2.Font.Style);
            label2.Font = temp;
            label2.Location = new Point((this.Width / 2) - (label2.Width / 2), this.Height / 2);
        }
        public void SetLocation(Point location)
        {
            this.Location = location;//sets the location relative to the panel which adds it
        }
        public string GetPath()
        {
            return path;//returns the directory path
        }
        public void SetText(string text)
        {
            this.label1.Text = text;//this is the text that appears at the bottom of the icon
        }
/*
 * Cady Modified
 */
        private void SetName(string path)
        {
            label1.Text = " " +Path.GetFileName(path); //added space
        }
        public void SetLabelBackColor(Color color)
        {
            label1.BackColor = color;
        }
        public void SetLabelTextColor(Color color)
        {
            label1.ForeColor = color;
        }
    }
}
