using System;
using System.Collections.Generic;
using System.IO;

namespace DbMigrator.Core
{
    public interface IFileSystem
    {
        IEnumerable<string> GetScriptFileNames(string basePath, IEnumerable<string> includeDirectories);
        Stream OpenFileReadOnly(string filename);
        Stream OpenFile(string filename);
    }

    public class FileSystem : IFileSystem
    {
        private const string ScriptSearchPattern = "*.sql";

        public IEnumerable<string> GetScriptFileNames(string basePath, IEnumerable<string> includeDirectories)
        {
            foreach (var file in Directory.EnumerateFiles(basePath, ScriptSearchPattern))
            {
                yield return file;
            }

            foreach (var includeDirectory in includeDirectories)
            {
                foreach (var file in Directory.EnumerateFiles(includeDirectory, ScriptSearchPattern))
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
