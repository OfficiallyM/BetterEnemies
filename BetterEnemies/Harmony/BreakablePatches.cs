using BetterEnemies.Components;
using BetterEnemies.Utilities;
using HarmonyLib;

namespace BetterEnemies.Harmony
{
	[HarmonyPatch(typeof(breakchilds), nameof(breakchilds.TryBreakShoot))]
	public static class Patch_breakchilds_TryBreakShoot
	{
		public static void Prefix(breakchilds __instance)
		{
			var zoneHandler = __instance.transform.root?.GetComponent<DamageZoneHandler>();
			if (zoneHandler == null) return;
			Logging.LogDebug($"[{__instance.name}] Hit {(zoneHandler.GetHitBone(__instance)?.ToString() ?? "Unknown")} - Modifier: {__instance.modifier}");
			zoneHandler.RaiseOnHit(__instance);
		}
	}
}
