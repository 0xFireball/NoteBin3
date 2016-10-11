using Nancy;
using System.IO;

namespace KJade.ViewEngine.Demo
{
    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}