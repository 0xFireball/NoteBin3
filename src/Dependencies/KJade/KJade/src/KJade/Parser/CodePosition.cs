namespace KJade.Parser
{
    public struct CodePosition
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public CodePosition(int line, int column)
        {
            Line = line;
            Column = column;
        }
    }
}