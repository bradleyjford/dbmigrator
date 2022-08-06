using System.Runtime.Serialization;

namespace DbMigrator.Core;

[Serializable]
public class DbMigrationException : Exception
{
    public DbMigrationException(string filename, int lineNumber, string message) 
        : base(message)
    {
        Filename = filename;
        LineNumber = lineNumber;
    }

    public DbMigrationException(string filename, int lineNumber, string message, Exception inner) 
        : base(message, inner)
    {
        Filename = filename;
        LineNumber = lineNumber;
    }

    protected DbMigrationException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
        Filename = info.GetString(nameof(Filename))!;
        LineNumber = info.GetInt32(nameof(LineNumber));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        
        info.AddValue(nameof(Filename), Filename);
        info.AddValue(nameof(LineNumber), LineNumber);
    }

    public string Filename { get; }
    public int LineNumber { get; }
}