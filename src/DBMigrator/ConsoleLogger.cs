using System;
using System.IO;

namespace DbMigrator
{
    internal class ConsoleLogger : ILogger
    {
        private readonly TextWriter _errorWriter;

        public ConsoleLogger()
        {
            _errorWriter = Console.Error;
        }

        public void Error(string format, params object[] values)
        {
            _errorWriter.WriteLine(format, values);
        }

        private void WriteLine(ConsoleColor color, string format, object[] values)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            Console.WriteLine(format, values);

            Console.ForegroundColor = originalColor;
        }

        public void Warn(string format, params object[] values)
        {
            WriteLine(ConsoleColor.Yellow, format, values);
        }

        public void Info(string format, params object[] values)
        {
            WriteLine(ConsoleColor.Gray, format, values);
        }
    }
}
