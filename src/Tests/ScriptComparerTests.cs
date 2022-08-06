using DbMigrator.Core;

namespace DbMigrator.Tests;

public class MigrationScriptFilenameComparerTests
{
    readonly MigrationScriptFilenameComparer _comparer;

    public MigrationScriptFilenameComparerTests()
    {
        _comparer = new MigrationScriptFilenameComparer();
    }

    [Fact]
    public void Compare_ScriptNames_AreComparedAlphaNumerically()
    {
        const string x = "Z\\0001 Script 1.sql";
        const string y = "A\\2 Script 2.sql";

        Assert.True(_comparer.Compare(x, y) < 0);
    }
}