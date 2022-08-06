using System.Collections.Immutable;
using System.IO;

namespace DbMigrator.Core;

public interface IFileSystem
{
    Stream OpenFileReadOnly(string filename);
    Stream OpenFile(string filename);
    ImmutableArray<string> GetMigrationScripts(string basePath, string[] includeDirectories);
}

public sealed class FileSystem : IFileSystem
{
    static readonly IComparer<string> MigrationFilenameComparer = new MigrationScriptFilenameComparer();

    readonly string _fileSpec;

    public FileSystem(string fileSpec)
    {
        _fileSpec = fileSpec;
    }

    public ImmutableArray<string> GetMigrationScripts(string basePath, string[] includeDirectories)
    {
        return EnumerateFiles(basePath, includeDirectories)
            .OrderBy(s => s, MigrationFilenameComparer)
            .ToImmutableArray();
    }

    IEnumerable<string> EnumerateFiles(string basePath, string[] includeDirectories)
    {
        foreach (var file in Directory.EnumerateFiles(basePath, _fileSpec))
        {
            yield return file;
        }

        foreach (var includeDirectory in includeDirectories)
        {
            foreach (var file in Directory.EnumerateFiles(includeDirectory, _fileSpec))
            {
                yield return file;
            }
        }
    }

    public Stream OpenFileReadOnly(string filename)
    {
        return File.OpenRead(filename);
    }

    public Stream OpenFile(string filename)
    {
        return File.OpenWrite(filename);
    }
}