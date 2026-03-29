using BetterEnemies.Utilities;
using System.Linq;
using UnityEngine;
using static BetterEnemies.Core.Enums;

namespace BetterEnemies.Components
{
	[DisallowMultipleComponent]
	public class CharacteristicsHandler : MonoBehaviour
	{
		public TraitLevel Speed { get; private set; }
		public TraitLevel Strength { get; private set; }
		public TraitLevel Toughness { get; private set; }

		public tosaveitemscript save;
		public newAiScript ai;
		public breakablescript breakable;

		private const float BaseHealthModifier = 1.5f;

		private static readonly Color SkinNormal = new Color(0.6f, 0.75f, 0.85f);
		private static readonly Color SkinEnhanced = new Color(0.5f, 0.55f, 0.75f);

		private static readonly Color ClothingNormal = new Color(0.65f, 0.75f, 0.65f);
		private static readonly Color ClothingEnhanced = new Color(0.45f, 0.55f, 0.45f);

		private static readonly Vector3 ScaleNormal = new Vector3(1.1f, 1.1f, 1.1f);
		private static readonly Vector3 ScaleEnhanced = new Vector3(1.2f, 1.2f, 1.2f);

		public void Start()
		{
			save = gameObject.GetComponent<tosaveitemscript>();
			ai = gameObject.GetComponent<newAiScript>();
			breakable = ai?.breakable;

			if (save == null || ai == null || breakable == null)
			{
				Destroy(this);
				return;
			}

			System.Random rng = new System.Random(save.idInSave);
			Speed = RollTrait(rng);
			Strength = RollTrait(rng);
			Toughness = RollTrait(rng);

			ApplyCharacteristics();
		}

		private TraitLevel RollTrait(System.Random rng)
		{
			double roll = rng.NextDouble();
			if (roll < 0.10) return TraitLevel.Enhanced;
			if (roll < 0.40) return TraitLevel.Normal;
			return TraitLevel.None;
		}

		private void ApplyCharacteristics()
		{
			Logging.LogDebug($"[CharacteristicsHandler] Characteristics:\nSpeed: {Speed}\nStrength: {Strength}\nToughness: {Toughness}");

			ApplyPhysical();
			ApplyVisual();
		}

		private void ApplyPhysical()
		{
			Logging.LogDebug($"[PRE] Stats:\nHealth: {ai.breakable.health:F2}\nShooth health: {ai.breakable.shootHealth}\nMax speed: {ai.maxSpeed}\nWalk dist: {ai.walkDist}\nRun dist: {ai.runDist}\nDamage: {ai.damage}");

			if (Speed != TraitLevel.None)
			{
				ai.maxSpeed *= Speed == TraitLevel.Enhanced ? 1.8f : 1.4f;
				ai.walkDist = 2f;
				ai.runDist = 5f;
			}

			if (Strength != TraitLevel.None)
				ai.damage *= Strength == TraitLevel.Enhanced ? 2.5f : 1.75f;

			// Don't apply health multiplier on save load.
			if (!save.loaded)
			{
				if (Toughness != TraitLevel.None)
				{
					float modifier = Toughness == TraitLevel.Enhanced ? 8f : 4f;
					ai.breakable.health *= modifier;
					ai.breakable.shootHealth *= modifier;
				}
				else
				{
					ai.breakable.health *= BaseHealthModifier;
					ai.breakable.shootHealth *= BaseHealthModifier;
				}
			}

			Logging.LogDebug($"[POST] Stats:\nHealth: {ai.breakable.health:F2}\nShooth health: {ai.breakable.shootHealth}\nMax speed: {ai.maxSpeed}\nWalk dist: {ai.walkDist}\nRun dist: {ai.runDist}\nDamage: {ai.damage}");
		}

		private void ApplyVisual()
		{
			Transform meshRoot = transform.Find("munkas01");
			if (meshRoot == null) return;

			SkinnedMeshRenderer skin = meshRoot.GetComponentsInChildren<SkinnedMeshRenderer>()
				.FirstOrDefault(r => r.name == "munkas01_20221025cMesh");
			SkinnedMeshRenderer clothing = meshRoot.GetComponentsInChildren<SkinnedMeshRenderer>()
				.FirstOrDefault(r => r.name == "male_worksuit01Mesh");

			MaterialPropertyBlock block = new MaterialPropertyBlock();

			if (Speed != TraitLevel.None && skin != null)
			{
				skin.GetPropertyBlock(block);
				block.SetColor("_Color", Speed == TraitLevel.Enhanced ? SkinEnhanced : SkinNormal);
				skin.SetPropertyBlock(block);
			}

			if (Strength != TraitLevel.None)
				transform.localScale = Strength == TraitLevel.Enhanced ? ScaleEnhanced : ScaleNormal;

			if (Toughness != TraitLevel.None && clothing != null)
			{
				clothing.GetPropertyBlock(block);
				block.SetColor("_Color", Toughness == TraitLevel.Enhanced ? ClothingEnhanced : ClothingNormal);
				clothing.SetPropertyBlock(block);
			}
		}
	}
}
