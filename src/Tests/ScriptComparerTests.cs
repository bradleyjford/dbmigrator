using DbMigrator.Core;

namespace DbMigrator.Tests;

public class MigrationScriptFilenameComparerTests
{
    readonly MigrationScriptFilenameComparer _comparer = MigrationScriptFilenameComparer.Instance;

    [Fact]
    public void Compare_ScriptNames_AreComparedAlphaNumerically()
    {
        const string x = "1 Script 1.sql";
        const string y = "2 Script 2.sql";

        Assert.True(_comparer.Compare(x, y) < 0);
    }
}