using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using F2Core;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Management
{
    public class Luva
    {
        public static Dictionary<string, int> ExtensionSeverities = new Dictionary<string, int>();

        public static Severity GetSeverity(string luvaValue) {
            if (luvaValue.StartsWith("forestual.")) {
                var InternalSeverities = JsonConvert.DeserializeObject<List<Severity>>(File.ReadAllText(Path.Combine(Application.StartupPath, "severities.luva")));
                var Severity = InternalSeverities.Find(s => s.Values.Contains(luvaValue));
                return Severity ?? new Severity { Color = "#0066CC", Description = "Unknown" };
            }
            if (ExtensionSeverities.ContainsKey(luvaValue)) {
                var InternalSeverities = JsonConvert.DeserializeObject<List<Severity>>(File.ReadAllText(Path.Combine(Application.StartupPath, "severities.luva")));
                var Severity = InternalSeverities.Find(s => s.Level == ExtensionSeverities[luvaValue]);
                return Severity ?? new Severity { Color = "#0066CC", Description = "Unknown" };
            }
            return new Severity { Color = "#0066CC", Description = "Unknown" };
        }
    }
}
