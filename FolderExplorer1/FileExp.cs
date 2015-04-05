using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using Microsoft.Kinect;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.Reflection;

namespace FolderExplorer1
{ 
    public partial class FileExp : Form
    {
        #region JOSH UPDATE
        //available cursors
        Cursor cursorLeft; Cursor cursorLeftCopy;
        Cursor cursorRight; Cursor cursorRightCopy;
        
        //error messages
        MessageBoxIcon mIcon = new MessageBoxIcon();
        MessageBoxButtons mButtons = new MessageBoxButtons();
        //help box
        HelpScreen helpBox = new HelpScreen();
        //current directory
        string currentDirectory = "C:\\";
        //popup menu:
        UserControl_Menu_Options popUp = new UserControl_Menu_Options();
        ScreenCopier SC = new ScreenCopier();
        //panels:
        BrowserPanel mainPanel = new BrowserPanel();// the static upper "main" panel
        BrowserPanel subPanel = new BrowserPanel();// the lower dynamic "sub" panel
        int screenWidth = 0;// set later to the form width when in full screen, used for positioning the panels
        int screenHeight = 0;// set later to the form height when in full screen, used for positioning the panels
        //scrolling variables:
        Thread thread_mainScrollLeft;// checks for left scrolling of the upper panel
        Thread thread_mainScrollRight;// checks for right scrolling of the upper panel
        Thread thread_subScrollLeft;// checks left scroll of the lower panel
        Thread thread_subScrollRight;// checks right scroll of the lower panel
        Boolean msLeft = false;// activates/deactivates upper panel left scroll, [m]ain [s]croll [Left]
        Boolean msRight = false;// activates/deactivates upper panel right scroll
        Boolean ssLeft = false;// activates/deactivates lower panel left scroll
        Boolean ssRight = false;// activates/deactivates lower panel right scroll
        Boolean runMSLeftThread = true;// should the thread run or quit
        Boolean runMSRightThread = true;// should the thread run or quit
        Boolean runSSLeftThread = true;// should the thread run or quit
        Boolean runSSRightThread = true;// should the thread run or quit
        int mainPanelY = 0;// main panel y autoscroll position 
        int mainPanelX = 0;// main panel x autoscroll position 
        int subPanelY = 0;// sub panel y autoscroll position 
        int subPanelX = 0;// sub panel y autoscroll position 
        int mainScrollSpeed = 3;// set the main panel scroll speed here
        int subScrollSpeed = 3;// set sub panel scroll speed here 
        //hand tracking variables:
        KinectManager KM = new KinectManager();// creates new instance of kinect manager used for tracking hands/gestures
        Boolean cursorTracking = true;// used for connecting and disconnecting kinect from mouse pointer
        Thread thread_updateCursorPosition;// puts the cursor in the correct screen location based on hand position
        Boolean runCursorPostionThread = true;
        public delegate void DelegateUpdateCursorImage();
        public DelegateUpdateCursorImage setCursorImageByActiveHand;
        //delegate to allow panel scroll positions to be updated from scroll threads:
        public delegate void DelegateUpdateMainScrollPositions(Point position);
        public DelegateUpdateMainScrollPositions scrollMain;
        public delegate void DelegateUpdateSubScrollPositions(Point position);
        public DelegateUpdateSubScrollPositions scrollSub;
        #endregion
        
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]

        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;

