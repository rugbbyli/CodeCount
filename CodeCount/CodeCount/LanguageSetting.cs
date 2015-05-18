using System.Collections.Generic;

namespace CodeCount
{
    /// <summary>
    /// 支持的语言的配置项
    /// </summary>
    public static class LanguageSetting
    {
        static Dictionary<string, Setting> _settings = new Dictionary<string, Setting>();
        
        public static Dictionary<string, Setting> Setting
        {
            get { return _settings; }
        }
        
        static LanguageSetting()
        {
            var cs = new Setting()
            {
                ExtArrayStr          = "*.cs",
                LineCommentStr       = "//",
                BlockCommentStartStr = "/*",
                BlockCommentEndStr   = "*/",
                LineStringArray      = new char[]{ '\"' },
                BlockStringStartStr  = "@\"",
                BlockStringEndStr    = "\"",
            };
            _settings.Add("C#", cs);

            var c = new Setting()
            {
                ExtArrayStr          = "*.c *.h",
                LineCommentStr       = "//",
                BlockCommentStartStr = "/*",
                BlockCommentEndStr   = "*/",
                LineStringArray      = new char[] { '\"' },
                BlockStringStartStr  = "",
                BlockStringEndStr    = "",
            };
            _settings.Add("C", c);

            var cpp = new Setting()
            {
                ExtArrayStr          = "*.cpp *.h",
                LineCommentStr       = "//",
                BlockCommentStartStr = "/*",
                BlockCommentEndStr   = "*/",
                LineStringArray      = new char[] { '\"' },
                BlockStringStartStr  = "",
                BlockStringEndStr    = "",
            };
            _settings.Add("C++", cpp);

            var java = new Setting()
            {
                ExtArrayStr          = "*.java",
                LineCommentStr       = "//",
                BlockCommentStartStr = "/*",
                BlockCommentEndStr   = "*/",
                LineStringArray      = new char[] { '\"' },
                BlockStringStartStr  = "",
                BlockStringEndStr    = "",
            };
            _settings.Add("JAVA", java);
        }
    }
}
