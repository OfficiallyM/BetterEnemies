using BetterEnemies.Core;
using BetterEnemies.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterEnemies.Components
{
	[DisallowMultipleComponent]
	public class DamageZoneHandler : MonoBehaviour
	{
		public static event Action<breakchilds, Enums.Bone?> OnHit;

		public breakablescript breakable;

		private const float HeadModifier = 1f;
		private const float ChestModifier = 0.5f;
		private const float ArmModifier = 0.25f;
		private const float LegModifier = 0.25f;

		private Dictionary<breakchilds, Enums.Bone> _boneMap = new Dictionary<breakchilds, Enums.Bone>();

		public void Start()
		{
			var ai = gameObject.GetComponent<newAiScript>();
			breakable = ai?.breakable;
			if (breakable == null)
			{
				Destroy(this);
				return;
			}

			var skeleton = transform.Find("munkas01/Default simplified/root");
			foreach (var bone in skeleton.GetComponentsInChildren<Collider>())
			{
				string boneName = bone.name.ToLowerInvariant();
				var boneObject = bone.gameObject;

				if (boneName.Contains("sens") || boneName.Contains("stimul"))
					continue;

				var breakChild = boneObject.GetComponent<breakchilds>() ?? boneObject.AddComponent<breakchilds>();
				breakChild.P = breakable;
				if (IsHeadBone(boneName))
				{
					_boneMap[breakChild] = Enums.Bone.Head;
					breakChild.modifier = HeadModifier;
				}
				else if (IsChestBone(boneName))
				{
					_boneMap[breakChild] = Enums.Bone.Chest;
					breakChild.modifier = ChestModifier;
				}
				else if (IsArmBone(boneName))
				{
					_boneMap[breakChild] = Enums.Bone.Arm;
					breakChild.modifier = ArmModifier;
				}
				else if (IsLegBone(boneName))
				{
					_boneMap[breakChild] = Enums.Bone.Leg;
					breakChild.modifier = LegModifier;
				}
			}
		}

		public Enums.Bone? GetHitBone(breakchilds hit)
		{
			return _boneMap.TryGetValue(hit, out var bone) ? bone : (Enums.Bone?)null;
		}

		internal void RaiseOnHit(breakchilds hit)
			=> OnHit?.Invoke(hit, GetHitBone(hit));

		private static bool IsHeadBone(string name) =>
			name.Contains("head");

		private static bool IsChestBone(string name) =>
			name.Contains("spine");

		private static bool IsArmBone(string name) =>
			name.Contains("arm");

		private static bool IsLegBone(string name) =>
			name.Contains("leg");
	}
}
