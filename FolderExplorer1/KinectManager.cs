using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using System.Threading;

namespace FolderExplorer1
{
    /// <summary>
    /// Issues mouse commands programatically without physically using the mouse.
    /// </summary>
    public static class MouseCommand
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        //LEFT BUTTON COMMANDS
        private const int LEFTDOWN = 0x0002;
        private const int LEFTUP = 0x0004;
        //RIGHT BUTTON COMMANDS
        private const int RIGHTDOWN = 0x0008;
        private const int RIGHTUP = 0x0010;
        //MOVEMENT COMMANDS
        private const int MOVE = 0x0001;
        private const int POSITION = 0x8000;
        //UNUSED
        private const int MIDDLEDOWN = 0x0020;
        private const int MIDDLEUP = 0x0040;
        
        public static void LeftDown()
        {
            mouse_event(LEFTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 20, 20);
        }
        public static void LeftUp()
        {
            mouse_event(LEFTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 20, 20);
        }
        public static void RightDown()
        {
            mouse_event(RIGHTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 20, 20);
        }
        public static void RightUp()
        {
            mouse_event(RIGHTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 20, 20);
        }
    }
    /// <summary>
    /// Hand state information is stored in this type of object. This object is used to return hand states to the application.
    /// </summary>
    public class HandState
    {
        public bool thumbUp;
        public bool fingerPoint;
        public bool active;
        public bool right;
        public bool left;

        public HandState()
        {
            thumbUp = false;
            fingerPoint = false;
            active = false;
            left = false;
            right = false;
        }
    }
    /// <summary>
    /// Imaginary screen that assists with translating hand coordinates to window coordinates.
    /// </summary>
    public class VirtualScreen
    {
        //keeps residual hand locations on form application
        private Point FP_hand = new Point(0, 0);
        //private Point FP_rightHand = new Point(0, 0);
        //virtual screen tracking values
        private double top;
        private double bottom;
        private double height;
        private double depth;
        private double left;
        private double right;
        private double width;
        //relative hand locations
        private double strideX;
        private double strideY;
        private double verticalShift;
        private double widthExpansion;

        public VirtualScreen()
        {
            top = 0;
            bottom = 0;
            height = 0;
            depth = 0;
            left = 0;
            right = 0;
            width = 0;
           
            strideX = 0;
            strideY = 0;
            verticalShift = .3;
            widthExpansion = .9;

        }
        /// <summary>
        /// Sets location and dimensions of virtual screen based on head and hip locations.
        /// </summary>
        /// <param name="headIN"></param>
        /// <param name="cntrHipIN"></param>
        public void SetVirtualScreen(DepthImagePoint topIN,DepthImagePoint bottomIN)
        {
            // use cntrShoulder for topIN
            // use cntrHip for bottomIN
            // may need to offset so hand position is never directly in front of cntrShoulder as this confuses kinect
            top = topIN.Y; // top of virtual screen
            bottom = bottomIN.Y; // bottom of virtual screen
            height = bottom - top; // virtual screen height
            //adjust location values to reduce point interference from cntrShoulder point
            double adjustment = height * verticalShift;
            top = top + adjustment; // this lowers the screen so that users hands will never overlap center shoulder point
            bottom = bottom + adjustment; // this lowers the screen so that users hands will never overlap center shoulder point
            double horizontalBound = height * (2*widthExpansion);
            // This is the virtual screen depth. It is not currently used but it will come in handy if one chooses to use 
            // depth selection instead of, or with, gesture recognition.
            depth = topIN.Depth - (height * .9);

            left = topIN.X - (height * widthExpansion); //  the top point is centered on the body so the left is a function of height
            if(left < 0) // this keeps the virtual screen in the viewport even if the user moves out of range
            {
                left = 0;
            }
            else if (left > (640 - (horizontalBound)))// this keeps the virtual screen in the viewport even if the user moves out of range
            {
                left = (640 - (horizontalBound));
            }
            right = left + (horizontalBound); // sets the right bound as a function of left and height
            width = right - left; // sets width
            //Console.WriteLine(top + " " + bottom + " " + left + " " + right);
        }
        /// <summary>
        /// Generates box indicating virtual screen position relative to depth bitmap coordinates.
        /// Useful for dev and ts.
        /// </summary>
        /// <returns></returns>
        public Rectangle GetScreenBox()
        {
            return new Rectangle((int)left,(int)top,(int)width,(int)height);
        }
        /// <summary>
        /// Converts DepthImagePoint coordinates to form coordinates.  It is important to run forms in full screen mode for
        /// this to be most effective.
        /// </summary>
        /// <param name="formLeft"></param>
        /// <param name="formTop"></param>
        /// <param name="formWidth"></param>
        /// <param name="formHeight"></param>
        /// <param name="hand"></param>
        /// <returns></returns>
        public Point GetHandFormPosition(int formLeft, int formTop, int formWidth, int formHeight, DepthImagePoint hand)
        {
            if (height > 0)
            {
                strideX = (hand.X - left) / width;
                strideY = (hand.Y - top) / height;
                FP_hand.X = (int)(formLeft + (((hand.X - left) / width) * formWidth));
                FP_hand.Y = (int)(formTop + ((hand.Y - top) / height) * formHeight);
            }
            
            return FP_hand;
        }
        /// <summary>
        /// Shifts the vertual screen up (negative doubles) or down (positive doubles).  Recommended value: 0.3.
        /// </summary>
        public void SetVerticalShift(double verticalShiftIN)
        {
            verticalShift = verticalShiftIN;
        }
        /// <summary>
        /// Expands the virtual screen width. Recommended value: 0.9.  This expands the screen width by 0.9 of the height on either
        /// side, making the width a total of 1.8 * the height.
        /// </summary>
        /// <param name="widthExpansionIN"></param>
        public void SetWidthExpansion(double widthExpansionIN)
        {
            widthExpansion = widthExpansionIN;
        }
    }
    /// <summary>
    /// Helper class reduces pointer jumping randomly.
    /// </summary>
    public class JitterReducer
    {
        // set up the hand point arrays for tracking N number of previous locations.
        int[] leftHandX = new int[20];
        int[] leftHandY = new int[20];
        int[] leftHandZ = new int[20];

        int[] rightHandX = new int[20];
        int[] rightHandY = new int[20];
        int[] rightHandZ = new int[20];

        int index = 0;
        //current locations
        DepthImagePoint DP_leftHand = new DepthImagePoint();
        DepthImagePoint DP_rightHand = new DepthImagePoint();
        //previous locations
        DepthImagePoint DP_leftHandPrevious = new DepthImagePoint();
        DepthImagePoint DP_rightHandPrevious = new DepthImagePoint();
        //Average Points
        DepthImagePoint LeftHandAvg = new DepthImagePoint();
        DepthImagePoint RightHandAvg = new DepthImagePoint();
        //Range for jitter reduction, if difference is greater than these values then new points are not updated
        int xBound = 20;
        int yBound = 20;
        int zBound = 10;
        //threading
        Thread JRThread;
        int threadSleep;
        public Boolean run = true;

        public JitterReducer()
        {
            for (int i = 0; i < 20; i++)
            {
                leftHandX[i] = 1;
                leftHandY[i] = 1;
                leftHandZ[i] = 1;
                rightHandX[i] = 1;
                rightHandY[i] = 1;
                rightHandY[i] = 1;
            }
            //Console.WriteLine(leftHandX.ToString());
        }
        public void Start()
        {
            JRThread = new Thread(new ThreadStart(SetJitterReducedPoints2));
        }
        public void STOP()
        {
            JRThread.Abort();
        }
        /// <summary>
        /// Gets the average point xyz from the applicable xzy arrays.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GetLeftHandAverage()
        {
            DP_leftHand.X = (int)leftHandX.Average();
            DP_leftHand.Y = (int)leftHandY.Average();
            DP_leftHand.Depth = (int)leftHandZ.Average();
            return DP_leftHand;
        }
        /// <summary>
        /// Gets the average point xyz from the applicable xzy arrays.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GetRightHandAverage()
        {
            DP_rightHand.X = (int)rightHandX.Average();
            DP_rightHand.Y = (int)rightHandY.Average();
            DP_rightHand.Depth = (int)rightHandZ.Average();
            return DP_rightHand;
        }
        /// <summary>
        /// Stores current DepthImagePoint coordinates in appropriate arrays.
        /// </summary>
        /// <param name="leftIN"></param>
        /// <param name="rightIN"></param>
        public void RecordHandDepthImagePoints(DepthImagePoint leftIN, DepthImagePoint rightIN)
        {
            if (index < 20)
            {
                leftHandX[index] = leftIN.X;
                leftHandY[index] = leftIN.Y;
                leftHandZ[index] = leftIN.Depth;
                rightHandX[index] = rightIN.X;
                rightHandY[index] = rightIN.Y;
                rightHandZ[index] = rightIN.Depth;
            }
            else
            {
                index = 0;
                leftHandX[index] = leftIN.X;
                leftHandY[index] = leftIN.Y;
                leftHandZ[index] = leftIN.Depth;
                rightHandX[index] = rightIN.X;
                rightHandY[index] = rightIN.Y;
                rightHandZ[index] = rightIN.Depth;
            }
            //for (int ar = 0; ar < 10; ar++)
            //{
            //    Console.WriteLine(leftHandX[ar].ToString() + index);
            //}
            index++;
        }
        /// <summary>
        /// Deprecated.
        /// </summary>
        /// <param name="leftIN"></param>
        /// <param name="rightIN"></param>
        /// <param name="xBound"></param>
        /// <param name="yBound"></param>
        public void SetJitterReducedPoints(DepthImagePoint leftIN,DepthImagePoint rightIN,int xBound,int yBound)
        {
            //left
            if (Math.Abs(leftIN.X - DP_leftHandPrevious.X) < xBound)
            {
                DP_leftHand.X = leftIN.X;
            }
            DP_leftHandPrevious.X = leftIN.X;
            if (Math.Abs(leftIN.Y - DP_leftHandPrevious.Y) < xBound)
            {
                DP_leftHand.Y = leftIN.Y;
            }
            DP_leftHandPrevious.Y = leftIN.Y;
            if (Math.Abs(leftIN.Depth - DP_leftHandPrevious.Depth) < zBound)
            {
                DP_leftHand.Depth = leftIN.Depth;
            }
            DP_leftHandPrevious.Depth = leftIN.Depth;
            //right
            if (Math.Abs(rightIN.X - DP_rightHandPrevious.X) < xBound)
            {
                DP_rightHand.X = rightIN.X;
            }
            DP_rightHandPrevious.X = rightIN.X;
            if (Math.Abs(rightIN.Y - DP_rightHandPrevious.Y) < xBound)
            {
                DP_rightHand.Y = rightIN.Y;
            }
            DP_rightHandPrevious.Y = rightIN.Y;
            if (Math.Abs(rightIN.Depth - DP_rightHandPrevious.Depth) < zBound)
            {
                DP_rightHand.Depth = rightIN.Depth;
            }
            DP_rightHandPrevious.Depth = rightIN.Depth;
        }
        /// <summary>
        /// Takes the average of the point history and stores the in a DepthImagePoint for the application to query.
        /// Causes latency.  To reduce latency, reduce the length of the point history by reduces the array sizes.
        /// Point history size is not paramaterized and reduction/increase must be done manually.
        /// </summary>
        public void SetJitterReducedPoints2()
        {
            while (run)
            {
                LeftHandAvg = GetLeftHandAverage();
                RightHandAvg = GetRightHandAverage();
                //left
                if (Math.Abs(LeftHandAvg.X - DP_leftHandPrevious.X) < xBound)
                {
                    DP_leftHand.X = LeftHandAvg.X;
                }
                DP_leftHandPrevious.X = LeftHandAvg.X;
                if (Math.Abs(LeftHandAvg.Y - DP_leftHandPrevious.Y) < xBound)
                {
                    DP_leftHand.Y = LeftHandAvg.Y;
                }
                DP_leftHandPrevious.Y = LeftHandAvg.Y;
                //right
                if (Math.Abs(RightHandAvg.X - DP_rightHandPrevious.X) < xBound)
                {
                    DP_rightHand.X = RightHandAvg.X;
                }
                DP_rightHandPrevious.X = RightHandAvg.X;
                if (Math.Abs(RightHandAvg.Y - DP_rightHandPrevious.Y) < xBound)
                {
                    DP_rightHand.Y = RightHandAvg.Y;
                }
                DP_rightHandPrevious.Y = RightHandAvg.Y;
                Thread.Sleep(threadSleep);
            }
            
        }
        /// <summary>
        /// Returns the current left hand DepthImagePoint to the application.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GetLeftHandDIP()
        {
            return DP_leftHand;
        }
        /// <summary>
        /// Returns the current right hand DepthImagePoint to the application.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GetRightHandDIP()
        {
            return DP_rightHand;
        }
        public void SET_THREADSLEEP(int sleep)
        {
            threadSleep = sleep;
        }
    }
    public class GestureEvaluater
    {
        //private byte[] colorPixels;// unused, but color data should be taken into account in the future
        public DepthImagePoint p = new DepthImagePoint();
        public Bitmap nullBitmap = new Bitmap(Image.FromFile("unavailable.jpg"));// returned if no image data available in various scenarios
        public short state_leftHand = -1;
        public short state_rightHand = -1;
        public Bitmap handDataRight;// bitmap taken from main depth data but centered around hand location
        public Rectangle handBoxRight;// used to show hand tracking box for troubleshooting and development
        public Bitmap handDataLeft;
        public Rectangle handBoxLeft;
        public Bitmap depthData;// for processing gestures
        
        public DepthImagePixel[] depthPixels;// keeps the xyz of each pixel from the depth frame
        public DepthImagePoint DP_rightHand;// this is smoothed in processing
        public DepthImagePoint DP_leftHand;// this is smoothed in processing
        public DepthImagePoint DP_rightHandRaw;// this is not smoothed, taking the true location for evaluating gesture
        public DepthImagePoint DP_leftHandRaw;// this is not smoothed, taking the true location for evaluating gesture
        //handstate classes used to store and return handstates for and to the application
        public HandState HS_left = new HandState();
        public HandState HS_right = new HandState();
        //deprecated but do not remove
        private int OFFSET_X;
        private int OFFSET_Y;
        public bool hasFrame = false;
        //threading
        public Boolean run = true;
        int threadSleep = 100;
        //gesture pools for ignoring erroneous gestures
        public int[] gesturePoolRight = new int[5];
        public int[] gesturePoolLeft = new int[5];
        int poolSize = 5;
        int gesturePoolIndex = 0;

        public GestureEvaluater()
        {
            depthData = new Bitmap(640, 480);
            
            //set intitial hand locations:
            DP_leftHand.X = -1;
            DP_leftHand.Y = -1;
            DP_leftHand.Depth = -1;

            DP_rightHand.X = -1;
            DP_rightHand.Y = -1;
            DP_rightHand.Depth = -1;

            //identify which hand state belongs to which hand:
            HS_left.left = true;
            HS_right.right = true;

            //initialize gesture pools for future use
            InitializeGesturePools();
        }
        private void InitializeGesturePools()
        {
            for (int i = 0; i < poolSize; i++)
            {
                gesturePoolLeft[i] = 0;
                gesturePoolRight[i] = 0;
            }
        }
        public bool GetLeftHandPoint()
        {
            double average = gesturePoolLeft.Average();
            if (average > 0.5) { return true; }
            return false;
        }
        public bool GetRightHandPoint()
        {
            double average = gesturePoolRight.Average();
            if (average > 0.5) { return true; }
            return false;
        }
        public void EvaluateGestures()// for threading
        {
            while (run)
            {
                if (hasFrame)
                {
                    SetHandBitmaps();
                    GetBothHandStates();
                    hasFrame = false;
                }
                Thread.Sleep(threadSleep);
            }
        }

        public void CaptureDepthFrameBitmap(Bitmap bmpIN)
        {
            if (!hasFrame)
            {
                depthData = bmpIN;
                hasFrame = true;
            }
        }
        /// <summary>
        /// Extracts the portion of the source bitmap defined by a rectangle.
        /// </summary>
        /// <param name="srcBitmap"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        static private Bitmap ExtractBitmap(Bitmap srcBitmap, Rectangle section)
        {
            /*CITATIONS*/
            //http://msdn.microsoft.com/en-us/library/aa457087.aspx
            //http://stackoverflow.com/questions/1563038/fast-work-with-bitmaps-in-c-sharp

            // Create the new bitmap and associated graphics object
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Draw the specified section of the source bitmap to the new bitmap
            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);

            // Clean up
            g.Dispose();

            // Return the bitmap
            return bmp;
        }
        /// <summary>
        /// Uses the tracking box to extract the hand bitmap from the overall depth image.
        /// </summary>
        double depth_shb; int extractionBoxSize_shb; int extractionBoxHalfSize_shb; int offsetX; int offsetY;
        private void SetHandBitmap(ref Bitmap handDataBitmapIN, DepthImagePoint dipIN, ref Rectangle handBoxIN)
        {
            extractionBoxHalfSize_shb = handBoxIN.Width / 2;// get current image extraction box size..width == height
            if (PointSafe(dipIN, extractionBoxHalfSize_shb))// check if hand point is safely within kinect view port so no sampling occurs off depth bitmap
            {
                depth_shb = (double)getPixelDepth(dipIN.X, dipIN.Y);// get the depth of the hand.. should be from 1 to 4000
                if (depth_shb > 0)// 1 to 4000, but near values not available so probably 1000+/- to 4000
                {
                    extractionBoxSize_shb = (int)(100000 / depth_shb);// size will range theoretically from 100 to 25
                }
                else
                    extractionBoxSize_shb = 100;

                extractionBoxHalfSize_shb = extractionBoxSize_shb / 2;// half size likely ranges from 50 to 12 

                offsetX = OFFSET_X - extractionBoxHalfSize_shb;// set offsets in order to center the handbox around the hand
                offsetY = OFFSET_Y - extractionBoxHalfSize_shb;

                if (PointSafe(dipIN, extractionBoxHalfSize_shb))// check to see if new extraction box size is within kinect viewport
                {
                    handBoxIN.X = dipIN.X - extractionBoxHalfSize_shb;
                    handBoxIN.Y = dipIN.Y - extractionBoxHalfSize_shb;
                    handBoxIN.Width = extractionBoxSize_shb;
                    handBoxIN.Height = extractionBoxSize_shb;
                }
                else
                {
                    handBoxIN.X = 0;
                    handBoxIN.Y = 0;
                    handBoxIN.Width = 10;
                    handBoxIN.Height = 10;
                }

                // check hand config before mapping handbox to screen coordinates
                // if you check after mapping the you will not be capture hand image
                // so here we use the handbox which identifies the bitmap location of
                // the hand to quickly assess the hand configuration
                GetClosestPoints(handBoxIN);
                setHandStates();

                try
                {
                    if (handDataBitmapIN != null) { handDataBitmapIN.Dispose(); }
                    handDataBitmapIN = ExtractBitmap(depthData, handBoxIN);// This copies the hand portion of the kinect viewport into a bitmap for gesture analysis.                
                }
                catch (Exception E)
                {
                    Console.WriteLine("Gesture Evaluater Exception::SetHandBitmap::" + E.Message);
                }

                //map the handBoxIN to the display to show tracking
                handBoxIN.X = dipIN.X + offsetX;
                handBoxIN.Y = dipIN.Y + offsetY;
                handBoxIN.Width = extractionBoxSize_shb;
                handBoxIN.Height = extractionBoxSize_shb;
            }
        }
        int location_gpd;// Setup variables outside of function to save runtime processing
        private short getPixelDepth(int x, int y)
        {
            location_gpd = x + (y * 640);
            /*
            depthPixel length = 307200 which is 640*480
            therefore location mapping is row*640 + col
            mapping to x and y parameters this becomes (y*640) + x
            */
            if ((location_gpd < 307200) && (location_gpd > -1))
            {
                return depthPixels[location_gpd].Depth;
            }
            else return -1;// Since depth values are never less than 0, -1 indicates the desired pixel was out of bounds
        }

        private void GetClosestPoints(Rectangle box)//curently not being used but may use for pointing
        {
            int closest = 4000; // Max Depth: http://msdn.microsoft.com/en-us/library/hh438998.aspx
            int temp = 4000;
            int x = 0;
            int y = 0;
            for (int r = box.Y; r < (box.Y + box.Height); r = r + 5)
            {
                for (int c = box.X; c < (box.X + box.Width); c = c + 5)
                {
                    temp = getPixelDepth(c, r);
                    if ((temp > 0) && (temp < closest))
                    {
                        closest = temp;
                        x = c;
                        y = r;
                    }
                }
            }
            p.Depth = closest;
            p.X = x;
            p.Y = y;
        }
        /// <summary>
        /// Tracks if a finger is protruding from the hand mass.  Useful for determining if hand is pointing toward kinect.
        /// </summary>
        int accumulatedDepth_agmdd; int sampleCount_gamdd; int minDepth_gamdd; int avgDepth_gamdd;
        private int GetAverageMinDepthDifference(Rectangle extractionBox, int zRef)
        {
            accumulatedDepth_agmdd = 1;
            sampleCount_gamdd = 1;
            minDepth_gamdd = 4000;
            avgDepth_gamdd = 0;

            for (int row = extractionBox.Y; row < (extractionBox.Y + extractionBox.Height); row++)
            {
                for (int col = extractionBox.X; col < (extractionBox.X + extractionBox.Width); col++)
                {
                    int temp = (int)getPixelDepth(col, row);
                    if ((temp < zRef) && (temp > 0))
                    {
                        if (temp < minDepth_gamdd)
                        {
                            minDepth_gamdd = temp;
                        }
                        accumulatedDepth_agmdd = accumulatedDepth_agmdd + temp;
                        sampleCount_gamdd++;
                    }

                }
            }
            avgDepth_gamdd = (accumulatedDepth_agmdd / sampleCount_gamdd);
            //Console.WriteLine(avg - minDepth);

            return (avgDepth_gamdd - minDepth_gamdd);
        }
        /// <summary>
        /// Counts the near pixels with a tracking box.  Useful for determining if a finger is pointing up.
        /// </summary>
        int temp_gdc; int count_gdc; int row_gdc; int col_gdc; int diff;// Sets up variable outside of function to increase performance
        private int GetDepthCount(Rectangle r, int zRef, int range, int lines)
        {
            temp_gdc = 0;
            count_gdc = 0;
            for (row_gdc = r.Y; row_gdc < (r.Y + lines); row_gdc++)
            {
                for (col_gdc = r.X; col_gdc < (r.X + r.Width); col_gdc++)
                {
                    temp_gdc = getPixelDepth(col_gdc, row_gdc);
                    diff = zRef - temp_gdc;
                    if ((Math.Abs(diff) <= range) && (temp_gdc > 0))
                    {
                        count_gdc++;
                    }
                }
            }

            return (count_gdc / lines);
        }
        int tcount_gdc; int bcount_gdc; int lcount_gdc; int rcount_gdc;
        private int GetDepthCount2(Rectangle r, int zRef, int range, int lines)
        {
            temp_gdc = 0;
            tcount_gdc = 0;
            bcount_gdc = 0;
            lcount_gdc = 0;
            rcount_gdc = 0;
            //top
            for (row_gdc = r.Y; row_gdc < (r.Y + lines); row_gdc++)
            {
                for (col_gdc = r.X; col_gdc < (r.X + r.Width); col_gdc++)
                {
                    temp_gdc = getPixelDepth(col_gdc, row_gdc);
                    diff = zRef - temp_gdc;
                    if ((Math.Abs(diff) <= range) && (temp_gdc > 0))
                    {
                        tcount_gdc++;
                    }
                }
            }
            //bottom
            for (row_gdc = (r.Y+r.Height); row_gdc > (r.Y+r.Height)-lines; row_gdc--)
            {
                for (col_gdc = r.X; col_gdc < (r.X + r.Width); col_gdc++)
                {
                    temp_gdc = getPixelDepth(col_gdc, row_gdc);
                    diff = zRef - temp_gdc;
                    if ((Math.Abs(diff) <= range) && (temp_gdc > 0))
                    {
                        bcount_gdc++;
                    }
                }
            }
            //left
            for (col_gdc = r.X; col_gdc < (r.X + lines); col_gdc++)
            {
                for (row_gdc = r.Y; row_gdc < (r.Y + r.Height); row_gdc++)
                {
                    temp_gdc = getPixelDepth(col_gdc, row_gdc);
                    diff = zRef - temp_gdc;
                    if ((Math.Abs(diff) <= range) && (temp_gdc > 0))
                    {
                        lcount_gdc++;
                    }
                }
            }
            //right
            for (col_gdc = (r.X+r.Width); col_gdc > ((r.X+r.Width) - lines); col_gdc--)
            {
                for (row_gdc = r.Y; row_gdc < (r.Y + r.Height); row_gdc++)
                {
                    temp_gdc = getPixelDepth(col_gdc, row_gdc);
                    diff = zRef - temp_gdc;
                    if ((Math.Abs(diff) <= range) && (temp_gdc > 0))
                    {
                        rcount_gdc++;
                    }
                }
            }
            
            return (Math.Max(rcount_gdc, Math.Max(lcount_gdc, Math.Max(tcount_gdc, bcount_gdc))) / lines);
        }
        /// <summary>
        /// Stores the hand state for the left hand into the class global HS_left variable for return to application.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="dip"></param>
        private void GetLeftHandState(Rectangle r, DepthImagePoint dip)
        {
            if (GetAverageMinDepthDifference(r, dip.Depth) > 40)
            {
                HS_left.fingerPoint = true;
                gesturePoolLeft[gesturePoolIndex] = 1;
            }
            else
            {
                HS_left.fingerPoint = false;
                gesturePoolLeft[gesturePoolIndex] = 0;
            }

            if (GetDepthCount(r, dip.Depth, 60, 4) > 0) HS_left.thumbUp = true;
            else HS_left.thumbUp = false;

            //gesturePoolLeft[gesturePoolIndex].fingerPoint = HS_left.fingerPoint;

        }
        /// <summary>
        /// Stores the hand state for the right hand into the class global HS_right variable for return to application.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="dip"></param>
        private void GetRightHandState(Rectangle r, DepthImagePoint dip)
        {
            //if (GetAverageMinDepthDifference(r, dip.Depth) > 40) HS_right.fingerPoint = true;
            //else HS_right.fingerPoint = false;

            //if (GetDepthCount(r, dip.Depth, 60, 4) > 0) HS_right.thumbUp = true;
            //else HS_right.thumbUp = false;

            if (GetAverageMinDepthDifference(r, dip.Depth) > 40)
            {
                HS_right.fingerPoint = true;
                gesturePoolRight[gesturePoolIndex] = 1;
            }
            else
            {
                HS_right.fingerPoint = false;
                gesturePoolRight[gesturePoolIndex] = 0;
            }

            if (GetDepthCount(r, dip.Depth, 60, 4) > 0)
            {
                HS_right.thumbUp = true;
                //gesturePoolRight[gesturePoolIndex] = 0;
            }
            else
            {
                HS_right.thumbUp = false;
                //gesturePoolRight[gesturePoolIndex] = 0;
            }
            
        }
        /// <summary>
        /// Get the states for both hands.  This does not return states but stores them in memory.
        /// </summary>
        public void GetBothHandStates()
        {
            if (gesturePoolIndex == poolSize) { gesturePoolIndex = 0; }//replace least recently used hand states in the pool with the next hand states
            GetLeftHandState(handBoxLeft, DP_leftHand);//get left hand gesture
            GetRightHandState(handBoxRight, DP_rightHand);//get right hand gesture
            gesturePoolIndex++;//go to next place in gesture pool
        }
        public string GetGesturePools()
        {
            string gp = "";
            for (int i = 0; i < poolSize; i++)
            {
                gp = gp + gesturePoolLeft[i] + "\n";
            }
            return gp;
        }
        /// <summary>
        /// Depricated.
        /// </summary>
        int temp_ghs; int count_ghs;// Sets up variable outside of function to increase performance
        private short getHandState(Rectangle r, DepthImagePoint dip)
        {



            if (r != null)
            {
                //depthReference_ghs = getPixelDepth(dip.X, dip.Y);
                //Console.WriteLine("Get Depth: " + depthReference_ghs+" DEPTH: "+dip.Depth);
                temp_ghs = 0;
                count_ghs = 0;
                count_ghs = GetDepthCount(r, dip.Depth, 50, 5);
                if (count_ghs < 1)
                {
                    return 0;
                }
                else if (count_ghs < 14)
                {
                    return 1;
                }
                else if (count_ghs < 22)
                {
                    return 3;
                }
                else if (count_ghs >= 22)
                {
                    return 3;
                }
            }
            return -1;
        }
        /// <summary>
        /// Depricated.
        /// </summary>
        private void setHandStates()
        {
            state_leftHand = getHandState(handBoxLeft, DP_leftHand);// sets the global left hand state variable
            state_rightHand = getHandState(handBoxRight, DP_rightHand);// sets the global right hand state variable
        }

        //#################
        //MANAGING BITMAPS#
        //#################


        /// <summary>
        /// Checks if the hand is in a trackable location (within kinect viewing range).
        /// </summary>
        /// <param name="handIN"></param>
        /// <param name="extractionBoxHalfSizeIN"></param>
        /// <returns></returns>
        private bool PointSafe(DepthImagePoint handIN, int extractionBoxHalfSizeIN)
        {
            if ((handIN.Y > extractionBoxHalfSizeIN) && (handIN.Y < (480 - extractionBoxHalfSizeIN)))//check if hand is within kinect view port height
            {
                if ((handIN.X > extractionBoxHalfSizeIN) && (handIN.X < (640 - extractionBoxHalfSizeIN)))//check if hand is kinect view port width
                {
                    return true;
                }
            }
            return false;
        }


        public void SetHandBitmaps()
        {
            SetHandBitmap(ref handDataLeft, DP_leftHandRaw, ref handBoxLeft);
            SetHandBitmap(ref handDataRight, DP_rightHandRaw, ref handBoxRight);
        }
        public void SET_THREADSLEEP(int sleep)
        {
            threadSleep = sleep;
        }
    }
    /// <summary>
    /// Gather Kinect frame data to interpret gestures and return those gestures to the application.
    /// </summary>
    public class KinectManager
    {
        public Bitmap nullBitmap = new Bitmap(Image.FromFile("unavailable.jpg"));// returned if no image data available in various scenarios
        public bool connected = false;
        private KinectSensor kinect = null;
        private Skeleton[] skeletonData;
        private Skeleton USER;
        //images for marking joint locations on screen
        private Image image_joint = Image.FromFile("joint.png");
        private Image image_bkground = Image.FromFile("bkground.png");
        private Image image_face = Image.FromFile("head1.png");
        private Image image_joint_transparent = Image.FromFile("jointTransparent.png");
        private Image image_digit = Image.FromFile("digit.png");
        private Image image_unavailable = Image.FromFile("unavailable.jpg");        
        private Bitmap depthData;// converts depth data to viewable bitmap
        private CoordinateMapper mapper;// converts from skeleton points to depth points

        public Bitmap depthImageBitmap;// for painting to screen
        //depth information for various joints that we track
        
        private DepthImagePoint DP_rightWrist;
        private DepthImagePoint DP_rightHand;
        private DepthImagePoint DP_leftWrist;
        private DepthImagePoint DP_leftHand;
        private DepthImagePoint DP_cntrShoulder;
        private DepthImagePoint DP_head;
        private DepthImagePoint DP_cntrHip;
        
        
        //virtual screen used for kinect to form hand point translation
        public VirtualScreen VS = new VirtualScreen();
        //count for the number of frames
        private int frameCount = 0; 
        int updateFrame = 3;//the frame count used for updating i.e. every fifth frame update
        public JitterReducer JR = new JitterReducer();
        private DepthImagePoint DP_Pointer = new DepthImagePoint();
        private Thread jitterControl;
        //Gesture Evaluation Objects
        public GestureEvaluater GE = new GestureEvaluater();
        private Thread gestureEval;

        //detection range variables
        double rangeNear = 0;
        double rangeFar = 0;

        //thread sleep
        int threadSleep = 100;

        public KinectManager()
        {
            //setup the depth bitmap
            //if I don't setup the bitmap initially it seems to fail
            depthImageBitmap = new Bitmap(640, 480);

            jitterControl = new Thread(new ThreadStart(JR.SetJitterReducedPoints2));
            
            gestureEval = new Thread(new ThreadStart(GE.EvaluateGestures));
            
            VS.SetVerticalShift(0.3);
            VS.SetWidthExpansion(0.9);
        }

        /// #########################
        /// STARTING/STOPPING KINECT#
        /// #########################
        /// 
        public void START()
        {
            this.kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor
            if (this.kinect != null)
            {
                connected = true;
                this.mapper = new CoordinateMapper(kinect);// Establish the corrdinate mapper 
                this.kinect.SkeletonStream.Enable();// Enable skeletal tracking
                this.kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);// Enable the depth stream

                skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength];// Setup the skeleton data array
                kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(AllFramesReady);// When all frames ready execute the function
                kinect.Start(); // Start Kinect sensor

                GE.depthPixels = new DepthImagePixel[this.kinect.DepthStream.FramePixelDataLength];// Setup the pixel data array to get depth data
                //this.colorPixels = new byte[this.kinect.DepthStream.FramePixelDataLength * sizeof(int)];// 

                jitterControl.Start();
                //JR.Start();
                gestureEval.Start();
                Console.WriteLine("Kinect Manager Message::Kinect Status::Started");// Started message
            }
        }
        public void STOP()
        {
            GE.run = false;
            JR.run = false;
            kinect.Stop();
            Console.WriteLine("Kinect Manager Message::Kinect Status::Stopped");
        }
        
        /// ##########################
        /// GETTING KINECT FRAME DATA#
        /// ##########################
        
        

        /// Executed whenever a skeleton frame is ready.
        private void SkeletonFrameReady(AllFramesReadyEventArgs e)
        {
                using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())// Entry into skeleton frame
                {
                    try
                    {
                        if (skeletonFrame != null && this.skeletonData != null)// Is frame ready?
                        {
                            skeletonFrame.CopySkeletonDataTo(this.skeletonData);// Get the skeleton data
                            USER = GET_SKELETON_NEAR();// Get the target skeleton
                            setDepthPointsFromSkeletonPoints(USER);// converts skeleton hand data to depth points for gesture analysis
                        }  
                    }
                    catch (Exception E)
                    {
                        Console.WriteLine("Kinect Manager Message::Skeleton Frame Exception::"+E.Message);
                    }
                }
        }
        /// <summary>
        /// Executed whenever a depth frame is ready.
        /// </summary>
        /// <param name="e"></param>
        private void DepthFrameReady(AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                try
                {
                    if (depthFrame != null)
                    {
                        Bitmap old = depthImageBitmap;
                        depthFrame.CopyDepthImagePixelDataTo(GE.depthPixels);//get the actual depth pixels
                        //Bitmap bmp;// = new Bitmap(640, 480);
                        //bmp = DepthImageFrameToBitmap(depthFrame);
                        //depthImageBitmap = bmp;//save the depth data into bitmap form for reference
                        //if (depthImageBitmap != null) { depthImageBitmap.Dispose(); }
                        depthImageBitmap = DepthImageFrameToBitmap(depthFrame);
                        if (old != null) { old.Dispose(); }
                        //GE.CaptureDepthFrameBitmap(bmp);
                        GE.CaptureDepthFrameBitmap(depthImageBitmap);
                    }
                }
                catch (Exception E)
                {
                    Console.WriteLine("Kinect Manager Message::Depth Frame Exception::"+E.Message);
                }
            }
        }
        /// <summary>
        /// This is executed only when all frames have been retrieved from stream and are ready for processing.
        /// Gesture calculation modules and any module dependent on the frame data should be placed here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            SkeletonFrameReady(e);// Get the Skeleton Data
            DepthFrameReady(e);// Get the Depth Data

            //GE.ready = true;// notifies gestureEval thread that it can evaluate a gesture
            //GE.SetHandBitmaps();//focus on hands in order to evaluate hand gestures
            //GE.GetBothHandStates();
            
            //Console.WriteLine("Frame Count: " + frameCount);
        }

        /// <summary>
        /// Converts depth image frame to bitmap so it can be rendered to screen for development and troubleshooting.
        /// Tutorials were used for development of this function and one citation is listed.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public Bitmap DepthImageFrameToBitmap(DepthImageFrame frame)
        {
            /*CITATION*/
            //http://msdn.microsoft.com/en-us/library/jj131029.aspx
            //
            Bitmap bmp = new Bitmap(frame.Width, frame.Height, PixelFormat.Format16bppRgb565);//get the data to make image bitmap
            try
            {
                    
                var depthPixelData = new short[frame.PixelDataLength];
                frame.CopyPixelDataTo(depthPixelData);//Copy the depth frame data onto the bitmap 
                BitmapData bmapdata = bmp.LockBits(new Rectangle(0, 0, frame.Width, frame.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                IntPtr ptr = bmapdata.Scan0;//go to the beginning of the data
                Marshal.Copy(depthPixelData, 0, ptr, frame.Width * frame.Height);
                bmp.UnlockBits(bmapdata);    
            }
            catch (Exception E)
            {
                Console.WriteLine("Kinect Manager Exception::DepthImageFrameToBitmap::" + E.Message);
            }
            return bmp;
        }
        
        /// ####################################
        /// GETTING X, Y, OR Z VALUES OR POINTS#
        /// ####################################

        
        
        /// <summary>
        /// Maps from skeleton point to depth point.  Allows us to determine where a joint is located in the depth image.
        /// </summary>
        /// <param name="s"></param>
        private void setDepthPointsFromSkeletonPoints(Skeleton s)
        {
            SetJitterReducedHandDepthPoints(s);
            GE.DP_leftHandRaw = mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            DP_leftWrist = mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.WristLeft].Position, DepthImageFormat.Resolution640x480Fps30);
            GE.DP_rightHandRaw = mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.HandRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DP_rightWrist = mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.WristRight].Position, DepthImageFormat.Resolution640x480Fps30);
            DP_cntrShoulder = mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.ShoulderCenter].Position, DepthImageFormat.Resolution640x480Fps30);
            DP_head = mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.Head].Position, DepthImageFormat.Resolution640x480Fps30);
            DP_cntrHip = mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.HipCenter].Position, DepthImageFormat.Resolution640x480Fps30);
            VS.SetVirtualScreen(DP_cntrShoulder, DP_cntrHip);
            
        }
        private void SetJitterReducedHandDepthPoints(Skeleton s)
        {
            JR.RecordHandDepthImagePoints(mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.HandLeft].Position, DepthImageFormat.Resolution640x480Fps30),mapper.MapSkeletonPointToDepthPoint(s.Joints[JointType.HandRight].Position, DepthImageFormat.Resolution640x480Fps30));
            //JR.SetJitterReducedPoints(JR.GetLeftHandAverage(), JR.GetRightHandAverage(), 20, 20);
            //JR.SetJitterReducedPoints2();//on separate thread now
            
            GE.DP_leftHand = JR.GetLeftHandDIP();
            GE.DP_rightHand = JR.GetRightHandDIP();
            //Console.WriteLine("Joint Positions:");
            //Console.WriteLine(s.Joints[JointType.HandLeft].Position.X.ToString());
            //Console.WriteLine(s.Joints[JointType.HandRight].Position.X.ToString());
            //Console.WriteLine("GE Hand Positions:");
            //Console.WriteLine(GE.DP_leftHand.X + " " + GE.DP_leftHand.Y);
            //Console.WriteLine(GE.DP_rightHand.X + " " + GE.DP_rightHand.Y);

            //GE.DP_leftHand = DP_leftHand;
            //GE.DP_rightHand = DP_rightHand;
        }
        
        //####################
        //EVALUATING GESTURES#
        //####################
        
        

        //########################################
        //METHODS FOR GETTING KINECT MANAGER DATA#
        //########################################  

        /// <summary>
        /// Returns 1 of the 6 skeletons tracked by kinect sensor.
        /// </summary>
        public Skeleton GET_SKELETON(int index)
        {
            return skeletonData[index];
        }
        /// <summary>
        /// Gets the nearest skeleton for tracking.
        /// </summary>
        int gsk_index; double closest; double gsk_temp; int gks_i;
        public Skeleton GET_SKELETON_NEAR()
        {
            gsk_index = 0;
            closest = 9999;
            for (gks_i = 0; gks_i < 6; gks_i++)
            {
                if ((skeletonData[gks_i] != null) && (skeletonData[gks_i].TrackingState == SkeletonTrackingState.Tracked))
                {
                    gsk_temp = skeletonData[gks_i].Position.Z;
                    if (gsk_temp < closest)
                    {
                        closest = gsk_temp;
                        gsk_index = gks_i;
                    }
                }
            }
            return skeletonData[gsk_index];
        }
        /// <summary>
        /// Returns the bitmap of the left hand within the tracking box.  Useful for trouble shooting and development.
        /// </summary>
        /// <returns></returns>
        public Bitmap GET_LEFTHAND_BITMAP()
        {
            if (GE.handDataLeft == null) return nullBitmap;
            else return GE.handDataLeft;
        }
        /// <summary>
        /// Returns the bitmap of the righthand within the tracking box.
        /// </summary>
        /// <returns></returns>
        public Bitmap GET_RIGHTHAND_BITMAP()
        {
            if (GE.handDataRight == null) return nullBitmap;
            else return GE.handDataRight;
        }
        /// <summary>
        /// Returns everything within the kinect viewing range in bitmap form.
        /// </summary>
        /// <returns></returns>
        public Bitmap GET_DEPTH_BITMAP()
        {
            if (depthImageBitmap == null) return nullBitmap;
            else return depthImageBitmap;
        }
        /// <summary>
        /// Returns the tracking box that should be centered around the left hand. Useful for dev and ts.
        /// </summary>
        /// <returns></returns>
        public Rectangle GET_LEFTHAND_FRAME()
        {
            return GE.handBoxLeft;
        }
        /// <summary>
        /// Returns the right hand tracking box that should be centered around the right hand. Useful for dev and ts.
        /// </summary>
        /// <returns></returns>
        public Rectangle GET_RIGHTHAND_FRAME()
        {
            return GE.handBoxRight;
        }
        /// <summary>
        /// Returns the right hand state.  Hand state is tracked in real time and this returns the hand state at the moment
        /// the function is called.  This method and the related functions are deprecated and should not be used.
        /// </summary>
        /// <returns></returns>
        public short GET_RIGHTHAND_STATE()
        {
            return GE.state_rightHand;
        }
        /// <summary>
        /// Returns the left hand state at the moment the function is called. The function related to this method is deprecated
        /// and this method should not be used.  It remains for reference.
        /// </summary>
        /// <returns></returns>
        public short GET_LEFTHAND_STATE()
        {
            return GE.state_leftHand;
        }
        /// <summary>
        /// Returns the absolute xyz of the left hand relative to the kinect. This is not translated to screen coordinates.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GET_LEFTHAND_DIP()
        {
            return GE.DP_leftHand;
        }
        /// <summary>
        /// Returns the absolute xyz of the right hand relative to the kinect.  this is not translated to screen coordinates.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GET_RIGHTHAND_DIP()
        {
            return GE.DP_rightHand;
        }
        /// <summary>
        /// Returns current hand state.  This method is current and should be used.
        /// </summary>
        /// <returns></returns>
        public HandState GET_LEFTHAND_STATE2()
        {
            return GE.HS_left;
        }
        /// <summary>
        /// Returns current hand state.
        /// </summary>
        /// <returns></returns>
        public HandState GET_RIGHTHAND_STATE2()
        {
            return GE.HS_right;
        }
        /// <summary>
        /// Returns xyz of left wrist relative to kinect.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GET_HEAD_DIP()
        {
            return DP_head;
        }
        public DepthImagePoint GET_LEFTWRIST_DIP()
        {
            return DP_leftWrist;
        }
        /// <summary>
        /// Returns xyz of right wrist relative to kinect.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GET_RIGHTWRIST_DIP()
        {
            return DP_rightWrist;
        }
        /// <summary>
        /// Returns xyz of center shoulder relative to kinect.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GET_CNTRSHOULDER_DIP()
        {
            return DP_cntrShoulder;
        }
        /// <summary>
        /// Returns xyz of kinect center hip.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GET_CNTRHIP_DIP()
        {
            return DP_cntrHip;
        }
        /// <summary>
        /// Returns kinect xyz of nearest hand.
        /// </summary>
        /// <returns></returns>
        public DepthImagePoint GET_NEARHAND_DIP()
        {
            if (GE.DP_rightHand.Depth < GE.DP_leftHand.Depth)
            {
                return GE.DP_rightHand;
            }
            return GE.DP_leftHand;
        }
        public bool GET_NEAR_HAND_POINT()
        {
            if (GET_NEARHAND_STATE2().left)
            {
                return (GE.GetLeftHandPoint());
            }
            else if (GET_NEARHAND_STATE2().right)
            {
                return (GE.GetRightHandPoint());
            }
            return false;
        }
        /// <summary>
        /// Returns hand state of nearest hand.
        /// </summary>
        /// <returns></returns>
        public HandState GET_NEARHAND_STATE2()
        {
            if (GE.DP_rightHand.Depth < GE.DP_leftHand.Depth)
            {
                return GE.HS_right;
            }
            return GE.HS_left;
        }
        public void SET_NEARHAND_ACTIVE(bool activeIN)
        {
            GET_NEARHAND_STATE2().active = activeIN;
        }
        public Boolean IS_IN_RANGE()
        {
            double temp = GET_HEAD_DIP().Depth;
            if (temp != null)
            {
                if ((temp > rangeNear) && (temp < rangeFar))
                {
                    return true;
                }
            }
            return false;
        }
        public void SET_TRACKING_RANGE(double near, double far)
        {
            rangeNear = near;
            rangeFar = far;
        }
    }
}
