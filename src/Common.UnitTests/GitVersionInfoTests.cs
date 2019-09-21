using FluentAssertions;
using ProvisionData.GitVersion;
using Xunit;

namespace ProvisionData
{
    public class GitVersionInfoTests
    {
        [Fact]
        public void Works()
        {
            var gvi = GitVersionInfo.ForAssemblyContaining<GitVersionInfo>();

            gvi.Major.Should().NotBeEmpty();
            gvi.Minor.Should().NotBeEmpty();
            gvi.Patch.Should().NotBeEmpty();
            gvi.PreReleaseTag.Should().NotBeEmpty();
            gvi.PreReleaseTagWithDash.Should().NotBeEmpty();
            gvi.PreReleaseLabel.Should().NotBeEmpty();
            gvi.PreReleaseNumber.Should().NotBeEmpty();
            gvi.WeightedPreReleaseNumber.Should().NotBeEmpty();
            gvi.BuildMetaData.Should().NotBeEmpty();
            gvi.BuildMetaDataPadded.Should().NotBeEmpty();
            gvi.FullBuildMetaData.Should().NotBeEmpty();
            gvi.MajorMinorPatch.Should().NotBeEmpty();
            gvi.SemVer.Should().NotBeEmpty();
            gvi.LegacySemVer.Should().NotBeEmpty();
            gvi.LegacySemVerPadded.Should().NotBeEmpty();
            gvi.AssemblySemVer.Should().NotBeEmpty();
            gvi.AssemblySemFileVer.Should().NotBeEmpty();
            gvi.FullSemVer.Should().NotBeEmpty();
            gvi.InformationalVersion.Should().NotBeEmpty();
            gvi.BranchName.Should().NotBeEmpty();
            gvi.Sha.Should().NotBeEmpty();
            gvi.ShortSha.Should().NotBeEmpty();
            gvi.NuGetVersionV2.Should().NotBeEmpty();
            gvi.NuGetVersion.Should().NotBeEmpty();
            gvi.NuGetPreReleaseTagV2.Should().NotBeEmpty();
            gvi.NuGetPreReleaseTag.Should().NotBeEmpty();
            gvi.VersionSourceSha.Should().NotBeEmpty();
            gvi.CommitsSinceVersionSource.Should().NotBeEmpty();
            gvi.CommitsSinceVersionSourcePadded.Should().NotBeEmpty();
            gvi.CommitDate.Should().NotBeEmpty();
        }
    }
}
