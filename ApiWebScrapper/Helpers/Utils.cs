using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWebScrapper.Helpers
{
    /// <summary>
    /// Class with utils methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Check if url is a github repository
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsRepository(string url)
        {
            return url.StartsWith("https://github.com/");
        }
    }
}
