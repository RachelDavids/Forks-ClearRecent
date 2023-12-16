using System;
using System.Collections;
using System.Reflection;

using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ClearRecent.Services
{
	internal sealed class FileMenuRecents
	{
		private readonly IServiceProvider _serviceProvider;

		internal FileMenuRecents(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		internal bool FilesFound()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return Found(Kind.File);
		}

		internal bool ProjectsFound()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return Found(Kind.Project);
		}

		internal void ClearAllFiles()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Clear(Kind.File, _ => true);
		}

		internal void ClearAllProjects()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Clear(Kind.Project, _ => true);
		}

		internal void ClearMissingFiles()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Clear(Kind.File, Files.Missing);
		}

		internal void ClearMissingProjects()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			Clear(Kind.Project, Files.Missing);
		}

		private bool Found(Kind kind)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return GetCount(GetDataSource(kind), kind) > 0;
		}

		private void Clear(Kind kind, Func<string, bool> shouldDelete)
		{
			Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
			IVsUIDataSource dataSource = GetDataSource(kind);
			IList recents = GetRecents(dataSource, kind);

			if (recents.Count == 0)
			{
				return;
			}

			MethodInfo remove = Types.GetRemoveItemAtMethod(kind);

			for (int i = recents.Count - 1; i > -1; i--)
			{
				if (shouldDelete(GetPath(recents[i])))
				{
					remove.Invoke(dataSource, new object[] { i });
				}
			}
		}

		private IVsUIDataSource GetDataSource(Kind kind)
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			IVsDataSourceFactory factory = _serviceProvider.GetService(typeof(SVsDataSourceFactory)) as IVsDataSourceFactory;
			Assumes.Present(factory);
			factory.GetDataSource(new Guid("9099ad98-3136-4aca-a9ac-7eeeaee51dca"),
								  (uint)kind,
								  out IVsUIDataSource dataSource);

			return dataSource;
		}

		private static int GetCount(IVsUIDataSource dataSource, Kind kind)
		{
			return (int)Types.GetCountProp(kind)
							 .GetValue(dataSource, index: null);
		}

		private static IList GetRecents(IVsUIDataSource dataSource, Kind kind)
		{
			return Types.GetItemsProp(kind)
						.GetValue(dataSource, index: null) as IList;
		}

		private static string GetPath(object recent)
		{
			return Types.GetPathProp()
						.GetValue(recent, index: null) as string;
		}
	}
}
