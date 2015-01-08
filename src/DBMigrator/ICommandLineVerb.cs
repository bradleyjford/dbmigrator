using System;
using System.Collections.Generic;

namespace DBMigrator
{
    abstract class CommandLineVerb
    {
        protected Dictionary<string, string> ParseArguments(string[] parameters)
        {
            var result = new Dictionary<string, string>(parameters.Length);

            foreach (var param in parameters)
            {
                var parts = param.Split('=');

                result.Add(parts[0], parts[1]);
            }

            return result;
        }

        public abstract void Execute();
    }
}
