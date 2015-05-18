using System.IO;

namespace CodeCount
{
    /// <summary>
    /// 文件行数统计类
    /// </summary>
    class Statistician
    {
        public void Init(Setting set)
        {
            _lineCommentStr = set.LineCommentStr;
            _blockCommentStartStr = set.BlockCommentStartStr;
            _blockCommentEndStr = set.BlockCommentEndStr;
            _lineStringStr = set.LineStringArray;
            _blockStringStartStr = set.BlockStringStartStr;
            _blockStringEndStr = set.BlockStringEndStr;
        }

        public CountResult StartNew(Stream stream)
        {
            this._stream = stream;

            _lineStringStart = 0;
            _blockStringLeftLength = 0;
            _blockStringRightLength = 0;
            _lineCommentLength = 0;
            _blockCommentLeftLength = 0;
            _blockCommentRightLength = 0;
            _result = new CountResult();

            Calc();

            return _result;
        }

        private void Calc()
        {
            //记录当前行的一些关键特征：
            int column = -1;//非空字符的个数
            int lastCommentEnd = -1;//上一个注释结束的位置，用以判断两个注释间是否夹杂了代码

            //记录扫描的状态
            bool hasLineCommentLeft = false;//是否已存在行注释
            bool hasBlockCommentLeft = false;//是否已存在块注释头
            bool hasLineStrLeft = false;//是否已存在行字符串头
            bool hasBlockStrLeft = false;//是否已存在块字符串头
            bool hasBlockCommentRight = false;//是否已存在块字符串尾
            bool hasCodeAndBlock = false;//是否既存在代码又存在注释

            int buf;//缓存读取的字符
            byte cur = 0;//当前字符
            byte last = 0;//上一个字符

            //逐字符扫描文件
            while ((buf = _stream.ReadByte()) != -1)
            {
                last = cur;
                cur = (byte)buf;

                //遇到行尾字符
                if (cur == LINEEND)
                {
                    _result.TotalLines++;
                    if (column == -1)
                    {
                        _result.EmptyLines++;
                    }
                    else
                    {
                        if (hasCodeAndBlock || (hasBlockCommentRight && column != lastCommentEnd))
                        {
                            _result.CodeAndCommentLines++;
                        }
                        else if (hasLineCommentLeft || hasBlockCommentLeft || hasBlockCommentRight)
                        {
                            _result.CommentLines++;
                        }
                        else _result.CodeLines++;
                    }

                    //一行结束，重置这些标志字段
                    column = -1;
                    lastCommentEnd = -1;

                    hasLineCommentLeft = false;
                    hasLineStrLeft = false;
                    hasBlockCommentRight = false;
                    hasCodeAndBlock = false;

                    _lineStringStart = 0;
                    _blockStringLeftLength = 0;
                    _blockStringRightLength = 0;
                    _lineCommentLength = 0;
                    _blockCommentLeftLength = 0;
                    _blockCommentRightLength = 0;
                }
                else if (cur < 33 || cur == 127)
                {
                    //空字符无需处理
                }
                else
                {

                    column++;

                    if (hasLineCommentLeft)
                    {
                        //本行已存在行注释，后续都不再处理
                    }
                    else if (hasBlockCommentLeft)
                    {
                        if (IsBlockCommentRight(cur))
                        {
                            hasBlockCommentLeft = false;
                            hasBlockCommentRight = true;
                            lastCommentEnd = column;
                        }
                    }
                    //行字符串判断结束标记需要处理转义字符的情况
                    else if (hasLineStrLeft)
                    {
                        if (IsLineStringEnd(cur) && last != ESCAPE)
                        {
                            hasLineStrLeft = false;
                        }
                    }
                    //块字符串无需判断转义字符
                    else if (hasBlockStrLeft)
                    {
                        if (IsBlockStringRight(cur))
                        {
                            hasBlockStrLeft = false;
                        }
                    }
                    else
                    {
                        if (IsLineComment(cur))
                        {
                            hasLineCommentLeft = true;
                            //如果注释前有代码，则标记下来
                            if (column - _lineCommentStr.Length + 1 != 0)
                            {
                                hasCodeAndBlock = true;
                            }
                        }
                        else if (IsBlockCommentLeft(cur))
                        {
                            hasBlockCommentLeft = true;

                            //遇到块注释左标记时，判断lastCommentEnd和cur之间是否有夹杂非注释内容
                            if (column - _blockCommentStartStr.Length - lastCommentEnd > 0)
                            {
                                hasCodeAndBlock = true;
                            }
                        }
                        else
                        {
                            if (IsBlockStringLeft(cur))
                            {
                                hasBlockStrLeft = true;
                            }
                            else if (IsLineString(cur))
                            {
                                hasLineStrLeft = true;
                                _lineStringStart = cur;
                            }
                        }
                    }
                }
            }
            //处理最后一行的统计。如果最后一行无任何内容则不计入总数
            if (cur != LINEEND && cur > 0)
            {
                _result.TotalLines++;
                if (column == -1)
                {
                    _result.EmptyLines++;
                }
                else
                {
                    if (hasCodeAndBlock || (hasBlockCommentRight && column != lastCommentEnd))
                    {
                        _result.CodeAndCommentLines++;
                    }
                    else if (hasLineCommentLeft || hasBlockCommentLeft || hasBlockCommentRight)
                    {
                        _result.CommentLines++;
                    }
                    else _result.CodeLines++;
                }
            }
        }

