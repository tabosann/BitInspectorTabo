using System;
using System.Runtime.InteropServices;

namespace ImageConverterTabo.Helpers
{
    internal partial class NativeMethod
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong);

        [Flags]
        internal enum WindowLongIndexFlags : int
        {
            GWL_HWNDPARENT = -8,
        }
    }
}
