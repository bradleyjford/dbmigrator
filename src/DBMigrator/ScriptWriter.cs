using System;
using System.IO;

namespace DbMigrator
{
    /// Based on http://blogs.msdn.com/b/danhardan/archive/2007/03/30/database-change-scripts-mambo-style.aspx
    internal class ScriptWriter
    {
        private const string BeginLoopToken = "[BEGIN LOOP]";
        private const string EndLookToken = "[END LOOP]";
        private const string InsertFileNameToken = "[INSERT FILENAME]";
        private const string InsertFileToken = "[INSERT FILE]";
        private const string InsertSectionNumberToken = "[INSERT SECTION NUMBER]";

        private readonly StringWriter _writer;
        private readonly string _repeatedTemplate;
        private readonly string _template;
        private readonly int _beginLoopIndex;
        private readonly int _endLoopIndex;

        public ScriptWriter(StringWriter writer)
        {
            _writer = writer;
            _template = Scripts.Template;

            _beginLoopIndex = _template.IndexOf(BeginLoopToken, StringComparison.Ordinal);
            _endLoopIndex = _template.IndexOf(EndLookToken, StringComparison.Ordinal);

            _repeatedTemplate = _template.Substring(_beginLoopIndex + BeginLoopToken.Length,
                _endLoopIndex - _beginLoopIndex - BeginLoopToken.Length);
        }
        
        public void WriteHeader()
        {
            _writer.Write(_template.Substring(0, _beginLoopIndex));  
        }

        public void WriteFooter()
        {
            _writer.Write(_template.Substring(_endLoopIndex + EndLookToken.Length,
               _template.Length - _endLoopIndex - EndLookToken.Length));
        }

        public void WriteScript(string fileName, int batch, string script)
        {
            var content = _repeatedTemplate;

            content = content.Replace(InsertFileNameToken, fileName);
            content = content.Replace(InsertSectionNumberToken, batch.ToString());
            content = content.Replace(InsertFileToken, script);

            _writer.Write(content);
        }
    }
}