        #region Helper Functions
        private byte _lineStringStart = 0;

        private bool IsLineString(byte data)
        {
            bool isStringSp = false;
            foreach (var ch in _lineStringStr)
            {
                if (ch == data)
                {
                    isStringSp = true;
                    break;
                }
            }
            return isStringSp;
        }

        private bool IsLineStringEnd(byte data)
        {
            if (IsLineString(data))
            {
                if (_lineStringStart == data)
                {
                    return true;
                }
            }
            return false;
        }

        private int _blockStringLeftLength = 0;
        private bool IsBlockStringLeft(byte data)
        {
            return TryMatch(ref _blockStringStartStr, ref data, ref _blockStringLeftLength);
        }

        private int _blockStringRightLength = 0;
        private bool IsBlockStringRight(byte data)
        {
            return TryMatch(ref _blockStringEndStr, ref data, ref _blockStringRightLength);
        }

        private int _lineCommentLength = 0;
        private bool IsLineComment(byte data)
        {
            return TryMatch(ref _lineCommentStr, ref data, ref _lineCommentLength);
        }

        private int _blockCommentLeftLength = 0;
        private bool IsBlockCommentLeft(byte data)
        {
            return TryMatch(ref _blockCommentStartStr, ref data, ref _blockCommentLeftLength);
        }

        private int _blockCommentRightLength = 0;
        private bool IsBlockCommentRight(byte data)
        {
            return TryMatch(ref _blockCommentEndStr, ref data, ref _blockCommentRightLength);
        }

        private bool TryMatch(ref string baseStr, ref byte data, ref int count)
        {
            if (baseStr.Length == 0) return false;

            if (baseStr[count] == data)
            {
                count++;
            }
            else
            {
                if (count > 0)
                {
                    count = 0;
                    return TryMatch(ref baseStr, ref data, ref count);
                }
                else
                {
                    count = 0;
                    return false;
                }
            }
            if (count == baseStr.Length)
            {
                count = 0;
                return true;
            }
            return false;
        }
        #endregion


        private string _lineCommentStr;
        private string _blockCommentStartStr;
        private string _blockCommentEndStr;

        private char[] _lineStringStr;
        private string _blockStringStartStr;
        private string _blockStringEndStr;

        private Stream _stream;

        private CountResult _result;

        private const byte LINEEND = 10;
        private const byte ESCAPE = 92;
    }
}