using System;
using System.Reflection;

namespace ClearRecent.Services
{
	internal static class Types
	{
		internal static PropertyInfo GetCountProp(Kind kind) =>
			CreateMruListType(kind).GetProperty("Count");

		internal static PropertyInfo GetItemsProp(Kind kind) =>
			CreateMruListType(kind).GetProperty("Items");

		internal static MethodInfo GetRemoveItemAtMethod(Kind kind) =>
			CreateMruListType(kind).GetMethod("RemoveItemAt");

		internal static PropertyInfo GetPathProp() =>
			CreateType("FileSystemMruItem").GetProperty("Path");

		private static Type CreateMruListType(Kind kind)
		{
			return CreateType($"{kind}MruList");
		}

		private static Type CreateType(string name)
		{
			const string Namespace = "Microsoft.VisualStudio.PlatformUI";
			const string Assembly = "Microsoft.VisualStudio.Shell.UI.Internal";

			return Type.GetType($"{Namespace}.{name}, {Assembly}",
								throwOnError: true);
		}
	}
}
