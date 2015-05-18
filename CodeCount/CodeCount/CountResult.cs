namespace CodeCount
{
    class CountResult
    {
        public int TotalLines { get; set; }
        public int EmptyLines { get; set; }
        public int CommentLines { get; set; }
        public int CodeLines { get; set; }
        public int CodeAndCommentLines { get; set; }

        public CountResult()
        {
            TotalLines          = 0;
            EmptyLines          = 0;
            CommentLines        = 0;
            CodeLines           = 0;
            CodeAndCommentLines = 0;
        }

        public void Add(CountResult other)
        {
            TotalLines          += other.TotalLines;
            EmptyLines          += other.EmptyLines;
            CommentLines        += other.CommentLines;
            CodeLines           += other.CodeLines;
            CodeAndCommentLines += other.CodeAndCommentLines;
        }

        public override string ToString()
        {
            return string.Format("TotalLines:{0};EmptyLines:{1};CommentLines:{2};CodeLines:{3};CodeAndCommentLines:{4}", TotalLines, EmptyLines, CommentLines, CodeLines, CodeAndCommentLines);
        }
    }
}
