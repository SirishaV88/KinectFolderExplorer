using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderExplorer1
{
    public partial class HelpScreen : Control
    {
        Random random = new Random();
        int randomX;
        int randomY;
        public HelpScreen()
        {
            InitializeComponent();
            this.Controls.Add(pictureBox1);
            this.Controls.Add(pictureBox2);
            this.Controls.Add(textBox1);
            this.Controls.Add(textBox2);
            this.Controls.Add(button1);
            this.Controls.Add(button2);
            this.Controls.Add(label1);
            this.Controls.SetChildIndex(label1, 0);
            ArrangeControls();
        }
        public void ArrangeControls()
        {
            pictureBox1.Location = new Point(0,label1.Location.Y + label1.Height + 10);
            pictureBox1.Size = new Size(100,100);
            textBox1.Location = new Point(100,pictureBox1.Location.Y);
            textBox1.Size = new Size(this.Size.Width - this.button1.Width - pictureBox1.Width, 100);

            pictureBox2.Location = new Point(0,pictureBox1.Location.Y + pictureBox1.Height + 10);
            pictureBox2.Size = new Size(100,100);
            textBox2.Location = new Point(100,pictureBox2.Location.Y);
            textBox2.Size = new Size(this.Size.Width - this.button1.Width - pictureBox1.Width,120);
            textBox2.Text = "Point at screen to flick the bug,"
            + " then quickly return to a fist. If the bug doesn't move,"
            + " try pointing longer before returning to a fist. When the"
            + " bug moves, flick it again for practice.";

            this.button1.Size = new Size(100, 100);
            this.button1.Location = new Point(this.Width - this.button1.Width, 0);

            this.button2.Size = new Size((int)(this.Height*.2), (int)(this.Height * .2));
            this.button2.Location = new Point(this.Width - button2.Width, this.Height - button2.Height);
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlayClickSound();
            #endregion
            this.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlayClickSound();
            #endregion
            randomX = random.Next(0, this.Width - button2.Width);
            randomY = random.Next(textBox2.Location.Y + textBox2.Height, this.Height - button2.Height);
            button2.Location = new Point(randomX, randomY);
        }
    }
}
