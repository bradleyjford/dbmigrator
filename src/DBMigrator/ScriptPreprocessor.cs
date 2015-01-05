using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DbMigrator
{
    internal class ScriptPreprocessor
    {
        private static readonly Regex ParameterRegex = 
            new Regex(@"\$\((?<name>[^)]+)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string Process(string script, IDictionary<string, string> arguments)
        {
            foreach (var argument in arguments)
            {
                script = script.Replace("$(" + argument.Key + ")", argument.Value);
            }

            if (ParameterRegex.IsMatch(script))
            {
                // TODO: fix exception message
                throw new Exception("Unresolved script parameter ...");
            }

            return script;
        }
    }
}
