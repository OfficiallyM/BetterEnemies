using TLDLoader;

namespace BetterEnemies
{
	public class BetterEnemies : Mod
	{
		public override string ID => "M_BetterEnemies";
		public override string Name => "Better Enemies";
		public override string Author => "M-";
		public override string Version => "0.0.1";
		public override bool UseLogger => true;
		public override bool UseHarmony => true;

		internal static BetterEnemies I;

		public BetterEnemies()
		{
			I = this;
		}
	}
}
