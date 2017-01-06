using F2Core.Compatibility;

namespace Forestual2ServerCS
{
    public class Version : F2Core.Compatibility.Version
    {
        public override int Major { get; set; } = 2;
        public override int Minor { get; set; } = 0;
        public override int Patch { get; set; } = 5;
        public override VersioningProfiler.Suffixes Suffix { get; set; } = VersioningProfiler.Suffixes.beta;
        public override string ReleaseDate { get; set; } = "17w01";
        public override string Commit { get; set; } = "8c0b0ab";
        public override string SupportedVersion { get; set; } = "2.1.22 [460e573]";
    }
}
