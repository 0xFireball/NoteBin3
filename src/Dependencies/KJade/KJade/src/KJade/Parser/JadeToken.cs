using System.Collections.Generic;

namespace KJade.Parser
{
    /// <summary>
    /// The final representation of a token. These will be parsed by the parser.
    /// </summary>
    public class JadeToken
    {
        /// <summary>
        /// Stores the type of node. For example, "p" would represent a paragraph element
        /// </summary>
        public string NodeName { get; set; }

        public List<string> Classes { get; set; }

        public string Id { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public int IndentationLevel { get; set; }

        public string Value { get; set; }

        public string TextRepresentation { get; set; }

        public JadeToken(string nodeName, List<string> classes, string id, Dictionary<string, string> attributes, int indentationLevel)
        {
            NodeName = nodeName;
            Classes = classes;
            Id = id;
            Attributes = attributes;
            IndentationLevel = indentationLevel;
        }
    }
}