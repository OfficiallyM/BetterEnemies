using BetterEnemies.Components;
using TLDLoader;

namespace BetterEnemies
{
	public class BetterEnemies : Mod
	{
		private string _version = "0.2.0";
		public override string ID => "M_BetterEnemies";
		public override string Name => "Better Enemies";
		public override string Author => "M-";
		public override string Version => _version;
		public override bool LoadInDB => true;
		public override bool UseLogger => true;
		public override bool UseHarmony => true;

		internal static BetterEnemies I;

		public BetterEnemies()
		{
			I = this;

#if DEBUG
			_version += "-DEV";
#endif
		}

		public override void DbLoad()
		{
			var munkas = itemdatabase.d.gmunkas01;
			if (munkas.GetComponent<CharacteristicsHandler>() == null)
				munkas.AddComponent<CharacteristicsHandler>();

			if (munkas.GetComponent<DamageZoneHandler>() == null)
				munkas.AddComponent<DamageZoneHandler>();

			if (munkas.GetComponent<InjuryHandler>() == null)
				munkas.AddComponent<InjuryHandler>();
		}
	}
}
