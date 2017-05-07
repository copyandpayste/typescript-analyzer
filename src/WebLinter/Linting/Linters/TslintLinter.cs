﻿// Modifications Copyright Rich Newman 2017
using Newtonsoft.Json.Linq;

namespace WebLinter
{
    internal class TsLintLinter : LinterBase
    {
        public TsLintLinter(ISettings settings) : base(settings)
        {
            Name = "TSLint";
            ConfigFileName = "tslint.json";
            IsEnabled = Settings.TSLintEnable;
        }

        protected override void ParseErrors(string output)
        {
            var array = JArray.Parse(output);

            foreach (JObject obj in array)
            {
                string fileName = obj["name"]?.Value<string>().Replace("/", "\\");

                if (string.IsNullOrEmpty(fileName))
                    continue;

                var le = new LintingError(fileName);
                le.Message = obj["failure"]?.Value<string>();
                le.LineNumber = obj["startPosition"]?["line"]?.Value<int>() ?? 0;
                le.ColumnNumber = obj["startPosition"]?["character"]?.Value<int>() ?? 0;
                le.EndLineNumber = obj["endPosition"]?["line"]?.Value<int>() ?? 0;
                le.EndColumnNumber = obj["endPosition"]?["character"]?.Value<int>() ?? 0;
                le.IsError = Settings.TSLintWarningsAsErrors;
                le.ErrorCode = obj["ruleName"]?.Value<string>();
                le.HelpLink = $"https://palantir.github.io/tslint/rules/{le.ErrorCode}";
                le.Provider = this;
                Result.Errors.Add(le);
            }
        }
    }
}
