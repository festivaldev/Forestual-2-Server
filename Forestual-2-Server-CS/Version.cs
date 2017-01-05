using F2Core.Compatibility;

namespace Forestual2ServerCS
{
    public class Version : F2Core.Compatibility.Version
    {
        public override int Major { get; set; } = 2;
        public override int Minor { get; set; } = 0;
        public override int Patch { get; set; } = 3;
        public override VersioningProfiler.Suffixes Suffix { get; set; } = VersioningProfiler.Suffixes.none;
        public override string ReleaseDate { get; set; } = "17w01";
        public override string Commit { get; set; } = "7c89075";
        public override string SupportedVersion { get; set; } = "2.1.7-beta [72d8ff8]";
    }
}
