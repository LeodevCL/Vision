using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Vision
{
    public class WindowsFileNameComparer : IComparer<FileInfo>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern int StrCmpLogicalW(String x, String y);
        public int Compare(FileInfo x, FileInfo y)
        {
            return StrCmpLogicalW(x.Name, y.Name);
        }
    }
}
