using Microsoft.VisualStudio.Shell;

namespace ClearRecent.Commands
{
	internal sealed class ClearMissingRecentFiles : Command
	{
		internal ClearMissingRecentFiles(Package package)
			: base(package,
				   0x0101,
				   "Remove Recent Files not found on disk from File menu?")
		{ }

		protected override bool Enabled()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return fileMenuRecents.FilesFound();
		}

		protected override void Execute()
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			fileMenuRecents.ClearMissingFiles();
		}
	}
}
