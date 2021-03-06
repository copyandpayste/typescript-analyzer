﻿// Modifications Copyright Rich Newman 2017
using System.ComponentModel;
using System.IO;
using WebLinter;

namespace WebLinterTest
{
    class Settings : ISettings
    {
        private static Settings _settings;

        public static Settings Instance
        {
            get
            {
                if (_settings == null)
                    _settings = new Settings();

                return _settings;
            }
        }

        public static string CWD
        {
            get { return new FileInfo("../../artifacts/").FullName; }
        }

        public bool TSLintEnable { get; set; } = true;
        public bool TSLintWarningsAsErrors { get; set; }
    }
}
