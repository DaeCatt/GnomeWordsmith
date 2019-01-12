using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GnomeWordsmith.Items {
	class GnomeWordsmithGlobalItem : GlobalItem {
		public GnomeWordsmithGlobalItem() { }

		/**
		 * Handle Portable Wormhole teleportations. This could be done on the
		 * Portable Wormhole item directly, but we might want to introduce
		 * other items that can teleport (eg Cellphone upgrade).
		 */
		public override void UpdateInventory(Item item, Player player) {
			if (!Main.mapFullscreen || item.type != mod.ItemType("PortableWormhole") || !Main.player[Main.myPlayer].HasItem(mod.ItemType("PortableWormhole")))
				return;

			if (!Main.mouseLeft || !Main.mouseLeftRelease)
				return;

			float mapWorldScale = Main.mapFullscreenScale / 16f;
			float offsetX = Main.screenWidth / 2 - Main.mapFullscreenPos.X * Main.mapFullscreenScale;
			float offsetY = Main.screenHeight / 2 - Main.mapFullscreenPos.Y * Main.mapFullscreenScale;

			/**
			 * Only check teleports to other players if we're only, we're on a
			 * team and we're not set to hostile.
			 */
			if (Main.netMode == 1 && Main.player[Main.myPlayer].team > 0 && !Main.player[Main.myPlayer].hostile) {
				for (int i = 0; i < Main.player.Length; i++) {
					/**
					 * Ignore players that are:
					 * - Yourself.
					 * - "Inactive".
					 * - Dead.
					 * - On another team.
					 * - Hostile.
					 */
					if (i == Main.myPlayer || !Main.player[i].active || Main.player[i].dead || Main.player[Main.myPlayer].team != Main.player[i].team || Main.player[i].hostile) {
						continue;
					}

					float playerHeadCenterX = offsetX + mapWorldScale * (Main.player[i].position.X + Main.player[i].width / 2);
					float playerHeadCenterY = offsetY + mapWorldScale * (Main.player[i].position.Y + Main.player[i].gfxOffY + Main.player[i].height / 2);
					playerHeadCenterX -= 2f;
					playerHeadCenterY -= 2f - Main.mapFullscreenScale / 5f * 2f;

					float minX = playerHeadCenterX - 14f * Main.UIScale;
					float minY = playerHeadCenterY - 14f * Main.UIScale;
					float maxX = minX + 28f * Main.UIScale;
					float maxY = minY + 28f * Main.UIScale;

					if (Main.mouseX >= minX && Main.mouseX <= maxX && Main.mouseY >= minY && Main.mouseY <= maxY) {
						Main.mouseLeftRelease = false;
						Main.mapFullscreen = false;
						Main.player[Main.myPlayer].UnityTeleport(Main.player[i].position);
						return;
					}
				}
			}

			for (int i = 0; i < Main.npc.Length; i++) {
				// Only check active NPCs that are set to townNPC.
				if (!Main.npc[i].active || !Main.npc[i].townNPC) {
					continue;
				}

				int headIndex = NPC.TypeToHeadIndex(Main.npc[i].type);
				if (headIndex <= 0) {
					continue;
				}

				float npcHeadCenterX = offsetX + mapWorldScale * (Main.npc[i].position.X + Main.npc[i].width / 2);
				float npcHeadCenterY = offsetY + mapWorldScale * (Main.npc[i].position.Y + Main.npc[i].gfxOffY + Main.npc[i].height / 2);

				float minX = npcHeadCenterX - Main.npcHeadTexture[headIndex].Width / 2 * Main.UIScale;
				float minY = npcHeadCenterY - Main.npcHeadTexture[headIndex].Height / 2 * Main.UIScale;
				float maxX = minX + Main.npcHeadTexture[headIndex].Width * Main.UIScale;
				float maxY = minY + Main.npcHeadTexture[headIndex].Height * Main.UIScale;

				if (Main.mouseX >= minX && Main.mouseX <= maxX && Main.mouseY >= minY && Main.mouseY <= maxY) {
					Main.mouseLeftRelease = false;
					Main.mapFullscreen = false;

					Main.NewText(Language.GetTextValue("Game.HasTeleportedTo", Main.player[Main.myPlayer].name, Main.npc[i].FullName), 255, 255, 0);
					Main.player[Main.myPlayer].Teleport(Main.npc[i].position);
					return;
				}
			}
		}
	}
}
