using BetterEnemies.Cache;
using HarmonyLib;
using UnityEngine;

namespace BetterEnemies.Harmony
{
	[HarmonyPatch(typeof(weaponscript), "Shot")]
	public static class WeaponShotPrefix
	{
		[HarmonyPrefix]
		public static void Prefix(weaponscript __instance)
		{
			// Workaround to using a transpiler patch.
			// Works by caching the shot, then picking up from the cache during
			// breakablescript.TryBreakShoot(). Cache has a very short TTL to avoid
			// it bleeding between shots.
			WeaponShotCache.LastWeapon = __instance;
			WeaponShotCache.LastShotTime = Time.time;
		}
	}
}
