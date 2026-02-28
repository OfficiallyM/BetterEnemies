namespace BetterEnemies.Utilities
{
	internal static class Logging
	{
		public static void Log(string message, TLDLoader.Logger.LogLevel logLevel = TLDLoader.Logger.LogLevel.Info) =>
			BetterEnemies.I.Logger.Log(message, logLevel);

		public static void LogDebug(string message)
		{
#if DEBUG
			BetterEnemies.I.Logger.LogDebug(message);
#endif
		}

		public static void LogInfo(string message) =>
			BetterEnemies.I.Logger.LogInfo(message);

		public static void LogWarning(string message) =>
			BetterEnemies.I.Logger.LogWarning(message);

		public static void LogError(string message) =>
			BetterEnemies.I.Logger.LogError(message);

		public static void LogCritical(string message) =>
			BetterEnemies.I.Logger.LogCritical(message);
	}
}
