using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace GnomeWordsmith.UI
{
	class ReforgeUI : UIState
	{
		public static bool visible = false;

		public static Item lastItem = null;
		// Allow up to 15 purchasable items
		public static Item[] purchasableItems = new Item[15];
		public static int purchasableItemsLength = 0;

		public static byte[] accessoryPrefixes = {
			PrefixID.Warding,
			PrefixID.Arcane,
			PrefixID.Lucky,
			PrefixID.Menacing,
			PrefixID.Quick,
			PrefixID.Violent
		};

		public override void OnInitialize()
		{
			// We might want to use vanilla panels here, but for now do nothing.
		}

		private static void AddPurchaseableItemWithPrefix(Item item, byte prefix)
		{
			if (item.prefix == prefix || purchasableItemsLength == 15)
			{
				return;
			}

			Item clone = new Item();
			clone.netDefaults(item.netID);
			clone = clone.CloneWithModdedDataFrom(item);
			clone.Prefix(prefix);

			purchasableItems[purchasableItemsLength] = clone;
			purchasableItemsLength++;
		}

		public static void UpdateCurrentPrefixesForItem(Item item)
		{
			if (item == lastItem)
			{
				return;
			}
			else
			{
				lastItem = item;
			}

			purchasableItemsLength = 0;
			if (item == null)
			{
				return;
			}

			if (item.accessory)
			{
				foreach (byte prefixID in accessoryPrefixes)
				{
					AddPurchaseableItemWithPrefix(item, prefixID);
				}
			}
			else
			{
				// Add Godly or Demonic
				if (item.knockBack > 0)
				{
					AddPurchaseableItemWithPrefix(item, PrefixID.Godly);
				}
				else
				{
					AddPurchaseableItemWithPrefix(item, PrefixID.Demonic);
				}

				if (item.axe > 0 || item.hammer > 0 || item.pick > 0)
				{
					// Tools
					if (!item.channel)
					{
						// Skip drills
						AddPurchaseableItemWithPrefix(item, PrefixID.Light);
						AddPurchaseableItemWithPrefix(item, PrefixID.Massive);
					}
				}
				else if (item.melee)
				{
					if (!item.noMelee)
					{
						// Skip spears
						AddPurchaseableItemWithPrefix(item, PrefixID.Legendary);
					}
				}
				else if (item.ranged)
				{
					// TODO: Investigate weird harpoon restriction
					if (item.knockBack > 0)
					{
						AddPurchaseableItemWithPrefix(item, PrefixID.Unreal);
					}
				}
				else if (item.summon)
				{
					AddPurchaseableItemWithPrefix(item, PrefixID.Ruthless);
				}
				else if (item.magic)
				{
					AddPurchaseableItemWithPrefix(item, PrefixID.Mythical);
				}
			}

			// TODO: Support mod prefixes.
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			GnomeWordsmithPlayer gnomeWordsmithPlayer = Main.LocalPlayer.GetModPlayer<GnomeWordsmithPlayer>(GnomeWordsmith.instance);

			// Make sure the inventory is still open
			if (!Main.playerInventory || Main.player[Main.myPlayer].chest != -1 || Main.npcShop != 0 || Main.player[Main.myPlayer].talkNPC == -1)
			{
				visible = false;

				// Drop item if closed
				if (!gnomeWordsmithPlayer.ReforgeItem.IsAir)
				{
					Main.LocalPlayer.QuickSpawnClonedItem(gnomeWordsmithPlayer.ReforgeItem, gnomeWordsmithPlayer.ReforgeItem.stack);
					gnomeWordsmithPlayer.ReforgeItem.TurnToAir();
				}

				// Make sure our player instance knows we don't have an item
				gnomeWordsmithPlayer.hasItem = false;

				// Remove list of purchasable items
				UpdateCurrentPrefixesForItem(null);

				// Let PlayerCraftingMenu show again
				Main.HidePlayerCraftingMenu = false;

				return;
			}

			Item[] slot = new Item[1];
			slot[0] = gnomeWordsmithPlayer.ReforgeItem;

			// Hide crafting interface
			Main.HidePlayerCraftingMenu = true;

			/**
			 * Create a point for where the mouse is. Used to check whether the
			 * cursor is inside certain regions of the interface.
			 */
			Point mousePoint = new Point(Main.mouseX, Main.mouseY);

			// Calculate Position of ItemSlot
			Main.inventoryScale = 0.85f;
			float xPosition = 50f;
			float yPosition = Main.instance.invBottom + 12f;

			// Pre-calculate slot width and height.
			int slotWidth = (int)(Main.inventoryBackTexture.Width * Main.inventoryScale);
			int slotHeight = (int)(Main.inventoryBackTexture.Height * Main.inventoryScale);

			// Create our "collision" rectangle
			Rectangle slotRectangle = new Rectangle((int)xPosition, (int)yPosition, slotWidth, slotHeight);
			if (slotRectangle.Contains(mousePoint))
			{
				Main.LocalPlayer.mouseInterface = true;
				if (Main.mouseLeftRelease && Main.mouseLeft)
				{
					/**
					 * Check if we should attempt to swap items with the slot.
					 * Item.Prefix(-3) checks if the item can be forged.
					 *
					 * This re-implements part of ItemSlot.LeftClick
					 */
					if (Main.mouseItem.type == 0 || Main.mouseItem.Prefix(-3))
					{
						Utils.Swap(ref slot[0], ref Main.mouseItem);
						if (slot[0].type == 0 || slot[0].stack < 1)
						{
							slot[0] = new Item();
						}

						if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
						{
							Main.mouseItem = new Item();
						}

						if (Main.mouseItem.type > 0 || slot[0].type > 0)
						{
							Main.PlaySound(SoundID.Grab);
						}
					}

					gnomeWordsmithPlayer.hasItem = !slot[0].IsAir;
					UpdateCurrentPrefixesForItem(gnomeWordsmithPlayer.hasItem ? slot[0] : null);
				}
				else
				{
					ItemSlot.MouseHover(slot, ItemSlot.Context.PrefixItem);
				}
			}

			slot[0].newAndShiny = false;
			ItemSlot.Draw(Main.spriteBatch, slot, ItemSlot.Context.PrefixItem, 0, new Vector2(xPosition, yPosition), default(Color));
			gnomeWordsmithPlayer.ReforgeItem = slot[0];

			bool favorited = Main.reforgeItem.favorited;
			int stack = Main.reforgeItem.stack;

			// If there's no purchasable prefixes, stop drawing the interface.
			if (purchasableItemsLength == 0)
			{
				return;
			}

			xPosition += slotWidth + 8;
			string labelText = Language.GetTextValue("Mods.GnomeWordsmith.ReforgeUI.ShopLabel");
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, labelText, new Vector2(xPosition, yPosition), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
			yPosition -= slotHeight / 2;

			// List all purchasable prefixes.
			for (int i = 0; i < purchasableItemsLength; i++)
			{
				yPosition += slotHeight + 8;

				Item item = purchasableItems[i];
				int buyCost = item.value * 2;
				string price = FormatValue(buyCost);

				ItemSlot.Draw(Main.spriteBatch, purchasableItems, ItemSlot.Context.CraftingMaterial, i, new Vector2(xPosition, yPosition), default(Color));
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, price, new Vector2(xPosition + slotWidth + 8, yPosition), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
				purchasableItems[i] = item;

				slotRectangle = new Rectangle((int)xPosition, (int)yPosition, slotWidth, slotHeight);
				if (slotRectangle.Contains(mousePoint))
				{
					Main.LocalPlayer.mouseInterface = true;
					ItemSlot.MouseHover(purchasableItems, ItemSlot.Context.PrefixItem, i);

					if (Main.mouseLeftRelease && Main.mouseLeft && Main.player[Main.myPlayer].CanBuyItem(buyCost, -1))
					{
						Main.mouseLeft = false;
						Main.mouseLeftRelease = false;

						Main.player[Main.myPlayer].BuyItem(buyCost, -1);

						// Show text and play forge sound
						ItemText.NewText(item, item.stack, true, false);
						Main.PlaySound(SoundID.Item37);

						/**
						 * Place purchased item in the reforge slot, but Hide
						 * all purchasable prefixes.
						 */
						gnomeWordsmithPlayer.ReforgeItem = item;
						UpdateCurrentPrefixesForItem(null);
						return;
					}
				}
			}
		}

		// TODO: Move this to somewhere more appropriate?
		public static string FormatValue(int value)
		{
			int copper = value;
			int silver = 0;
			int gold = 0;
			int platinum = 0;

			if (copper >= 100)
			{
				silver = copper / 100;
				copper %= 100;
			}

			if (silver >= 100)
			{
				gold = silver / 100;
				silver %= 100;
			}

			if (gold >= 100)
			{
				platinum = gold / 100;
				gold %= 100;
			}

			string tagString = "";
			if (platinum > 0)
			{
				tagString = tagString + "[c/" + Colors.AlphaDarken(Colors.CoinPlatinum).Hex3() + ":" + platinum + " " + Language.GetTextValue("LegacyInterface.15") + "] ";
			}

			if (gold > 0)
			{
				tagString = tagString + "[c/" + Colors.AlphaDarken(Colors.CoinGold).Hex3() + ":" + gold + " " + Language.GetTextValue("LegacyInterface.16") + "] ";
			}

			if (silver > 0)
			{
				tagString = tagString + "[c/" + Colors.AlphaDarken(Colors.CoinSilver).Hex3() + ":" + silver + " " + Language.GetTextValue("LegacyInterface.17") + "] ";
			}

			if (copper > 0)
			{
				tagString = tagString + "[c/" + Colors.AlphaDarken(Colors.CoinCopper).Hex3() + ":" + copper + " " + Language.GetTextValue("LegacyInterface.18") + "] ";
			}

			return tagString;
		}
	}
}
