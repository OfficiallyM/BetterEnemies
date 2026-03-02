using BetterEnemies.Components;
using BetterEnemies.Utilities;
using HarmonyLib;
using UnityEngine;

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

	// Improve killing munkas with vehicles.
	[HarmonyPatch(typeof(breakablescript), "OnCollisionEnter")]
	public static class Patch_breakablescript_OnCollisionEnter
	{
		public static bool Prefix(breakablescript __instance, Collision c)
		{
			if (__instance.newAI == null) return true;

			var massScript = c.transform.root?.GetComponent<massScript>();
			if (massScript == null) return true;

			float mass = massScript.Mass();
			float velocity = c.relativeVelocity.magnitude;
			// Don't include mass in calculation for light objects.
			if (mass < 200f) return true;
			if (velocity < 5f) return true;

			float force = velocity * mass * 0.25f;

			if (c.contactCount > 0)
				Traverse.Create(__instance).Method("DamageFrom", c.GetContact(0).point - __instance.transform.position).GetValue();
			__instance.TryBreak(force);

			// Skip original.
			return false;
		}
	}
}
