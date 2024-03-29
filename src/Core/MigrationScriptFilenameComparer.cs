﻿using System.IO;

namespace DbMigrator.Core;

sealed class MigrationScriptFilenameComparer : IComparer<string>
{
    public static MigrationScriptFilenameComparer Instance = new();

    MigrationScriptFilenameComparer()
    {
    }
    
    public int Compare(string? x, string? y)
    {
        var fileNameX = Path.GetFileName(x);
        var fileNameY = Path.GetFileName(y);

        return string.Compare(fileNameX, fileNameY, StringComparison.OrdinalIgnoreCase);
    }
}