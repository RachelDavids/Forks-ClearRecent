using System.IO;

namespace ClearRecent.Services
{
	internal static class Files
	{
		internal static bool Missing(string path) =>
			!File.Exists(path) && !Directory.Exists(path);
	}
}
