using System.Text;

namespace KJade.Compiler.Html
{
    public class HtmlCompileResult : IJadeCompileResult
    {
        public StringBuilder Value { get; } = new StringBuilder();
    }
}