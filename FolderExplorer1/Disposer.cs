using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderExplorer1
{
    class Disposer
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool DestroyCursor(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr handle);

        public bool DestroyCursorU(IntPtr handle)
        {
                return DestroyCursor(handle);
        }
        public bool DestroyIconU(IntPtr handle)
        {
                return DestroyIcon(handle);
        }
        public bool DeleteObjectG(IntPtr handle)
        {
            return DeleteObject(handle);
        }
    }
}
