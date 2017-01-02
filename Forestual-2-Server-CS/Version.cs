using F2Core.Compatibility;

namespace Forestual2ServerCS
{
    public class Version : F2Core.Compatibility.Version
    {
        public override int Major { get; set; } = 1;
        public override int Minor { get; set; } = 0;
        public override int Patch { get; set; } = 0;
        public override string Suffix { get; set; } = "";
        public override string ReleaseDate { get; set; } = "17w1";
        public override string Commit { get; set; } = "7fc1fbd";
        public override string SupportedVersion { get; set; } = "1.0.0 [9f8b271]";
    }
}
