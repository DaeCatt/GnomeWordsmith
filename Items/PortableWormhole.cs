using Terraria;
using Terraria.ModLoader;

namespace GnomeWordsmith.Items {
	public class PortableWormhole : ModItem {
		public override void SetStaticDefaults() { }

		// Teleport effect is handled in GnomeWordsmithGlobalItem.cs
		public override void SetDefaults() {
			item.CloneDefaults(2997);
			item.maxStack = 1;
			item.consumable = false;
			item.value = Item.buyPrice(4, 19, 0, 0);
			item.width = 14;
			item.height = 14;
			item.useStyle = 4; // Like a life crystal
			item.rare = 5; // Pink, Pre-Plantera items
		}
	}
}
