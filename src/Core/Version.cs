using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.IO;

namespace Core
{
    public static class Version
    {
        public static string Current
        {
            get
            {
                var version = ReadVersion();

#if DEBUG
                var suffix = "-dev";
#elif ALPHA
                var suffix = "-alpha";
#else
                var suffix = string.Empty;
#endif
                return $"{version}{suffix}";
            }
        }

        private static string ReadVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var embeddedProvider = new EmbeddedFileProvider(assembly, "Core");
            using (var stream = embeddedProvider.GetFileInfo(".version").CreateReadStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadLine();
            }
        }
    }
}