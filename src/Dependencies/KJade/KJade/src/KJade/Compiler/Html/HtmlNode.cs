using System.Collections.Generic;

namespace KJade.Compiler.Html
{
    public class HtmlNode
    {
        public List<HtmlNode> Children { get; set; } = new List<HtmlNode>();

        public string Element { get; set; }

        public List<string> Classes { get; set; }

        public string Id { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public int IndentationLevel { get; set; }

        public string Value { get; set; }
    }
}