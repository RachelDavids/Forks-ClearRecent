using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.CodeContainerManagement;

namespace ClearRecent.Services
{
	internal class StartPageRecents
	{
		private readonly IServiceProvider _serviceProvider;

		internal StartPageRecents(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		internal bool ProjectsFound()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return GetRecents(GetManager()).Count > 0;
		}

		internal void ClearAllProjects()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Clear(_ => true);
		}

		internal void ClearMissingProjects()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Clear(Files.Missing);
		}

		private void Clear(Func<string, bool> shouldDelete)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			ISettingsManager manager = GetManager();
			IList<string> recents = GetRecents(manager);

			if (recents.Count == 0)
			{
				return;
			}

			CodeContainerRegistry registry = GetRegistry(manager);

			foreach (string path in recents)
			{
				if (shouldDelete(path))
				{
					_ = registry.RemoveAsync(path);
				}
			}
		}

		private ISettingsManager GetManager()
		{
			return _serviceProvider.GetService(typeof(SVsSettingsPersistenceManager)) as ISettingsManager;
		}

		private static IList<string> GetRecents(ISettingsManager manager)
		{
			return manager.GetOrCreateList("CodeContainers.Offline",
										   isMachineLocal: true)
						  .Keys
						  .ToList();
		}

		private static CodeContainerRegistry GetRegistry(ISettingsManager manager)
		{
			return new CodeContainerRegistry(manager);
		}

		[Guid("9b164e40-c3a2-4363-9bc5-eb4039def653")]
		private class SVsSettingsPersistenceManager
		{
		}
	}
}
