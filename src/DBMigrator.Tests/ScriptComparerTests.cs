using System;
using Xunit;

namespace DbMigrator.Tests
{
    public class ScriptComparerTests
    {
        private readonly FileNameComparer _comparer;

        public ScriptComparerTests()
        {
            _comparer = new FileNameComparer();
        }

        [Fact]
        public void Compare_ScriptNames_AreComparedAlphaNumerically()
        {
            var x = "Z\\0001 Script 1.sql";
            var y = "A\\2 Script 2.sql";

            Assert.Equal(-1, _comparer.Compare(x, y));
        }
    }
}
