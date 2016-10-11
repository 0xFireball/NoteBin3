using KJade.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KJade.Parser
{
    public class JadeLexer
    {
        public string[] IgnoredCharacters { get; } = new string[] { "\r" };

        public void ReadCode(StringBuilder input)
        {
            ReadCode(input.ToString());
        }

        private Tuple<int, string> ReadIndentedToken(string line, string indentIndicator)
        {
            if (indentIndicator == "") return new Tuple<int, string>(0, line); //If no indents, then the indent level is 0
            string rl = line;
            string ii = indentIndicator;
            int indLv = 0;
            for (int i = 1; rl.StartsWith(ii, StringComparison.CurrentCulture); i++)
            {
                rl = rl.Eat(ii);
                indLv = i;
            }
            return new Tuple<int, string>(indLv, rl);
        }

        private string InferIndentIndicator(string[] codeLines)
        {
            //Read some raw indents
            foreach (string line in codeLines)
            {
                string rl = line;
                //Check if the first character is an indent.
                //If not, just continue in the loop.

                if (rl.Length == 0) continue;

                switch (rl[0])
                {
                    case ' ':
                        //Space indents
                        //We have to find out how many spaces
                        int sc = 0; //space count
                        for (int i = 1; rl[0] == ' '; i++)
                        {
                            rl = rl.Substring(1);
                            sc = i;
                        }
                        return new string(' ', sc);

                    case '\t':
                        //Tab indents
                        return "\t";
                }
            }
            //None of the lines seemed indented.
            return "";
        }

        public List<JadeToken> ReadCode(string input)
        {
            input = input.Strip(IgnoredCharacters) + "\n"; //Strip useless characters and add a trailing \n, as it simplifies lexing

            Queue<RawToken> rawTokens = new Queue<RawToken>(); //a queue of raw tokens to be processed
            string[] codeLines = input.Split('\n');
            string indentIndicator = InferIndentIndicator(codeLines);
            //Process indentation structure
            foreach (string line in codeLines)
            {
                //Read an indented token that has an indentation amount and a value not including the indent characters
                var indentedToken = ReadIndentedToken(line, indentIndicator);

                var lToken = new RawToken
                {
                    IndentLevel = indentedToken.Item1,
                    Value = indentedToken.Item2,
                };
                rawTokens.Enqueue(lToken);
            }
            //Create real Token objects based on the indentation structure
            List<JadeToken> jadeTokens = new List<JadeToken>();
            bool multilineScope = false; //whether we are in a multiline scope.
            int multilineScopeStart = 0;
            int previousIndentLevel = 0;
            foreach (var rawTok in rawTokens)
            {
                var processedTokValue = rawTok.Value;
                if (processedTokValue.StartsWith("//", StringComparison.CurrentCulture) || string.IsNullOrWhiteSpace(processedTokValue))
                {
                    continue;
                }
                if (multilineScope) //We're in a special area, normal rules don't apply
                {
                    if (rawTok.IndentLevel <= multilineScopeStart)
                    {
                        //The current indent level is less than where the multiline scope started
                        //Exit the multiline scope
                        multilineScope = false;
                        multilineScopeStart = 0;
                    }
                    else
                    {
                        //We're still in the multiline scope.
                        //Edit the previous token.
                        //Grab the last token
                        var prevTok = jadeTokens.Last();

                        //Append the current token's value to the previous token, along with a whitespace character
                        prevTok.Value += rawTok.Value + " ";
                        continue;
                    }
                }

                //Normal node token
                //Look for indicators showing end of node name
                var nodeNameRegex = new Regex(@"(^\w+(?=(\.|#|\()))|(^\w+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                var nodeNameMatch = nodeNameRegex.Match(processedTokValue); //Only match one
                //The match should have extracted the node name. If not, there is no node name.
                var nodeName = nodeNameMatch.Success ? nodeNameMatch.Value : null;
                //Don't remove the name yet, it will be used to verify later

                var classes = new List<string>();
                var nodeAttributes = new Dictionary<string, string>();
                string nodeIdAttribute = null; //none specified

                //Read attributes first, as they may contain characters that will trigger class and Id regexes

                //Look for an attribute group - looks like this: (attr="some value")
                var attributeGroupRegex = new Regex(@"\(([^\)]+)\)");
                var attributeGroupMatch = attributeGroupRegex.Match(processedTokValue);
                if (attributeGroupMatch.Success)
                {
                    //An attribute group was found!
                    var attributeGroupText = attributeGroupMatch.Value;
                    //Remove the opening ( and the closing )
                    attributeGroupText = attributeGroupText.Substring(1, attributeGroupText.Length - 2);
                    //Parse the attribute group
                    var attributeSeparatorRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    var nameValuePairs = attributeSeparatorRegex.Split(attributeGroupText);

                    foreach (var nvp in nameValuePairs)
                    {
                        var nvpRegex = new Regex("=(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        var components = nvpRegex.Split(nvp);
                        var attributeName = components[0];
                        var attributeValue = components[1];
                        //clean up attribute value
                        attributeValue = attributeValue.Eat("\""); //Eat the beginning `"` character
                        attributeValue = attributeValue.Last() == '"' ? attributeValue.Substring(0, attributeValue.Length - 1) : attributeValue; //Strip ending quote
                        //Save the attribute
                        nodeAttributes.Add(attributeName, attributeValue);
                    }
                }
                processedTokValue = attributeGroupRegex.Replace(processedTokValue, ""); //Remove matched part of value

                //Attempt to read classes
                var classRegex = new Regex(@"\.(\w|-)+");
                var classMatches = classRegex.Matches(processedTokValue);
                //Add matched class names to classes collection
                foreach (Match classMatch in classMatches)
                {
                    classes.Add(classMatch.Value);
                }
                processedTokValue = classRegex.Replace(processedTokValue, ""); //Remove matched part of value

                //Attempt to read id
                var idRegex = new Regex(@"\#(\w|-)+");
                var idMatch = idRegex.Match(processedTokValue);
                nodeIdAttribute = idMatch.Success ? idMatch.Value : nodeIdAttribute;
                processedTokValue = idRegex.Replace(processedTokValue, ""); //Remove matched part of value

                if (processedTokValue.EndsWith(".", StringComparison.CurrentCulture)) //This token isn't ending yet
                {
                    processedTokValue = processedTokValue.Substring(0, processedTokValue.Length - 1);
                    multilineScope = true;
                    multilineScopeStart = rawTok.IndentLevel; //Where the multiline scope started
                }

                string remainingValue = processedTokValue;

                //If node name is empty, and there are classes or an Id, it's a div. There also shouldn't be any value.
                if (string.IsNullOrWhiteSpace(processedTokValue))
                {
                    //If the processed token value is empty, then the name must also be empty
                    //The name was not removed, but attribute sets, ids, and classes were removed.
                    //This, if the remaining content is empty, the name is empty.
                    //In HTML, an empty name indicates that the node is a div.
                    //processedTokValue = "div"; //This is HTML-specific though, so we will mark the node name null, as none was specified
                    nodeName = null;
                }
                else
                {
                    //Normal node, get the value now
                    //All that should be left now is the name and the value.
                    //Remove the name
                    remainingValue = nodeNameRegex.Replace(remainingValue, "");
                    //Strip any unecessary whitespace
                    remainingValue = remainingValue.Trim();

                    //If the remaining value is just empty, it's null, not set
                    if (string.IsNullOrWhiteSpace(remainingValue))
                    {
                        remainingValue = null;
                    }
                }

                if (string.IsNullOrWhiteSpace(remainingValue)) //If just whitespace, make it null
                {
                    remainingValue = null;
                }

                var jTok = new JadeToken(nodeName, classes, nodeIdAttribute, nodeAttributes, rawTok.IndentLevel);

                jadeTokens.Add(jTok);
                jTok.Value = remainingValue;
                jTok.TextRepresentation = rawTok.Value; //Store the raw text representation
                previousIndentLevel = rawTok.IndentLevel;
            }
            return jadeTokens; //TODO: update
        }
    }
}