using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyMostRecent
{
    public static class Classes
    {
        /// <summary>
        /// Formats a long byte size into readable shorthand, up to Terabytes.
        /// Size is calculated as binary e.g. multiples of 1024.
        /// </summary>
        /// <param name="size">The file size.</param>
        /// <returns>Formatted string display of the size, showing shorthand e.g. KB, MB, GB, TB.</returns>
        public static string FormatSize(long size)
        {
            double sizeRound = size;
            string result = String.Empty;
            if (size < 1024)
            {
                result = String.Format("{0:n0} bytes", sizeRound);
            }
            else if (size < 1048576)
            {
                sizeRound = Math.Round(((double)size / 1024), 1);
                result = String.Format("{0:n1} KB", sizeRound);
            }
            else if (size < 1073741824)
            {
                sizeRound = Math.Round(((double)size / 1048576), 1);
                result = String.Format("{0:n1} MB", sizeRound);
            }
            else if (size < 1099511627776)
            {
                sizeRound = Math.Round(((double)size / 1073741824), 1);
                result = String.Format("{0:n1} GB", sizeRound);
            }
            else
            {
                sizeRound = Math.Round(((double)size / 1099511627776), 1);
                result = String.Format("{0:n1} TB", sizeRound);
            }
            return result;
        }
    }
}
