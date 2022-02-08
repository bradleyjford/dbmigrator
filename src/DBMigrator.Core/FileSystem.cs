using System;
using System.Collections.Generic;
using System.IO;

namespace DBMigrator.Core
{
    public interface IFileSystem
    {
        IEnumerable<string> GetScriptFileNames(string basePath, IEnumerable<string> includeDirectories);
        Stream OpenFileReadOnly(string filename);
        Stream OpenFile(string filename);
    }

    public class FileSystem : IFileSystem
    {
        readonly string _fileSpec;

        public FileSystem(string fileSpec)
        {
            _fileSpec = fileSpec;
        }

        public IEnumerable<string> GetScriptFileNames(string basePath, IEnumerable<string> includeDirectories)
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
}
