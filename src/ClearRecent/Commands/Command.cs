using System;
using System.ComponentModel.Design;

using ClearRecent.Services;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ClearRecent.Commands
{
	internal abstract class Command
	{
		private readonly IServiceProvider _serviceProvider;

		protected readonly FileMenuRecents fileMenuRecents;
		protected readonly StartPageRecents startPageRecents;

		private readonly string _message;

		protected Command(Package package,
						  int commandId,
						  string message)
		{
			_serviceProvider = package ?? throw new ArgumentNullException(nameof(package));

			fileMenuRecents = new FileMenuRecents(_serviceProvider);
			startPageRecents = new StartPageRecents(_serviceProvider);

			_message = message;

			if (_serviceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
			{
				commandService.AddCommand(new OleMenuCommand(Invoke,
															 null,
															 Prepare,
															 new CommandID(Guids.MenuGroup,
																		   commandId)));
			}
		}

		private void Prepare(object sender, EventArgs e) =>
			((OleMenuCommand)sender).Enabled = Enabled();

		private void Invoke(object sender, EventArgs e)
		{
			if (VsShellUtilities.ShowMessageBox(
					_serviceProvider,
					_message,
					null,
					OLEMSGICON.OLEMSGICON_QUERY,
					OLEMSGBUTTON.OLEMSGBUTTON_YESNO,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST) == 6)
			{
				Execute();
			}
		}

		protected abstract bool Enabled();

		protected abstract void Execute();
	}
}
