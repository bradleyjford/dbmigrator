using System;

namespace DbMigrator
{
    internal interface ILogger
    {
        void Error(string format, params object[] values);
        void Warn(string format, params object[] values);
        void Info(string format, params object[] values);
    }
}
