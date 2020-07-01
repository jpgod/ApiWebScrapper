using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebScrapper.Helpers
{
    /// <summary>
    /// Class with utils methods
    /// </summary>
    public static class Utils
    {
        private static readonly string[] binaryTypes = { ".JPG",".PNG",".JPEG",".BMP",".GIF",".FLAC",".MP3",".WAV",".MP4",".MKV",".AVI",".OGG",".EXE",".DLL",".ZIP",".7Z",".RAR",".TAR",".3GP",".ICO",".BAK" };
        
        /// <summary>
        /// Check if url is a github repository
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsRepository(string url)
        {
            return url.StartsWith("https://github.com/");
        }

        /// <summary>
        /// Return the total lines from a file
        /// </summary>
        public static int TotalLines(string filePath, string extension)
        {
            //Check if file is binary type (like a image, video, audio, etc)
            if (binaryTypes.Contains(extension)) return 0;

            using (StreamReader r = new StreamReader(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }
    }
}
