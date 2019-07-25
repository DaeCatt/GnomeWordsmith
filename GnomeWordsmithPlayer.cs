using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace GnomeWordsmith
{
	internal class GnomeWordsmithPlayer : ModPlayer
	{
		public Item ReforgeItem;
		public bool hasItem = false;

		/**
		 * Keep track of the active reforging item on the player, to avoid
		 * having the item disappear under some conditions.
		 */
		public override void Initialize()
		{
			ReforgeItem = new Item();
			ReforgeItem.SetDefaults(0, true);
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{ "HasItem", hasItem },
				{ "ReforgeItem", ReforgeItem }
			};
		}

		public override void Load(TagCompound tag)
		{
			/**
			 * HasItem ensures that we don't accidentally create nonsense items
			 * out of thin air.
			 */
			bool hasItem = tag.GetBool("HasItem");
			Item loadedItem = tag.Get<Item>("ReforgeItem");
			if (hasItem)
			{
				ReforgeItem = loadedItem;
			}
		}
	}
}
