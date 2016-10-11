using KJade.Ast;
using KJade.Parser;
using System.Text.RegularExpressions;

namespace KJade.Compiler
{
    public abstract class JadeCompiler : IJadeCompiler
    {
        private static readonly Regex SingleVariableSubstitutionRegex = new Regex(@"(?<Encode>!)?#{model(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))}", RegexOptions.Compiled);

        public static string XmlEncode(string value)
        {
            return value
              .Replace("<", "&lt;")
              .Replace(">", "&gt;")
              .Replace("\"", "&quot;")
              .Replace("'", "&apos;")
              .Replace("&", "&amp;");
        }

        public static string XmlDecode(string value)
        {
            return value
              .Replace("&lt;", "<")
              .Replace("&gt;", ">")
              .Replace("&quot;", "\"")
              .Replace("&apos;", "'")
              .Replace("&amp;", "&");
        }

        public string ReplaceInput(string input, object model)
        {
            //Replace variables
            var replacedInput = SingleVariableSubstitutionRegex.Replace(input, m =>
            {
                var properties = ModelReflectionUtil.GetCaptureGroupValues(m, "ParameterName");
                var substitution = ModelReflectionUtil.GetPropertyValueFromParameterCollection(model, properties);
                if (!substitution.Item1)
                {
                    return "[ERR!]";
                }

                if (substitution.Item2 == null)
                {
                    return string.Empty;
                }
                return m.Groups["Encode"].Success ? XmlEncode(substitution.Item2.ToString()) : substitution.Item2.ToString();
            });
            return replacedInput;
        }

        public IJadeCompileResult Compile(string input, object model)
        {
            var replacedInput = ReplaceInput(input, model);
            return Compile(replacedInput);
        }

        public IJadeCompileResult Compile(string input)
        {
            var lexer = new JadeLexer();
            lexer = new JadeLexer();
            var tokens = lexer.ReadCode(input);
            var parser = new JadeParser();
            var ast = parser.ParseTokens(tokens);
            return CompileFromAst(ast);
        }

        protected abstract IJadeCompileResult CompileFromAst(JRootNode ast);
    }
}