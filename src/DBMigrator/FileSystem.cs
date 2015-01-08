using System;
using System.Collections.Generic;
using System.IO;

namespace DbMigrator
{
    internal interface IFileSystem
    {
        IEnumerable<string> GetScriptFileNames(IEnumerable<string> includeSubDirectories);
        Stream OpenFileReadOnly(string filename);
        Stream OpenFile(string filename);
    }

    internal class FileSystem : IFileSystem
    {
        private const string ScriptSearchPattern = "*.sql";

        private readonly string _basePath;

        public FileSystem(string basePath)
        {
            _basePath = basePath;
        }

        public IEnumerable<string> GetScriptFileNames(IEnumerable<string> includeSubDirectories)
        {
            foreach (var file in Directory.EnumerateFiles(_basePath, ScriptSearchPattern))
            {
                yield return file;
            }

            foreach (var includedEnvironment in includeSubDirectories)
            {
                var includedPath = Path.Combine(_basePath, includedEnvironment);

                foreach (var file in Directory.EnumerateFiles(includedPath, ScriptSearchPattern))
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
