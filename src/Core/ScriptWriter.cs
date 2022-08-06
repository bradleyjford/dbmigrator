using System.IO;

namespace DbMigrator.Core
{
    /// Based on http://blogs.msdn.com/b/danhardan/archive/2007/03/30/database-change-scripts-mambo-style.aspx
    class ScriptWriter
    {
        readonly StreamWriter _writer;
        readonly string _repeatedTemplate;
        readonly string _template;
        readonly int _beginLoopIndex;
        readonly int _endLoopIndex;

        public ScriptWriter(StreamWriter writer, string template)
        {
            _writer = writer;
            _template = template;

            _beginLoopIndex = _template.IndexOf(TemplateToken.BeginLoop, StringComparison.Ordinal);
            _endLoopIndex = _template.IndexOf(TemplateToken.EndLoop, StringComparison.Ordinal);

            _repeatedTemplate = _template.Substring(
                _beginLoopIndex + TemplateToken.BeginLoop.Length,
                _endLoopIndex - _beginLoopIndex - TemplateToken.BeginLoop.Length);
        }
        
        public async Task WriteHeader()
        {
            await _writer.WriteAsync(_template.Substring(0, _beginLoopIndex));  
        }

        public async Task WriteFooter()
        {
            await _writer.WriteAsync(_template.Substring(_endLoopIndex + TemplateToken.EndLoop.Length,
               _template.Length - _endLoopIndex - TemplateToken.EndLoop.Length));
        }

        public async Task WriteScript(string script)
        {
            await _writer.WriteAsync(script);
        }
    }
}
