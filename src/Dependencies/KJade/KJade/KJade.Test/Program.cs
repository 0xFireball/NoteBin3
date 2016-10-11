using KJade.Compiler.Html;
using System.IO;

namespace KJade.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var compiler = new JadeHtmlCompiler();
            var context = new { Name = "Bob" };
            var compiledHtml = compiler.Compile(File.ReadAllText("test.jade"), context);
            File.WriteAllText("test.jade.html", compiledHtml.Value.ToString());
        }
    }
}