using System;

namespace DbMigrator.Core
{
    public interface ILogger
    {
        void Error(string format, params object[] values);
        void Warn(string format, params object[] values);
        void Info(string format, params object[] values);
        void Verbose(string format, params object[] values);
    }
}
