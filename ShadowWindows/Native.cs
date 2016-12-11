using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
namespace ShadowWindows
{
    public class Native
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);
        public static void LoadProcessInControl(string _Process, Control _Control/*,IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags*/)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(_Process);
            Thread.Sleep(10000);
            try
            {
                p.WaitForInputIdle();
                Native.SetParent(p.MainWindowHandle, _Control.Handle);
               // SetWindowPos(p.MainWindowHandle, hWnd, hWndInsertAfter, X, Y, cx, cy, uFlags);
            }
            catch(Exception e)
            {
                Console.WriteLine("Caught exception {0}", e);
            }
            //Native.SetParent(p.MainWindowHandle, _Control.Handle);

        }
    }
}
