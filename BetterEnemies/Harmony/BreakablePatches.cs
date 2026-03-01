using BetterEnemies.Cache;
using BetterEnemies.Utilities;
using HarmonyLib;
using UnityEngine;

namespace BetterEnemies.Harmony
{
	[HarmonyPatch(typeof(breakablescript), nameof(breakablescript.TryBreakShoot))]
	public static class Patch_breakablescript_TryBreakShoot
	{
		public static void Prefix(breakablescript __instance, ref float force)
		{
			// Only care about breakablescripts attached to munkas.
			newAiScript munkas = __instance.GetComponentInParent<newAiScript>();
			if (munkas == null)
				return;

			// If cache is stale, let normal damage apply.
			if (!WeaponShotCache.IsValid)
				return;

			RaycastHit? hitRaycast = null;
			RaycastHit[] hits = Traverse.Create(WeaponShotCache.LastWeapon).Field<RaycastHit[]>("hit").Value;
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit? hit = hits[i];
				if (hit?.collider == null)
					break;

				breakablescript bs = hit?.collider.GetComponentInParent<breakablescript>();
				if (bs == __instance)
				{
					hitRaycast = hit;
					break;
				}
			}

			if (hitRaycast == null)
				return;

			Logging.LogDebug($"[PRE] Force: {force}");
			float modifier = GetZoneModifier(hitRaycast.Value, munkas);
			force *= modifier;
			Logging.LogDebug($"[POST] Force: {force} (Modifier: {modifier}) - ShootHealth: {munkas.breakable.shootHealth}");
		}

		private static float GetZoneModifier(RaycastHit hit, newAiScript munkas)
		{
			// Get all colliders in a small radius at the hit point.
			Collider[] nearby = Physics.OverlapSphere(hit.point, 0.5f);

			Collider bestBone = null;
			float bestDist = float.MaxValue;

			foreach (Collider c in nearby)
			{
				string boneName = c.name.ToLower();
				// Must be a valid bone.
				if (boneName.Contains("bcollider") || boneName.Contains("sens") || boneName.Contains("stimul")) continue;
				if (!c.transform.IsChildOf(munkas.transform)) continue;

				float dist = Vector3.Distance(c.transform.position, hit.point);
				if (dist < bestDist)
				{
					bestDist = dist;
					bestBone = c;
				}
			}

			if (bestBone != null)
			{
				Logging.LogDebug($"Hit bone: {bestBone.name}");
				if (IsHeadBone(bestBone.name.ToLower()))
					return 1f;

				if (IsLimbBone(bestBone.name.ToLower()))
					return 0.25f;
			}

			// Chest shot.
			return 0.5f;
		}

		private static bool IsHeadBone(string name) =>
			name.Contains("head");

		private static bool IsLimbBone(string name) =>
			name.Contains("leg") ||
			name.Contains("arm");
	}
}
