using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace FolderExplorer1
{  
    /// <summary>
    /// The structure of this class was modeled after this site and modified for use: http://www.developerfusion.com/code/4630/capture-a-screen-shot/
    /// The explanations of the external functions can be found here: http://msdn.microsoft.com/en-us/library/windows/desktop/dd183370(v=vs.85).aspx
    /// </summary>
    public class ScreenCopier
    {
        /// <summary>
        /// Returns an image manufactured from the viewable screen.
        /// </summary>
        public Image GetScreenImage()
        {
            return GetImageFromScreen(HLPR_USER32.GetDesktopWindow());
        }
        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        public Image GetImageFromScreen(IntPtr handle)
        {
            IntPtr HDC_window = HLPR_USER32.GetWindowDC(handle);//get the handle to the device context (HDC) for the window
            HLPR_USER32.Frame frame = new HLPR_USER32.Frame();//make a new rectangle to store the window dimensions
            HLPR_USER32.GetWindowRect(handle, ref frame);//get the window dimensions
            frame.width = frame.right - frame.left;//get the window dimensions
            frame.height = frame.bottom - frame.top;//get the window dimensions
            IntPtr HDC_Copy = HLPR_GDI32.CreateCompatibleDC(HDC_window);//create a device context to which we can copy the window device context
            IntPtr myBitmap = HLPR_GDI32.CreateCompatibleBitmap(HDC_window, frame.width, frame.height);//create a bitmap to which we can copy the screen image
            IntPtr myBmpPtr = HLPR_GDI32.SelectObject(HDC_Copy, myBitmap);//point to the bitmap
            HLPR_GDI32.BitBlt(HDC_Copy, 0, 0, frame.width, frame.height, HDC_window, 0, 0, HLPR_GDI32.COPY);//tranfer color data to copy, pixel by pixel
            HLPR_GDI32.SelectObject(HDC_Copy, myBmpPtr);//select the copy
            HLPR_GDI32.DeleteDC(HDC_Copy);//delete the context
            HLPR_USER32.ReleaseDC(handle, HDC_window);//release the device context so it can be used by other applications 
            Image img = Image.FromHbitmap(myBitmap);//get the image from the bitmap
            HLPR_GDI32.DeleteObject(myBitmap);//delete the bitmap
            return img;//return the image
        }
        /// <summary>
        /// This is a helper class for accessing gdi32 methods
        /// </summary>
        private class HLPR_GDI32
        {
            public const int COPY = 0x00CC0020;
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);//transfer color data pixel by pixel
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);//create bitmap that is compatible with the device of the device context
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);//creates compatible memory device context
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);//deletes the device context
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);//deletes the object
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);//select the object "into" the device context
        }

        /// <summary>
        /// This is a helper class containing for access to user32 methods
        /// </summary>
        private class HLPR_USER32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Frame//we will link window rectangles to this structure
            {
                //window "frame" dimensions:
                public int left;
                public int top;
                public int right;
                public int bottom;
                public int width;
                public int height;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();//external method we need to get the window
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);//external method we need to get the window device context
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);//external method we need to release the window device context
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Frame frame);//external method we need to get the window rectangle which we link to our frame structure
        }
    }
}
