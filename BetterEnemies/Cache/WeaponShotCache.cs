using UnityEngine;

namespace BetterEnemies.Cache
{
	internal static class WeaponShotCache
	{
		public static weaponscript LastWeapon;
		public static float LastShotTime;
		public const float CacheTTL = 0.05f;

		public static bool IsValid =>
			LastWeapon != null && Time.time - LastShotTime <= CacheTTL;
	}
}
