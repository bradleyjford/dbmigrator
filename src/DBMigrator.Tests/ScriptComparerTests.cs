using System;
using DBMigrator.Core;
using Xunit;

namespace DBMigrator.Tests
{
    public class ScriptComparerTests
    {
        readonly FilenameComparer _comparer;

        public ScriptComparerTests()
        {
            _comparer = new FilenameComparer();
        }

        [Fact]
        public void Compare_ScriptNames_AreComparedAlphaNumerically()
        {
            var x = "Z\\0001 Script 1.sql";
            var y = "A\\2 Script 2.sql";

            Assert.True(_comparer.Compare(x, y) < 0);
        }
    }
}
