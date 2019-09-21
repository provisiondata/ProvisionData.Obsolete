using System;
using System.Collections.Concurrent;
using System.Linq;

namespace ProvisionData.GitVersion
{
    public class GitVersionInfo
    {
        private static readonly ConcurrentDictionary<Type, GitVersionInfo> Versions = new ConcurrentDictionary<Type, GitVersionInfo>();

        public static GitVersionInfo ForAssemblyContaining<T>()
        {
            var key = typeof(T);
            return Versions.GetOrAdd(key, type =>
            {
                var assembly = type.Assembly;
                var typeInfo = assembly.DefinedTypes.SingleOrDefault(t => String.Equals(t.Name, "GitVersionInformation", StringComparison.InvariantCulture));
                if (typeInfo is null)
                {
                    throw new InvalidOperationException($"The \"{assembly.FullName}\" does not contain a \"GitVersionInformation\" type.  Are you sure GitVersion has been setup correctly?");
                }
                return new GitVersionInfo(typeInfo.AsType());
            });
        }

        private readonly Type _type;

        public GitVersionInfo(Type type) => _type = type ?? throw new ArgumentNullException(nameof(type));

        private String GetValue(String name) => _type.GetField(name).GetValue(null).ToString();

        private String _major;
        public String Major => _major ?? (_major = GetValue("Major"));

        private String _minor;
        public String Minor => _minor ?? (_minor = GetValue("Minor"));

        private String _patch;
        public String Patch => _patch ?? (_patch = GetValue("Patch"));

        private String _preReleaseTag;
        public String PreReleaseTag => _preReleaseTag ?? (_preReleaseTag = GetValue("PreReleaseTag"));

        private String _preReleaseTagWithDash;
        public String PreReleaseTagWithDash => _preReleaseTagWithDash ?? (_preReleaseTagWithDash = GetValue("PreReleaseTagWithDash"));

        private String _preReleaseLabel;
        public String PreReleaseLabel => _preReleaseLabel ?? (_preReleaseLabel = GetValue("PreReleaseLabel"));

        private String _preReleaseNumber;
        public String PreReleaseNumber => _preReleaseNumber ?? (_preReleaseNumber = GetValue("PreReleaseNumber"));

        private String _weightedPreReleaseNumber;
        public String WeightedPreReleaseNumber => _weightedPreReleaseNumber ?? (_weightedPreReleaseNumber = GetValue("WeightedPreReleaseNumber"));

        private String _buildMetaData;
        public String BuildMetaData => _buildMetaData ?? (_buildMetaData = GetValue("BuildMetaData"));

        private String _buildMetaDataPadded;
        public String BuildMetaDataPadded => _buildMetaDataPadded ?? (_buildMetaDataPadded = GetValue("BuildMetaDataPadded"));

        private String _fullBuildMetaData;
        public String FullBuildMetaData => _fullBuildMetaData ?? (_fullBuildMetaData = GetValue("FullBuildMetaData"));

        private String _majorMinorPatch;
        public String MajorMinorPatch => _majorMinorPatch ?? (_majorMinorPatch = GetValue("MajorMinorPatch"));

        private String _semVer;
        public String SemVer => _semVer ?? (_semVer = GetValue("SemVer"));

        private String _legacySemVer;
        public String LegacySemVer => _legacySemVer ?? (_legacySemVer = GetValue("LegacySemVer"));

        private String _legacySemVerPadded;
        public String LegacySemVerPadded => _legacySemVerPadded ?? (_legacySemVerPadded = GetValue("LegacySemVerPadded"));

        private String _assemblySemVer;
        public String AssemblySemVer => _assemblySemVer ?? (_assemblySemVer = GetValue("AssemblySemVer"));

        private String _assemblySemFileVer;
        public String AssemblySemFileVer => _assemblySemFileVer ?? (_assemblySemFileVer = GetValue("AssemblySemFileVer"));

        private String _fullSemVer;
        public String FullSemVer => _fullSemVer ?? (_fullSemVer = GetValue("FullSemVer"));

        private String _informationalVersion;
        public String InformationalVersion => _informationalVersion ?? (_informationalVersion = GetValue("InformationalVersion"));

        private String _branchName;
        public String BranchName => _branchName ?? (_branchName = GetValue("BranchName"));

        private String _sha;
        public String Sha => _sha ?? (_sha = GetValue("Sha"));

        private String _shortSha;
        public String ShortSha => _shortSha ?? (_shortSha = GetValue("ShortSha"));

        private String _nuGetVersionV2;
        public String NuGetVersionV2 => _nuGetVersionV2 ?? (_nuGetVersionV2 = GetValue("NuGetVersionV2"));

        private String _nuGetVersion;
        public String NuGetVersion => _nuGetVersion ?? (_nuGetVersion = GetValue("NuGetVersion"));

        private String _nuGetPreReleaseTagV2;
        public String NuGetPreReleaseTagV2 => _nuGetPreReleaseTagV2 ?? (_nuGetPreReleaseTagV2 = GetValue("NuGetPreReleaseTagV2"));

        private String _nuGetPreReleaseTag;
        public String NuGetPreReleaseTag => _nuGetPreReleaseTag ?? (_nuGetPreReleaseTag = GetValue("NuGetPreReleaseTag"));

        private String _versionSourceSha;
        public String VersionSourceSha => _versionSourceSha ?? (_versionSourceSha = GetValue("VersionSourceSha"));

        private String _commitsSinceVersionSource;
        public String CommitsSinceVersionSource => _commitsSinceVersionSource ?? (_commitsSinceVersionSource = GetValue("CommitsSinceVersionSource"));

        private String _commitsSinceVersionSourcePadded;
        public String CommitsSinceVersionSourcePadded => _commitsSinceVersionSourcePadded ?? (_commitsSinceVersionSourcePadded = GetValue("CommitsSinceVersionSourcePadded"));

        private String _commitDate;
        public String CommitDate => _commitDate ?? (_commitDate = GetValue("CommitDate"));
    }
}
