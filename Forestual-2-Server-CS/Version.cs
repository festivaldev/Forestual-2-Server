using F2Core.Compatibility;

namespace Forestual2ServerCS
{
    public class Version : F2Core.Compatibility.Version
    {
        public override int Major { get; set; } = 2;
        public override int Minor { get; set; } = 1;
        public override int Patch { get; set; } = 16;
        public override VersioningProfiler.Suffixes Suffix { get; set; } = VersioningProfiler.Suffixes.none;
        public override string ReleaseDate { get; set; } = "17w01";
        public override string Commit { get; set; } = "e2c2e08";
        public override string SupportedVersion { get; set; } = "2.2.34 [b506f75]";
    }
}
