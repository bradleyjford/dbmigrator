using System;
using System.Collections.Generic;
using System.IO;

namespace DbMigrator
{
    internal interface IFileSystem
    {
        IEnumerable<string> GetScriptFileNames(IEnumerable<string> includeEnvironments);
        Stream OpenFile(string fileName);
    }

    internal class FileSystem : IFileSystem
    {
        private readonly string _basePath;

        public FileSystem(string basePath)
        {
            _basePath = basePath;
        }

        public IEnumerable<string> GetScriptFileNames(IEnumerable<string> includeEnvironments)
        {
            foreach (var file in Directory.EnumerateFiles(_basePath, "*.sql"))
            {
                yield return file;
            }

            foreach (var includedEnvironment in includeEnvironments)
            {
                var includedPath = Path.Combine(_basePath, includedEnvironment);

                foreach (var file in Directory.EnumerateFiles(includedPath, "*.sql"))
                {
                    yield return file;
                }
            }
        }

        public Stream OpenFile(string fileName)
        {
            return File.OpenRead(fileName);
        }
    }
}
