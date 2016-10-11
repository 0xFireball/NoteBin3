namespace KJade.Parser
{
    //An intermediate token type that is created by the lexer. This will be processed into a Token.
    public class RawToken
    {
        public int IndentLevel { get; set; }
        public string Value { get; set; }
    }
}