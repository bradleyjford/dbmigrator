using System;
using System.IO;

namespace DbMigrator
{
    /// Based on http://blogs.msdn.com/b/danhardan/archive/2007/03/30/database-change-scripts-mambo-style.aspx
    internal class ScriptWriter
    {
        private readonly StreamWriter _writer;
        private readonly string _repeatedTemplate;
        private readonly string _template;
        private readonly int _beginLoopIndex;
        private readonly int _endLoopIndex;

        public ScriptWriter(StreamWriter writer, string template)
        {
            _writer = writer;
            _template = template;

            _beginLoopIndex = _template.IndexOf(TemplateToken.BeginLoop, StringComparison.Ordinal);
            _endLoopIndex = _template.IndexOf(TemplateToken.EndLoop, StringComparison.Ordinal);

            _repeatedTemplate = _template.Substring(_beginLoopIndex + TemplateToken.BeginLoop.Length,
                _endLoopIndex - _beginLoopIndex - TemplateToken.BeginLoop.Length);
        }
        
        public void WriteHeader()
        {
            _writer.Write(_template.Substring(0, _beginLoopIndex));  
        }

        public void WriteFooter()
        {
            _writer.Write(_template.Substring(_endLoopIndex + TemplateToken.EndLoop.Length,
               _template.Length - _endLoopIndex - TemplateToken.EndLoop.Length));
        }

        public void WriteScript(string fileName, int batch, string script)
        {
            var content = _repeatedTemplate;

            content = content.Replace(TemplateToken.Filename, fileName);
            content = content.Replace(TemplateToken.Batch, batch.ToString());
            content = content.Replace(TemplateToken.Script, script);

            _writer.Write(content);
        }
    }
}
