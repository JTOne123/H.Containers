using System;
using System.IO;
using System.Linq;
using System.Reflection;

#nullable enable

namespace H.Containers.Process.Utilities
{
    internal static class ResourcesUtilities
    {
        /// <summary>
        /// <![CDATA[Version: 1.0.0.1]]>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Stream ReadFileAsStream(string name, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

            return assembly.GetManifestResourceStream(resourceName) ?? throw new ArgumentException($"\"{name}\" is not found in embedded resources");
        }

        public static string[] GetResourcesNames(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            return assembly.GetManifestResourceNames().ToArray();
        }

        /// <summary>
        /// <![CDATA[Version: 1.0.0.1]]>
        /// <![CDATA[Dependency: ReadFileAsStream(string name, Assembly? assembly = null)]]>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static byte[] ReadFileAsBytes(string name, Assembly? assembly = null)
        {
            using var stream = ReadFileAsStream(name, assembly);
            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }
    }
}