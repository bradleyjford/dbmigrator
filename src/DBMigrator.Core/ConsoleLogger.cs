﻿using System;
using System.IO;

namespace DbMigrator.Core
{
    public class ConsoleLogger : ILogger
    {
        private readonly bool _verbose;
        private readonly TextWriter _errorWriter;
        private readonly TextWriter _writer;

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

        private void WriteLine(TextWriter wrtier, ConsoleColor color, string format, object[] values)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = color;
            wrtier.WriteLine(format, values);

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
