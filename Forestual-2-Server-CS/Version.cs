using F2Core.Compatibility;

namespace Forestual2ServerCS
{
    public class Version : F2Core.Compatibility.Version
    {
        public override int Major { get; set; } = 2;
        public override int Minor { get; set; } = 2;
        public override int Patch { get; set; } = 0;
        public override VersioningProfiler.Suffixes Suffix { get; set; } = VersioningProfiler.Suffixes.alpha;
        public override string ReleaseDate { get; set; } = "17w05";
        public override string Commit { get; set; } = "b236067";
        public override string SupportedVersion { get; set; } = "2.2.41 [cf4640d]";
    }
}
