using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuKeeper.Configuration;
using NuKeeper.Creators;
using NuKeeper.Inspection.Files;
using NuKeeper.Inspection.Logging;
using NuKeeper.Inspection.Report;
using NuKeeper.Inspection.RepositoryInspection;
using NuKeeper.Inspection.Sources;
using NuKeeper.Update;
using NuKeeper.Update.Process;
using NuKeeper.Update.Selection;

namespace NuKeeper.Local
{
    public class LocalUpdater : ILocalUpdater
    {
        private readonly IUpdateSelection _selection;
        private readonly IUpdateRunner _updateRunner;
        private readonly SolutionsRestore _solutionsRestore;
        private readonly INuKeeperLogger _logger;

        public LocalUpdater(
            IUpdateSelection selection,
            IUpdateRunner updateRunner,
            SolutionsRestore solutionsRestore,
            INuKeeperLogger logger)
        {
            _selection = selection;
            _updateRunner = updateRunner;
            _solutionsRestore = solutionsRestore;
            _logger = logger;
        }

        public async Task ApplyUpdates(
            IReadOnlyCollection<PackageUpdateSet> updates,
            IFolder workingFolder,
            NuGetSources sources,

            SettingsContainer settings)
        {
            if (!updates.Any())
            {
                return;
            }

            var filtered = await _selection
                .Filter(updates, settings.PackageFilters, p => Task.FromResult(true));

            if (!filtered.Any())
            {
                _logger.Detailed("All updates were filtered out");
                return;
            }

            await ApplyUpdates(filtered, workingFolder, sources);
        }

        private async Task ApplyUpdates(IReadOnlyCollection<PackageUpdateSet> updates, IFolder workingFolder, NuGetSources sources)
        {
            await _solutionsRestore.CheckRestore(updates, workingFolder, sources);

            foreach (var update in updates)
            {
                var reporter = new ConsoleReporter();
                _logger.Minimal("Updating " + reporter.Describe(update));

                await _updateRunner.Update(update, sources);
            }
        }
    }
}
