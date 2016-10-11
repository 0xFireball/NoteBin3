using KJade.Ast;
using System.Collections.Generic;
using System.Linq;

namespace KJade.Parser
{
    /// <summary>
    /// Parses a list of tokens and builds an AST.
    /// </summary>
    public class JadeParser
    {
        /// <summary>
        /// Create a node based on the input JToken
        /// </summary>
        /// <param name="inputToken"></param>
        /// <returns></returns>
        private JNode CreateNodeFromJadeToken(JadeToken inputToken)
        {
            return new JNode
            {
                Attributes = inputToken.Attributes,
                Classes = inputToken.Classes,
                Element = inputToken.NodeName,
                Id = inputToken.Id,
                IndentationLevel = inputToken.IndentationLevel,
                Value = inputToken.Value,
            };
        }

        public JRootNode ParseTokens(List<JadeToken> tokens)
        {
            string originalCode = string.Concat(tokens.Select(t => t.TextRepresentation + new string(' ', t.IndentationLevel) + '\n'));
            JRootNode rootNode = new JRootNode();
            //Parse the input tokens and build an AST.
            Stack<int> nestingLevel = new Stack<int>();
            nestingLevel.Push(-1); //-1, signifying an unreachable nesting level. All nodes are children of this root.
            Stack<JNode> lastNodeOnLevel = new Stack<JNode>();
            lastNodeOnLevel.Push(rootNode);
            foreach (var jToken in tokens)
            {
                var newNode = CreateNodeFromJadeToken(jToken);

                if (newNode.Attributes.Count == 0 && newNode.Element == null && newNode.Classes.Count == 0 && newNode.Id == null && newNode.Value == null)
                {
                    //Unfortunately, this node is useless!
                    continue;
                }

                var currentNestingLevel = nestingLevel.Peek();
                if (jToken.IndentationLevel > currentNestingLevel + 1)
                {
                    //The indentation level jumped illegally
                    var errorPosition = CalculateCodePosition(tokens, jToken);
                    throw new KJadeParserException("Unexpected indentation: the indentation may not change by more than one value at a time!", errorPosition);
                }

                if (jToken.IndentationLevel > currentNestingLevel)
                {
                    //We're nesting deeper
                    lastNodeOnLevel.Peek().Children.Add(newNode);
                    nestingLevel.Push(currentNestingLevel + 1);
                    lastNodeOnLevel.Push(newNode);
                }
                else if (jToken.IndentationLevel == currentNestingLevel)
                {
                    //We're on the same level
                    lastNodeOnLevel.Pop(); //Pop the previous node added, because it's on the same level
                    lastNodeOnLevel.Peek().Children.Add(newNode);
                    lastNodeOnLevel.Push(newNode);
                }
                else if (jToken.IndentationLevel < currentNestingLevel)
                {
                    //We're going back toward the root
                    while (nestingLevel.Peek() > jToken.IndentationLevel)
                    {
                        //Pop off parent layers
                        nestingLevel.Pop();
                        lastNodeOnLevel.Pop();
                    }
                    //We've popped our way down
                    //Indent level has been decreased
                    //Pop the same-level last node on the level
                    lastNodeOnLevel.Pop();
                    lastNodeOnLevel.Peek().Children.Add(newNode);
                    lastNodeOnLevel.Push(newNode);
                }
            }
            nestingLevel.Pop(); //We've reached the end!

            //Prune the tree


            return rootNode;
        }

        private CodePosition CalculateCodePosition(List<JadeToken> allTokens, JadeToken currentToken)
        {
            var precedingTokens = allTokens.Take(allTokens.Count - allTokens.IndexOf(currentToken));
            var precedingCode = string.Concat(precedingTokens.Select(t => t.TextRepresentation + new string(' ', t.IndentationLevel) + '\n'));
            string[] precedingLines = precedingCode.Split(' ');
            return new CodePosition(precedingLines.Length + 1, 0);
        }
    }
}