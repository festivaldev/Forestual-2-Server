using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using F2Core.Extension;

namespace Forestual2ServerCS.Management
{
    public class ExtensionManager
    {
        public static List<IExtension> Extensions = new List<IExtension>();

        public static void LoadExtension(string path) {
            if (File.Exists(path) && new FileInfo(path).Extension == ".dll") {
                Assembly.LoadFile(path);
                var ExtensionType = typeof(IExtension);
                var AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => ExtensionType.IsAssignableFrom(type) && type.IsClass)
                    .ToArray();
                foreach (var Type in AssemblyTypes) {
                    var Extension = (IExtension) Activator.CreateInstance(Type);
                    Extension.Path = path;
                    if (Extensions.Any(e => e.Namespace == Extension.Namespace)) {
                        continue;
                    }
                    Extensions.Add(Extension);
                }
            }
        }
    }
}
