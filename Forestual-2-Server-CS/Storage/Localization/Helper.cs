using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Forestual2ServerCS.Storage.Localization
{
    public class Helper
    {
        public static bool Exists(string languageCode) {
            return File.Exists(Application.StartupPath + "\\" + languageCode + ".json");
        }

        public static Values GetLocalization(string languageCode) {
            if (File.Exists(Path.Combine(Application.StartupPath, languageCode + ".json"))) {
                return JsonConvert.DeserializeObject<Values>(File.ReadAllText(Path.Combine(Application.StartupPath, languageCode + ".json")));
            }
            throw new FileNotFoundException();
        }

        public static void CreateDefault() {
            File.WriteAllText(Application.StartupPath + "\\en-en.json", JsonConvert.SerializeObject(new Values(), Formatting.Indented));
        }
    }
}
