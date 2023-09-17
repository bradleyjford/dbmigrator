using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using DbMigrator.Core;

namespace DbMigrator.Tests;

public class EmbeddedResourceFileSystem : IFileSystem
{
    readonly Assembly _containingAssembly;

    public EmbeddedResourceFileSystem(Assembly containingAssembly)
    {
        _containingAssembly = containingAssembly;
    }
    public Stream OpenFileReadOnly(string filename)
    {
        return OpenFile(filename);
    }

    public Stream OpenFile(string filename)
    {
        var result = _containingAssembly.GetManifestResourceStream(filename);

        if (result is null)
        {
            throw new FileNotFoundException("The specified resource file could not be found", filename);
        }

        return result;
    }

    public ImmutableArray<string> GetMigrationScripts(string basePath, string[] includeDirectories)
    {
        var resourceNames = _containingAssembly.GetManifestResourceNames();

        var result = ImmutableArray.CreateBuilder<string>();
        
        foreach (var resourceName in resourceNames)
        {
            if (resourceName.StartsWith(basePath) || includeDirectories.Any(d => resourceName.StartsWith(d)))
            {
                result.Add(resourceName);
            }
        }

        return result.ToImmutable();
    }
}