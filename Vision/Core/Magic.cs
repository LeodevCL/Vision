using System.IO;

namespace Vision
{
    public static class Magic
    {
        public static void OpenAndSelect(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            // combine the arguments together
            // it doesn't matter if there is a space after ','
            string argument = "/select, \"" + path + "\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }
    }
}
