﻿using System;
using System.Collections.Generic;
using System.IO;

namespace DbMigrator
{
    internal class FileNameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var fileNameX = Path.GetFileName(x);
            var fileNameY = Path.GetFileName(y);

            return String.Compare(fileNameX, fileNameY, StringComparison.OrdinalIgnoreCase);
        }
    }
}
