using BetterEnemies.Core;
using BetterEnemies.Utilities;
using UnityEngine;

namespace BetterEnemies.Components
{
	public class InjuryHandler : MonoBehaviour
	{
		public newAiScript ai;
		public DamageZoneHandler damageHandler;

		private float _baseArmDamage;
		private float _baseMaxSpeed;

		private float _armInjury = 0f;
		private float _legInjury = 0f;

		private const float MaxInjury = 100f;
		private const float ForceScale = 0.1f;
		private const float ArmModifierMin = 0.25f;
		private const float LegModifierMin = 0.25f;

		public void Start()
		{
			ai = gameObject.GetComponent<newAiScript>();
			damageHandler = gameObject.GetComponent<DamageZoneHandler>();

			if (ai == null || damageHandler == null)
			{
				Destroy(this);
				return;
			}

			_baseArmDamage = ai.damage;
			_baseMaxSpeed = ai.maxSpeed;

			damageHandler.OnHit += OnHit;
		}

		public void OnDestroy()
		{
			damageHandler.OnHit -= OnHit;
		}

		private void OnHit(breakchilds breakchild, Enums.Bone? bone, float force)
		{
			if (breakchild == null || bone == null) return;

			force *= ForceScale;

			switch (bone)
			{
				case Enums.Bone.Arm:
					_armInjury = Mathf.Min(_armInjury + force, MaxInjury);
					ai.damage = Mathf.Lerp(_baseArmDamage, _baseArmDamage * ArmModifierMin, _armInjury / MaxInjury);
					break;
				case Enums.Bone.Leg:
					_legInjury = Mathf.Min(_legInjury + force, MaxInjury);
					ai.maxSpeed = Mathf.Lerp(_baseMaxSpeed, _baseMaxSpeed * LegModifierMin, _legInjury / MaxInjury);
					break;
			}

			Logging.LogDebug($"[InjuryHandler] Bone: {bone} - Force {force}\nBase damage: {_baseArmDamage} - Arm injury: {_armInjury}/{MaxInjury}\nBase speed: {_baseMaxSpeed} - Leg injury: {_legInjury}/{MaxInjury}");
		}
	}
}
