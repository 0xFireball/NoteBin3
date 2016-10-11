using System.Collections.Generic;
using System.Text;

namespace KJade.Ast
{
    public class JNode
    {
        public List<JNode> Children { get; set; } = new List<JNode>();

        public string Element { get; set; }

        public List<string> Classes { get; set; }

        public string Id { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public int IndentationLevel { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"[{Element}] {Value}";
        }
    }
}