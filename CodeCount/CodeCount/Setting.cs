using System;
using System.Text;

namespace CodeCount
{
    /// <summary>
    /// 具体语言的配置项类。程序接收cmd启动参数，将参数转换为此类的实例。参数格式如下：
    /// -p "path" -e "ext1 ext2 .. extn" -c "line_comment_str block_comment_str_start block_comment_str_end" -s "line_string_str1 line_string_str2 .. line_string_strn,block_string_str_start block_string_str_end"
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// 磁盘路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 文件扩展名集合
        /// </summary>
        public string[] ExtArray { get; set; }

        /// <summary>
        /// 文件扩展名集合的字符串形式
        /// </summary>
        public string ExtArrayStr
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var ch in ExtArray)
                {
                    sb.Append(ch);
                    sb.Append(SP_SPACE);
                }
                sb.Length -= 1;
                return sb.ToString();
            }
            set
            {
                var exts = value.Split(SP_SPACE);
                ExtArray = new string[exts.Length];
                for (int index = 0; index < exts.Length; index++)
                {
                    ExtArray[index] = exts[index];
                }
            }
        }

        /// <summary>
        /// 单行注释的标记字符串
        /// </summary>
        public string LineCommentStr { get; set; }

        /// <summary>
        /// 块注释的起始标记字符串
        /// </summary>
        public string BlockCommentStartStr { get; set; }

        /// <summary>
        /// 块注释的结束标记字符串
        /// </summary>
        public string BlockCommentEndStr { get; set; }

        /// <summary>
        /// 单行字符串的标记字符集合
        /// </summary>
        public char[] LineStringArray { get; set; }

        /// <summary>
        /// 块字符串的起始标记字符串
        /// </summary>
        public string BlockStringStartStr { get; set; }

        /// <summary>
        /// 块字符串的结束标记字符串
        /// </summary>
        public string BlockStringEndStr { get; set; }

        /// <summary>
        /// 单行字符串的标记字符集合的字符串形式
        /// </summary>
        public string LineStringArrayStr
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var ch in LineStringArray)
                {
                    sb.Append(ch);
                    sb.Append(SP_SPACE);
                }
                sb.Length -= 1;
                return sb.ToString();
            }
            set
            {
                var lineStrs = value.Split(SP_SPACE);
                LineStringArray = new char[lineStrs.Length];
                for (int index = 0; index < lineStrs.Length; index++)
                {
                    LineStringArray[index] = lineStrs[index][0];
                }
            }
        }

        /// <summary>
        /// 将Setting实例转换为cmd启动参数
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var cmt = string.Format("{1}{0}{2}{0}{3}", SP_SPACE, LineCommentStr, BlockCommentStartStr, BlockCommentEndStr);
            cmt = cmt.Replace("\"", "\\\"");
            var blockStr = string.Format("{1}{0}{2}", SP_SPACE, BlockStringStartStr, BlockStringEndStr);
            var str = string.Format("{1}{0}{2}", SP_COMMA, LineStringArrayStr, blockStr);
            str = str.Replace("\"", "\\\"");
            return string.Format("{1}{0}\"{2}\"{0}{3}{0}\"{4}\"{0}{5}{0}\"{6}\"{0}{7}{0}\"{8}\"", SP_SPACE, ARG_PATH, Path, ARG_EXT, ExtArrayStr, ARG_CMT, cmt, ARG_STR, str);
        }

        /// <summary>
        /// 将cmd启动参数转换为Setting实例
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Setting Parse(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("The parameters can not be empty: -p -c");
            }
            if (args.Length % 2 != 0)
            {
                throw new ArgumentException("The parameter's count is not correct, please check your input");
            }

            Setting setting = new Setting();

            for (int i = 0; i < args.Length; i += 2)
            {
                if (args[i].ToLower().Equals(ARG_PATH))
                {
                    setting.Path = args[i + 1];
                }
                else if (args[i].ToLower().Equals(ARG_CMT))
                {
                    var comments = args[i + 1].Split(SP_SPACE);
                    if (comments.Length == 0) throw new ArgumentException("The format of parameter [-c] is not correct, please check your input");
                    setting.LineCommentStr = comments[0];
                    setting.BlockCommentStartStr = comments.Length > 1 ? comments[1] : string.Empty;
                    setting.BlockCommentEndStr = comments.Length > 2 ? comments[2] : string.Empty;
                }
                else if (args[i].ToLower().Equals(ARG_EXT))
                {
                    setting.ExtArrayStr = args[i + 1];
                }
                else if (args[i].ToLower().Equals(ARG_STR))
                {
                    var stringStrs = args[i + 1].Split(SP_COMMA);
                    if (stringStrs.Length == 0)
                    {
                        throw new ArgumentException("The format of parameter [-s] is not correct, please check your input");
                    }
                    var lineStrs = stringStrs[0].Split(SP_SPACE);
                    setting.LineStringArray = new char[lineStrs.Length];
                    for (int index = 0; index < lineStrs.Length; index++)
                    {
                        setting.LineStringArray[index] = lineStrs[index][0];
                    }
                    if (stringStrs.Length > 1)
                    {
                        var blockStrs = stringStrs[1].Split(SP_SPACE);
                        if (blockStrs.Length != 2)
                        {
                            throw new ArgumentException("The format of parameter [-s] is not correct, please check your input");
                        }
                        setting.BlockStringStartStr = blockStrs[0];
                        setting.BlockStringEndStr = blockStrs[1];
                    }
                }
                else
                {
                    //throw new ArgumentException("The parameter type is not recognized" + args[i]);
                }
            }
            return setting;
        }

        const string ARG_PATH = "-p";
        const string ARG_EXT = "-e";
        const string ARG_STR = "-s";
        const string ARG_CMT = "-c";
        const char SP_SPACE = ' ';
        const char SP_COMMA = ',';
    }
}