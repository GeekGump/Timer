using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer
{
    public static class Config
    {
        public static string AppName => "WPFTimer";

        public static string AppFolder 
        {
            get
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string folderPath = System.IO.Path.Combine(appData, AppName);
                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }
                return folderPath;
            }
        }

    }
}
