using BetterEnemies.Core;
using BetterEnemies.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterEnemies.Components
{
	public class DamageZoneHandler : MonoBehaviour
	{
		public static event Action<breakchilds, Enums.Bone?> OnHit;

		public breakablescript breakable;
		private List<breakchilds> _headBones = new List<breakchilds>();
		private List<breakchilds> _chestBones = new List<breakchilds>();
		private List<breakchilds> _armBones = new List<breakchilds>();
		private List<breakchilds> _legBones = new List<breakchilds>();

		private const float HeadModifier = 1f;
		private const float ChestModifier = 0.5f;
		private const float ArmModifier = 0.25f;
		private const float LegModifier = 0.25f;

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
					_headBones.Add(breakChild);
					breakChild.modifier = HeadModifier;
				}
				else if (IsChestBone(boneName))
				{
					_chestBones.Add(breakChild);
					breakChild.modifier = ChestModifier;
				}
				else if (IsArmBone(boneName))
				{
					_armBones.Add(breakChild);
					breakChild.modifier = ArmModifier;
				}
				else if (IsLegBone(boneName))
				{
					_legBones.Add(breakChild);
					breakChild.modifier = LegModifier;
				}
			}
		}

		public Enums.Bone? GetHitBone(breakchilds hit)
		{
			foreach (var bone in _headBones)
				if (bone == hit)
					return Enums.Bone.Head;

			foreach (var bone in _chestBones)
				if (bone == hit)
					return Enums.Bone.Chest;

			foreach (var bone in _armBones)
				if (bone == hit)
					return Enums.Bone.Arm;

			foreach (var bone in _legBones)
				if (bone == hit)
					return Enums.Bone.Leg;

			return null;
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
