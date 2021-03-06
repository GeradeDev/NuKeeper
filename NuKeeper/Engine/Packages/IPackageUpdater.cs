using System.Threading.Tasks;
using NuKeeper.Configuration;
using NuKeeper.Git;
using NuKeeper.Inspection.RepositoryInspection;
using NuKeeper.Inspection.Sources;

namespace NuKeeper.Engine.Packages
{
    public interface IPackageUpdater
    {
        Task<bool> MakeUpdatePullRequest(
            IGitDriver git,
            RepositoryData repository,
            PackageUpdateSet updateSet,
            NuGetSources sources,
            SourceControlServerSettings serverSettings);
    }
}
