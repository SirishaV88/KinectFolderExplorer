using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;


namespace FolderExplorer1
{
    public partial class BrowserPanel : Control
    {
        //ArrayList mItems = new ArrayList();
        Thread scrollUpdate;
        Point autoScrollPosition;
        Item selection;
        public BrowserPanel()
        {
            InitializeComponent();
            this.Controls.Add(flowLayoutPanel1);//this panel controls content
            this.Controls.Add(rightPanel);//hides possible scroll bars on the right
            this.Controls.Add(bottomPanel);//hides possible scroll bars on the left
            this.Controls.SetChildIndex(this.rightPanel, 0);//brings hider panel to front
            this.Controls.SetChildIndex(this.bottomPanel, 0);//brings hider panel to front
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        //METHODS
        public void SetSelection(Item item)
        {
            selection = item;
        }
        public Item GetSelection()
        {
            return selection;
        }
        public void SetPanelSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            //give desired dimensions to content panel
            flowLayoutPanel1.Width = width;
            flowLayoutPanel1.Height = height;
                      
            //give hider appropriate width to conceal scroll bar
            bottomPanel.Width = width;
            bottomPanel.Height = 20;
           
            //give hider appropriate height to conceal scroll bar
            rightPanel.Height = height;
            rightPanel.Width = 20;
           
        }
        public void SetPanelLocation(Point location)
        {
            this.Location = location;//places control in location relative to parent
            flowLayoutPanel1.Location = new Point(0, 0);//redundancy ensure content panel is aligned with control
        }
        public void SetPanelOrientation(System.Windows.Forms.FlowDirection direction)
        {
            // The content panel is populated dynamically, but the items are arranged within the control automatically.
            // This tells the panel which direction to populate the content (top to bottom, left to right). Do not try
            // to arrange items within the control manually.  If the orientation is not what you want, just change the 
            // orientation by using this method when the BrowerPanel is declared.
            flowLayoutPanel1.FlowDirection = direction;
        }
        public void SetAutoScrollPosition(Point position)
        {
            // This allows scrolling of the content programatically.
            flowLayoutPanel1.AutoScrollPosition = position;
        }
        public Point GetAutoScrollPosition()
        {
            // Get the current scroll position, relative to the control iteself.
            return flowLayoutPanel1.AutoScrollPosition;
        }
        
        public void SetBackgroundColor(Color color)
        {
            this.BackColor = color;// sets color of control
            flowLayoutPanel1.BackColor = color;// sets color of content panel
            rightPanel.BackColor = color;// sets color of hider to match content and provide seamless masking of scroll bars
            bottomPanel.BackColor = color;// sets color of hider to match content and provide seamless masking of scroll bars
        }
        public void AddItem(Control control)
        {
            flowLayoutPanel1.Controls.Add(control);
        }
        public void CreateItemByPath(string path)
        {
            Item pasteItem = new Item();//create new item to add to the panel
            pasteItem.SetImage(Properties.Resources.documentImage);//give the item an image, this can be done programatically according to type
            pasteItem.SetLabelBackColor(Color.Transparent);
            pasteItem.SetLabelTextColor(Color.White);
            pasteItem.SetPath(path);
            pasteItem.SetItemType(ItemType.file);//for demo we are assuming pasted items are files, but in real world this assumption is not valid
            pasteItem.SetSize(132, 135);//give the item size, originally 100, 100
            this.flowLayoutPanel1.Controls.Add(pasteItem);
            this.flowLayoutPanel1.Update();
        }
        public void WrapContents(Boolean value)
        {
            flowLayoutPanel1.WrapContents = value;// you should always wrap contents, this could probably be removed and kept true
        }
        public void Clear()
        {
            this.flowLayoutPanel1.Controls.Clear();//removes all controls for when the content is updated
        }
        public void RemoveSelection()//used for "deleting" file Items
        {
            if (selection != null)//if an item has been selected. this a redundancy to prevent attempted delete of null values
            {
                this.Controls.Remove(selection);//remove selected item from the panel
                selection.Dispose();//release the items resources
                this.Update();//refresh the panel to show that the item is removed
            }
        }
    }
}
