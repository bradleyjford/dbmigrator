using System;
using System.IO;

namespace DBMigrator.Core
{
    public class ConsoleLogger : ILogger
    {
        readonly bool _verbose;
        readonly TextWriter _errorWriter;
        readonly TextWriter _writer;

        public ConsoleLogger(bool verbose)
        {
            _verbose = verbose;

            _errorWriter = Console.Error;
            _writer = Console.Out;
        }

        public void Error(string format, params object[] values)
        {
            WriteLine(_errorWriter, ConsoleColor.Red, format, values);
        }

        void WriteLine(TextWriter writer, ConsoleColor color, string format, object[] values)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            writer.WriteLine(format, values);

            Console.ForegroundColor = originalColor;
        }

        public void Warn(string format, params object[] values)
        {
            WriteLine(_writer, ConsoleColor.Yellow, format, values);
        }

        public void Info(string format, params object[] values)
        {
            WriteLine(_writer, ConsoleColor.Gray, format, values);
        }

        public void Verbose(string format, params object[] values)
        {
            if (_verbose)
            {
                WriteLine(_writer, ConsoleColor.Gray, format, values);
            }
        }
    }
}