            #region JOSH UPDATE
            //dispose of your handles or they clog up the GDI+ allocations
            //causing crash
            Disposer D;
            public void Dispose()
            {
                D = new Disposer();
                D.DeleteObjectG(hbmColor);
                D.DeleteObjectG(hbmMask);

                //trouble shooting print
                //Console.WriteLine(D.DeleteObjectG(hbmColor).ToString());
                //Console.WriteLine(D.DeleteObjectG(hbmMask).ToString());
            }
            #endregion
        }

        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            Disposer D = new Disposer();//help dispose of GDI objects/handles
            
            //construct the icon info:
            IntPtr bmpPtr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(bmpPtr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;

            //construct the cursor:
            bmpPtr = CreateIconIndirect(ref tmp);
            
            //release resources:
            tmp.Dispose();
            //D.DestroyCursorU(bmpPtr);

            //return cursor:
            return new Cursor(bmpPtr);
            //return bmpPtr;
        }

        Disposer D = new Disposer();
        private void SetCursorImage(Image image,int width,int height,int hotSpotX,int hotSpotY)
        {
            //Cursor oldCursor = this.Cursor;
            this.Cursor.Dispose();
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(image, 0, 0, width, height);
                g.Dispose();
            }
            
            //construct the icon info:
            IntPtr bmpPtr = bitmap.GetHicon();//this needs to be disposed of
            IconInfo tmp = new IconInfo();
            GetIconInfo(bmpPtr, ref tmp);
            tmp.xHotspot = hotSpotX;
            tmp.yHotspot = hotSpotY;
            tmp.fIcon = false;
            //cunstruct the cursor:
            IntPtr icoPtr = CreateIconIndirect(ref tmp);
            this.Cursor = new Cursor(icoPtr);
            
            
            //free GDI objects:            
            bitmap.Dispose();
            tmp.Dispose();
            D.DestroyCursorU(bmpPtr);
            //oldCursor.Dispose();

            //print
            //Console.WriteLine(D.(icoPtr).ToString());
            
        }
        public FileExp()
        {
            InitializeComponent();

            this.AutoScroll = false;
            this.HorizontalScroll.Enabled = false;
            this.VerticalScroll.Enabled = false;

            #region JOSH UPDATE
            //cursor image updater
            this.timerUpdateCursorImage.Start();
            //error messages
            mIcon = MessageBoxIcon.Error;
            mButtons = MessageBoxButtons.OK;
            //help box
            helpBox.Visible = false;
            this.Controls.Add(helpBox);
            //cursor image
            SetCursorImage(Properties.Resources.pointLeft,80,80,52,10);
            //pupop menu:
            popUp.Visible = false;
            this.Controls.Add(popUp);
            this.Controls.SetChildIndex(popUp, 0);
            
            //KINECT MANAGER:
            KM.GE.SET_THREADSLEEP(1);//set the sleep for gesture evaluation
            KM.JR.SET_THREADSLEEP(1);//set the sleep for jitter reduction
            KM.START(); // start kinect manager and child threads
            if (KM.connected)
            {
                KM.SET_TRACKING_RANGE(1100.0, 4000.0); // set near and far tracking limits
                KM.SET_NEARHAND_ACTIVE(true); // start tracking near hand gesture
            }
            thread_updateCursorPosition = new Thread(new ThreadStart(UpdateCursorPositionUsingKinect));
            thread_updateCursorPosition.Start();
            
            //FORM WINDOW SETUP:
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // expand window to encompass full screen, and eliminate resize options
            this.WindowState = FormWindowState.Maximized; // start form maximized
            this.KeyPreview = true; // get key strokes so user can quite on esacpe key down
            this.DoubleBuffered = true; // use back buffering to further eliminate flickering
            screenWidth = Screen.FromControl(this).Bounds.Width;// get screen width, used later to setup panel sizes
            screenHeight = Screen.FromControl(this).Bounds.Height;// get screen height, used later to setup panel sizes
            subPanel.flowLayoutPanel1.ControlAdded += new ControlEventHandler(LinkNewItemToOperation);//give the item an action
            Setup();
            //SCROLLING THREADS SETUP AND START:
            thread_mainScrollLeft = new Thread(new ThreadStart(MSLeft));
            thread_mainScrollLeft.Start();
            thread_mainScrollRight = new Thread(new ThreadStart(MSRight));
            thread_mainScrollRight.Start();
            thread_subScrollLeft = new Thread(new ThreadStart(SSLeft));
            thread_subScrollLeft.Start();
            thread_subScrollRight = new Thread(new ThreadStart(SSRight));
            thread_subScrollRight.Start();
            //CROSS THREADING DELEGATE SETUP:
            scrollMain = new DelegateUpdateMainScrollPositions(this.ScrollMainPanel);
            scrollSub = new DelegateUpdateSubScrollPositions(this.ScrollSubPanel);
            //allow clicking of subpanel
            subPanel.flowLayoutPanel1.MouseClick += new System.Windows.Forms.MouseEventHandler(SubPanel_click);
            popUp.MouseMove += new System.Windows.Forms.MouseEventHandler(HidePopup);
            //POPUP MENU APPEARANCE SETUP:
            popUp.SetBackColor(Color.White);//set the popup menu color
            popUp.SetBackOpacity(200);//opacity
            popUp.SetBorderColor(Color.DarkBlue);//border color
            popUp.SetParent(subPanel);
            timerHidePopup.Start();
            //cursors:
            BuildCursors();
            #endregion
        }

        #region JOSH UPDATE
        public void BuildCursors()
        {
            //cursorLeft = new Cursor(CreateCursor(Properties.Resources.pointLeft2, 52, 10));
            //cursorLeftCopy = new Cursor(CreateCursor(Properties.Resources.pointLeftCopy2, 52, 10));
            //cursorRight = new Cursor(CreateCursor(Properties.Resources.pointRight2, 28, 10));
            //cursorRightCopy = new Cursor(CreateCursor(Properties.Resources.pointRightCopy2, 28, 10)); 

            cursorLeft = CreateCursor(Properties.Resources.pointLeft2, 52, 10);
            cursorLeftCopy = CreateCursor(Properties.Resources.pointLeftCopy2, 52, 10);
            cursorRight = CreateCursor(Properties.Resources.pointRight2, 28, 10);
            cursorRightCopy = CreateCursor(Properties.Resources.pointRightCopy2, 28, 10); 
            
            Console.WriteLine(cursorLeft.Handle.ToString());
            Console.WriteLine(cursorLeftCopy.Handle.ToString());
            Console.WriteLine(cursorRight.Handle.ToString());
            Console.WriteLine(cursorRightCopy.Handle.ToString());
        }
        public void LinkNewItemToOperation(object sender,ControlEventArgs e)
        {
            Item temp = (Item)e.Control;
            string tempstr = temp.GetExtension();
            if(temp.GetItemType() == ItemType.folder)
            {
                temp.MouseClick += new MouseEventHandler(FolderClick);
            }
            else if (temp.GetItemType() == ItemType.file)
            {
                temp.MouseClick += new MouseEventHandler(ShowPopupMenu);
                temp.label2.MouseClick += new MouseEventHandler(ShowPopupMenu);
            }
            
        }
        public void SetupBrowserPanels()
        {
            //Miscellaneous:
            //Abdul's Panel:
            panel2.Width = (int)(subPanel.Width * .5);
            panel2.Location = new System.Drawing.Point(subPanel.Location.X + 5, subPanel.Location.Y + subPanel.Height - 5);
            //application title:
            this.titleImage.Location = new Point(300, 0);
            this.titleImage.Size = new System.Drawing.Size(screenWidth - 600, 80);
            this.titleImage.SendToBack();
            this.titleImage.Select();
            //close button:
            this.buttonClose.Size = new System.Drawing.Size(200, 65);
            this.buttonClose.Location = new Point((screenWidth - 100) - this.buttonClose.Width,0);
            //help button:
            this.buttonHelp.Size = new System.Drawing.Size(200, 65);
            this.buttonHelp.Location = new Point((screenWidth - 100) - this.buttonClose.Width, 65);
            //back button:
            buttonBack.Location = new Point(100, 0);//back button location
            buttonBack.Size = new System.Drawing.Size(200, 130);
            //file path text box:
            FilePath.Location = new Point(300, 75);//set location for file path text box
            FilePath.Size = new System.Drawing.Size(screenWidth - 600, 100);
            FilePath.Text = "C:\\";//set initial text
            
            //main panel setup:
            //mainPanel.WrapContents = true;
            mainPanel.SetPanelOrientation(FlowDirection.LeftToRight);//sets the orientation for populating the panel and wrapping content
            mainPanel.SetPanelLocation(new Point(100, 130));//places the panel at the defined position in the form
            mainPanel.SetPanelSize(screenWidth - 200, 175);//defines the panel size
            mainPanel.SetBackgroundColor(Color.Black);//defines the panel background color
            this.Controls.Add(mainPanel);//adds the panel to the main form/window
            //scroll buttons for main panel; these are children of the form, not children of the panel, they just conrol the panel:
            mainScrollLeft.Location = new Point(0, 0);//set location for the left scroll button for main panel
            mainScrollLeft.Width = 100;//set width
            mainScrollLeft.Height = 305;//set height
            mainScrollRight.Location = new Point(screenWidth - 100, 0);//set location of the right scroll button for main panel
            mainScrollRight.Width = 100;//set width
            mainScrollRight.Height = 305;//set height
            
            //sub panel setup:
            subPanel.WrapContents(true);//allows wrapping content automatically, leave this true
            subPanel.SetPanelOrientation(FlowDirection.TopDown);//set panel orientation for autopopulate and content wrapping
            subPanel.SetPanelLocation(new Point(100, 310));//put the panel at location in form
            subPanel.SetPanelSize(screenWidth - 200, screenHeight - 310);//give the panel dimensions
            subPanel.SetBackgroundColor(Color.Black);//set panel background color
            this.Controls.Add(subPanel);//add the panel to the main form/window
            //scroll buttons for sub panel:
            subScrollLeft.Location = new Point(0, 310);//sub panel left scroll button location
            subScrollLeft.Width = 100;//button width
            subScrollLeft.Height = screenHeight - 310;//button height

            subScrollRight.Location = new Point(screenWidth - 100, 310);//right scroll button location
            subScrollRight.Width = 100;//button width
            subScrollRight.Height = screenHeight - 310;//button height
            
            //process default directory
            InitializeSubPanelContent("C:\\");//initially populates the sub panel with content from the root directory
        }
        
        public void InitializeMainPanelContents()
        {    
            //get the main path values for the main panel content items/icons
            string path_Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path_MyComputer = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string path_MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path_MyMusic = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string path_MyPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path_MyVideos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string path_NetworkShortcuts = Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts);
            string path_ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string path_Recent = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            string path_StartMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            //put common paths in an array to simplify initial population of main panel:
            string[] path = new string[11];
            path[0] = "C:\\";
            path[1] = path_StartMenu;
            path[2] = path_Desktop;
            path[3] = path_MyComputer;
            path[4] = path_MyDocuments;
            path[5] = path_MyMusic;
            path[6] = path_MyPictures;
            path[7] = path_MyVideos;
            path[8] = path_NetworkShortcuts;
            path[9] = path_ProgramFiles;
            path[10] = path_Recent;
            
            //populate main panel with initial content
            //As we have it currently set, this content is static but it doesn't have to be.
            //Simply populate the panel when and with what you like based on the functions
            //in the following loop.
            for(int i =0;i < 11;i++)
            {
                Item temp = new Item();//define a new item (this is like an icon that you can click to navigate)
                temp.SetPath(path[i]);//give the item a path
                temp.SetSize(150, 150);//give the item/icon size
/*
* Cady added
*/
                temp.SetLabelBackColor(Color.Transparent);
                if (i == 2)
                    temp.SetImage(new Bitmap(Properties.Resources.desktopImage));//give the item an image, in this case a destop image
                else if (i == 3)
                    temp.SetImage(new Bitmap(Properties.Resources.myComputerImage));//give the item an image, in this case a computer image
                else if (i == 4)
                    temp.SetImage(new Bitmap(Properties.Resources.myDocumentsImage));//give the item an image, in this case a documents image
                else if (i == 5)
                    temp.SetImage(new Bitmap(Properties.Resources.musicImage));//give the item an image, in this case a music image
                else if (i == 6)
                    temp.SetImage(new Bitmap(Properties.Resources.pictureImage));//give the item an image, in this case a picture image
                else if (i == 7)
                    temp.SetImage(new Bitmap(Properties.Resources.videoImage));//give the item an image, in this case a video image
                else if (i == 8)
                    temp.SetImage(new Bitmap(Properties.Resources.shortcutsImage));//give the item an image, in this case a generic image
                else if (i == 9)
                    temp.SetImage(new Bitmap(Properties.Resources.programFiles));//give the item an image, in this case a generic image
                else if (i == 10)
                    temp.SetImage(new Bitmap(Properties.Resources.RecentDocuments));//give the item an image, in this case a recent docs image
                else if (i == 1)
                    temp.SetImage(new Bitmap(Properties.Resources.startmenu));//give the item an image, in this case a generic image
                else if (i == 0)
                {
                    temp.SetImage(new Bitmap(Properties.Resources.driverImage));//give the item an image, in this case a generic image
                    temp.SetText(" C:\\");
                }

                else
                    temp.SetImage(new Bitmap(Properties.Resources.folder));
                temp.MouseClick += new System.Windows.Forms.MouseEventHandler(FolderClick);//tell the item what to do when clicked
                mainPanel.AddItem(temp);//add the item to the main panel
            }
        }
        public void FolderClick(Object sender, System.EventArgs e)
        {
            #region Sirisha Update
            PointerFeedback.PlayClickSound();
            #endregion
            Item temp = (Item)sender;//type cast the sender to an item so you can access its data
            FilePath.Text = temp.GetPath();//set the file path text box text to reflect the navigation
            popUp.SetPastePath(FilePath.Text);
            ProcessDirectory(temp.GetPath());//process the file path in order to populate the sub panel
        }
        public Boolean InitializeSubPanelContent(string targetDirectory)
        {
            //disable back button if at root directory:
            if (targetDirectory == "C:\\")
            {
                buttonBack.Enabled = false;
            }
            else { buttonBack.Enabled = true; }

            //don't process directory if already in directory:


            string[] files;//list of files in target directory
            string[] directories;//list of folders in target directory 
            // Process the list of files found in the directory:
            try
            {
                files = Directory.GetFiles(targetDirectory);
            }
            catch (UnauthorizedAccessException e)
            {
                // If you do not have access to the file, this exception is thrown.  Any custom error messages or actions need to be
                // placed here.
                return false;
            }
            // Process the list of folders found in the directory:
            try
            {
                directories = Directory.GetDirectories(targetDirectory);
            }
            catch (UnauthorizedAccessException e)
            {
                // If you do not have access to the folder, this exception is thrown.  Any custom error messages or actions need to be
                // placed here.
                return false;
            }
            UpdateSubPanelContents(directories, files);//populate the sub panel with the files and folders
            return true;
        }
        public Boolean ProcessDirectory(string targetDirectory)
        {
            if (targetDirectory != currentDirectory)
            {
                currentDirectory = targetDirectory;
                //disable back button if at root directory:
                if (targetDirectory == "C:\\")
                {
                    buttonBack.Enabled = false;
                }
                else { buttonBack.Enabled = true; }

                //don't process directory if already in directory:


                string[] files;//list of files in target directory
                string[] directories;//list of folders in target directory 
                // Process the list of files found in the directory:
                try
                {
                    files = Directory.GetFiles(targetDirectory);
                }
                catch (UnauthorizedAccessException e)
                {
                    // If you do not have access to the file, this exception is thrown.  Any custom error messages or actions need to be
                    // placed here.
                    MessageBox.Show("You do not have access to this folder. Try opening another folder.","Access Denied",mButtons,mIcon);
                    return false;
                }
                // Process the list of folders found in the directory:
                try
                {
                    directories = Directory.GetDirectories(targetDirectory);
                }
                catch (UnauthorizedAccessException e)
                {
                    // If you do not have access to the folder, this exception is thrown.  Any custom error messages or actions need to be
                    // placed here.
                    return false;
                }
                UpdateSubPanelContents(directories, files);//populate the sub panel with the files and folders
                return true;
            }
            return false;
        }
        public void UpdateSubPanelContents(string[] folders, string[] files)
        {
            //reset the autoscroll position when the content is updated:
            mainPanelX = 0;
            mainPanelY = 0;
            subPanelX = 0;
            subPanelY = 0;
            //update sub panel content
            int folderQty = folders.Length;
            int fileQty = files.Length;
            subPanel.Clear();//remove old items from subPanel
            //
            //Folders
            //
/*
* Cady modified
*/
            for (int i = 0; i < folderQty; i++)//make new item for each folder
            {
                Item temp = new Item();//create new item
                temp.SetItemType(ItemType.folder);
                temp.SetPath(folders[i].ToString());//give the item a folder path
                temp.SetSize(132,135);//give the item size, originally 100, 100
                temp.SetImage(Properties.Resources.folder);//give the item an image
                temp.SetLabelBackColor(Color.Transparent);
                temp.SetLabelTextColor(Color.White);
                subPanel.AddItem(temp);//add the item to the sub panel, the panel will decide where to put it
            }
            //
            //Files
            //
            for (int i = 0; i < fileQty; i++)//make a new item for each file
            {
                Item temp = new Item();//make new item
                temp.SetItemType(ItemType.file);
                temp.SetPath(files[i].ToString());//give the item a file path
                temp.SetSize(132, 135);//give the item size, originally 100, 100
                temp.SetImage(Properties.Resources.documentImage);//give the item an image, this can be done programatically according to type
                temp.SetLabelBackColor(Color.Transparent);
                temp.SetLabelTextColor(Color.White);
                subPanel.AddItem(temp);//add the item to the sub panel, the panel will decide where to put it
            }
        }
        public string GoBackPath(string path)
        {
            if (path != "C:\\")//do not try to go back if you are at root. This is a redundant check. Button should not be enabled if at root anyway.
            {
                char[] pathArray = path.ToCharArray();//separate the path into characters
                int mLength = pathArray.Length;
                string name = null;
                for (int i = (mLength - 1); i > 0; i--)//traverse array backward to get name
                {
                    if (pathArray[i] == '\\')//when you get to the first slash then you know where the previous directory ends
                    {
                        for (int j = 0; j < i; j++)
                        {
                            name = name + pathArray[j];//record the file path except the last portion which is your current directory
                        }
                        i = 0;
                    }
                }
                if (name == "C:") name = "C:\\";//make sure you will not try and backup from root
                return name;//return the parent directory, the "go back" directory
            }
            
            return "C:\\";
        }
        public void Setup()
        {
            SetupBrowserPanels();//setup main and sub panels
            InitializeMainPanelContents();//populate main panel
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            #region Sirisha Update
            PointerFeedback.PlayClickSound();
            #endregion
            string currentPath = FilePath.Text.ToString();//get the current path
            string newPath = GoBackPath(currentPath);//get the parent directory
            ProcessDirectory(newPath);//go to the parent directory
            FilePath.Text = newPath;//update file path text bar to reflect the new directory location
        }

        //SCROLLING METHODS:
        public void ScrollMainPanel(Point potision)
        {
            mainPanel.SetAutoScrollPosition(potision);//set the autoscroll position
        }
        public void ScrollSubPanel(Point potision)
        {
            subPanel.SetAutoScrollPosition(potision);//set the autoscroll position
        }
        private void MSLeft()
        {
            Console.WriteLine("MSLeft Scroll Thread Started!");
            while (runMSLeftThread)
            {
                if (msLeft)//if the button was entered then this will be true, if button was left then this will be false
                {
                    mainPanelX = Math.Abs(mainPanel.GetAutoScrollPosition().X);//redundancy check to base scroll from current scroll position
                    mainPanelX = mainPanelX - mainScrollSpeed;//change scroll position
                    this.Invoke(this.scrollMain, new Object[] { new Point(mainPanelX, mainPanelY) });//force autoscroll
                }
                Thread.Sleep(10);
            }
        }
        private void MSRight()
        {
            Console.WriteLine("MSRight Thread Started!");
            while (runMSRightThread)
            {
                if (msRight)//if the button was entered then this will be true, if button was left then this will be false
                {
                    mainPanelX = Math.Abs(mainPanel.GetAutoScrollPosition().X);//redundancy check to base scroll from current scroll position
                    mainPanelX = mainPanelX + mainScrollSpeed;//change scroll position
                    this.Invoke(this.scrollMain, new Object[] { new Point(mainPanelX, mainPanelY) });//force autoscroll
                }
                Thread.Sleep(10);
            }
        }
        private void SSLeft()
        {
            Console.WriteLine("Scroll Thread Started!");
            while (runSSLeftThread)
            {
                if (ssLeft)//if the button was entered then this will be true, if button was left then this will be false
                {
                    subPanelX = Math.Abs(subPanel.GetAutoScrollPosition().X);//redundancy check to base scroll from current scroll position
                    subPanelX = subPanelX - subScrollSpeed;//change scroll position
                    this.Invoke(this.scrollSub, new Object[] { new Point(subPanelX, subPanelY) });//force autoscroll
                }
                Thread.Sleep(10);
            }
        }
        private void SSRight()
        {
            while (runSSRightThread)
            {
                if (ssRight)//if the button was entered then this will be true, if button was left then this will be false
                {
                    subPanelX = Math.Abs(subPanel.GetAutoScrollPosition().X);//redundancy check to base scroll from current scroll position
                    subPanelX = subPanelX + subScrollSpeed;//change scroll position
                    this.Invoke(this.scrollSub, new Object[] { new Point(subPanelX, subPanelY) });//force autoscroll
                }
                Thread.Sleep(10);
            }
        }
        private void mainScrollLeft_MouseEnter(object sender, EventArgs e)
        {
            msLeft = true;//allow associated thread to produce autoscroll effect
            //Console.WriteLine("Left Scroll: " + msLeft);
            
        }
        private void mainScrollLeft_MouseLeave(object sender, EventArgs e)
        {
            msLeft = false;//allow associated thread to produce autoscroll effect
            //Console.WriteLine("Left Scroll: " + msLeft);
        }
        private void mainScrollRight_MouseEnter(object sender, EventArgs e)
        {
            msRight = true;//allow associated thread to produce autoscroll effect
            //Console.WriteLine("Right Scroll: " + msRight);
        }
        private void mainScrollRight_MouseLeave(object sender, EventArgs e)
        {
            msRight = false;//allow associated thread to produce autoscroll effect
            //Console.WriteLine("Right Scroll: " + msRight);
        }
        private void subScrollLeft_MouseEnter(object sender, EventArgs e)
        {
            ssLeft = true;//allow associated thread to produce autoscroll effect
            //Console.WriteLine("Right Scroll: " + ssLeft);
        }
        private void subScrollLeft_MouseLeave(object sender, EventArgs e)
        {
            ssLeft = false;//allow associated thread to produce autoscroll effect
            //Console.WriteLine("Right Scroll: " + ssLeft);
        }
        private void subScrollRight_MouseEnter(object sender, EventArgs e)
        {
            ssRight = true;//allow associated thread to produce autoscroll effect
            //Console.WriteLine("Right Scroll: " + ssRight);
        }
        private void subScrollRight_MouseLeave(object sender, EventArgs e)
        {
            ssRight = false;//allow associated thread to produce autoscroll effect
            //Console.WriteLine("Right Scroll: " + ssRight);
        }
        private void FileExp_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Console.WriteLine("ESACPE");
                this.Close();//exit by pressing escape
            }
        }
        /// <summary>
        /// Simulates a click.
        /// </summary>
        private void GetMouseCommand()
        {
            if (KM.GET_NEAR_HAND_POINT())
            {
                if (KM.GET_NEARHAND_STATE2().active)
                {
                    MouseCommand.LeftDown();
                    MouseCommand.LeftUp();
                    KM.SET_NEARHAND_ACTIVE(false);
                }
            }
            else
            {
                KM.SET_NEARHAND_ACTIVE(true);
            }
        }
        /// <summary>
        /// Capture near hand position from kinect and translates it to screen coordinates for cursor.
        /// </summary>
        private void UpdateCursorPositionUsingKinect()
        {
            while (runCursorPostionThread)
            {
                //update cursor position based on hand locations and get gesture:
                if (KM.connected)
                {
                    if ((cursorTracking) && (KM.IS_IN_RANGE()))
                    {
                        Cursor.Position = KM.VS.GetHandFormPosition(0, 0, screenWidth, screenHeight, KM.GET_NEARHAND_DIP());
                        GetMouseCommand();
                    }
                }
                Thread.Sleep(1);
            }
        }
        /// <summary>
        /// Referenced: http://stackoverflow.com/questions/3306600/c-how-to-take-a-screenshot-of-a-portion-of-screen
        /// Reference: http://www.developerfusion.com/code/4630/capture-a-screen-shot/
        /// </summary>
        Image screenshot; Bitmap original; Rectangle srcRect;
        private void makePopupBackground(int x,int y)
        {
            screenshot = SC.GetScreenImage();//get screen shot
            original = new Bitmap(screenshot);//convert to bitmap object so it can be cropped
            srcRect = new Rectangle(x, y, popUp.Width, popUp.Height);//set the cropping area to the location where the menu will be
            try
            {
                popUp.SetBackground(original.Clone(srcRect, original.PixelFormat));//crop the image and set the popup back ground to the crop so drawing over it will make menu look transparent
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);//this will be out of memory exception
            }
            original.Dispose();
        }
        int popUpX; int popUpY;
        private void ShowPopupMenu(Object sender, System.EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlayClickSound();
            #endregion
            Item temp;// = (Item)sender;
            try
            {
                temp = (Item)sender;//cast sender into Item to get the Item's path
            }
            catch (Exception E)
            {
                Label mLabel = (Label)sender;
                temp = (Item)mLabel.Parent;//cast sender parent into Item to get the Item's path since the sender must have been the label
            }
            subPanel.SetSelection(temp);
            popUp.SetPath(temp.GetPath());//get the Item's path
            popUpX = (Cursor.Position.X - (int)(popUp.Width/2));//draw the popup menu around the cursor
            popUpY = (Cursor.Position.Y) - (int)(popUp.Height / 2);//draw the popup menu around the cursor
            //keep the menu completely on the screen
            if (popUpX < 0)
            {
                popUpX = 0;
            }
            else if (popUpX > (screenWidth - popUp.Width))
            {
                popUpX = screenWidth - popUp.Width;
            }
            if (popUpY < 0)
            {
                popUpY = 0;
            }
            else if (popUpY > (screenHeight - popUp.Height))
            {
                popUpY = screenHeight - popUp.Height;
            }
            popUp.Visible = false;//make the menu invisible first so the background can be captured
            subPanel.Update();//clear any residual popup menu imagery, this needs to be accomplished for any controls that are in range of the popup menu or residuals images may be left behind when the popup moves or closes
            popUp.SetLocation(new Point(popUpX,popUpY));//put the popup at the correct location
            makePopupBackground(popUpX, popUpY);//give the menu the correct background
            popUp.BringToFront();
            popUp.ToggleOptions(true, false, false, false,ItemType.file);//delete should be set to true in practice, but in the demo we don't want to actually delete any files
            popUp.Visible = true;//show the popup
            popUp.Update();//make sure the new background is shown and not the old background
        }
        private void subPanelPopup()
        {
            // This is the same as ShowPopupMenu except no path data is extracted because no item was clicked.
            // This is so if the user wants to paste, they can simply click on a blank area of the subpanel
            // and the popup menu will appear.
            popUpX = (Cursor.Position.X) - (int)(popUp.Width / 2);
            popUpY = (Cursor.Position.Y) - (int)(popUp.Height / 2);
            //keep the menu completely on the screen
            if (popUpX < 0)
            {
                popUpX = 0;
            }
            else if (popUpX > (screenWidth - popUp.Width))
            {
                popUpX = screenWidth - popUp.Width;
            }
            if (popUpY < 0)
            {
                popUpY = 0;
            }
            else if (popUpY > (screenHeight - popUp.Height))
            {
                popUpY = screenHeight - popUp.Height;
            }
            popUp.Visible = false;
            subPanel.Update();
            popUp.SetLocation(new Point(popUpX, popUpY));
            makePopupBackground(popUpX, popUpY);
            popUp.BringToFront();
            popUp.ToggleOptions(false, false, false, false,ItemType.folder);
            popUp.Visible = true;
            popUp.Update();
        }
        public void SubPanel_click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlayClickSound();
            #endregion
            subPanelPopup();
        }
        public void HidePopup(object sender, EventArgs e)
        {
            // This is executed while moving the mouse within the menu and by the timer.
            // It needs both forms of execution because there are instances when the mouse can exit the 
            // menu without firing any movement or leaving events.  In such instnces the timer provides
            // a fail safe that will ensure the menu turns off.  The movement events are more precise 
            // though and should not be completely replace by the timer.

            if (popUp.Visible)//only execute if the popup has been activated
            {
                Rectangle rectPopup = popUp.Bounds;//get the bounding box of the menu
                Rectangle rectCursor = new Rectangle(Cursor.Position, Cursor.Size);//get the bounding box of the cursor
                double value = (Math.Sqrt((Cursor.Position.X - (popUp.Location.X + (popUp.Width / 2))) * (Cursor.Position.X - (popUp.Location.X + (popUp.Width / 2))) + (Cursor.Position.Y - (popUp.Location.Y + (popUp.Height / 2))) * (Cursor.Position.Y - (popUp.Location.Y + (popUp.Height / 2)))));//get the bounding box intersections
                if((Rectangle.Intersect(rectPopup, rectCursor).Size.Width == 0)||(value > (popUp.Width / 2)))//if there is no intersection then the cursor has left the menu and it can disappear
                {
                    popUp.Visible = false;//turn off the menu
                }
            }
        }
        private void timerHidePopup_Tick(object sender, EventArgs e)
        {
            HidePopup(sender, e);
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlayClickSound();
            #endregion
            this.Close();            
        }
        /// <summary>
        /// Stops threads on close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileExp_FormClosing(object sender, FormClosingEventArgs e)
        {
            runMSLeftThread = false;// stop thread
            runMSRightThread = false;// stop thread
            runSSLeftThread = false;// stop thread
            runSSRightThread = false;// stop thread
            runCursorPostionThread = false;// stop thread
            if (KM.connected) { KM.STOP(); }//stop all of the kinect manager threads
        }
        #endregion

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            #region Sirisha update
            PointerFeedback.PlayClickSound();
            #endregion
            helpBox.Size = new Size(screenWidth / 2, screenHeight / 2);
            helpBox.Location = new System.Drawing.Point((screenWidth / 2) - (helpBox.Width / 2), (screenHeight / 2) - (helpBox.Height / 2));
            helpBox.ArrangeControls();
            helpBox.Visible = true;
            this.Controls.SetChildIndex(helpBox, 0);
        }

        private void timerUpdateCursorImage_Tick(object sender, EventArgs e)
        {
            if (popUp.copyBufferLoaded)
            {
                //set cursor image depending on which hand is near:
                if (KM.GET_NEARHAND_STATE2().left)
                {
                    //SetCursorImage(Properties.Resources.pointLeftCopy, 80, 80, 52, 10);
                    this.Cursor.Dispose();
                    this.Cursor = new Cursor(cursorLeftCopy.Handle);
                }
                else
                {
                    //SetCursorImage(Properties.Resources.pointRightCopy, 80, 80, 28, 10);
                    this.Cursor.Dispose();
                    this.Cursor = new Cursor(cursorRightCopy.Handle);
                }
            }
            else
            {
                //set cursor image depending on which hand is near:
                if (KM.GET_NEARHAND_STATE2().left)
                {
                    //SetCursorImage(Properties.Resources.pointLeft, 80, 80, 52, 10);
                    this.Cursor.Dispose();
                    this.Cursor = new Cursor(cursorLeft.Handle);
                }
                else
                {
                    //SetCursorImage(Properties.Resources.pointRight, 80, 80, 28, 10);
                    this.Cursor.Dispose();
                    this.Cursor = new Cursor(cursorRight.Handle);
                }
            }
        }
    }
}
