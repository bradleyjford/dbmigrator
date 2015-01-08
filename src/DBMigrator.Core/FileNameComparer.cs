using System;
using System.Collections.Generic;
using System.IO;

namespace DbMigrator.Core
{
    internal class FilenameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var fileNameX = Path.GetFileName(x);
            var fileNameY = Path.GetFileName(y);

            return String.Compare(fileNameX, fileNameY, StringComparison.OrdinalIgnoreCase);
        }
    }
}
