using Terraria;
using Terraria.ModLoader;

namespace GnomeWordsmith.Items {
	class GnomeWordsmithGlobalItem : GlobalItem {
		public GnomeWordsmithGlobalItem() {
		}

		/**
		 * Handle Portable Wormhole teleportations. This could be done on the
		 * Portable Wormhole item directly, but we might want to introduce
		 * other items that can teleport (eg Cellphone upgrade).
		 */
		public override void UpdateInventory(Item item, Player player) {
			if (
				!Main.mapFullscreen ||
				!Main.mouseLeft ||
				!Main.mouseLeftRelease ||
				!Main.player[Main.myPlayer].HasItem(mod.ItemType("PortableWormhole"))
			) {
				return;
			}

			// Translate fullscreen map position into world coordinates
			float mapScale = 16f / Main.mapFullscreenScale;
			float mapCenterX = Main.mapFullscreenPos.X * 16f - 10f;
			float mapCenterY = Main.mapFullscreenPos.Y * 16f - 21f;

			// Translate mouse position relative to the center of the screen
			float mouseDeltaX = Main.mouseX - Main.screenWidth / 2;
			float mouseDeltaY = Main.mouseY - Main.screenHeight / 2;

			/**
			 * Translate cursor position on the fullscreen map into cursor
			 * position on the entire map.
			 */
			float cursorOnMapX = mapCenterX + mouseDeltaX * mapScale;
			float cursorOnMapY = mapCenterY + mouseDeltaY * mapScale;

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

					// TODO: use player head dimensions instead of 16x16 rect.
					float minX = Main.player[i].position.X - 8f * mapScale;
					float minY = Main.player[i].position.Y - 8f * mapScale;
					float maxX = Main.player[i].position.X + 8f * mapScale;
					float maxY = Main.player[i].position.Y + 8f * mapScale;

					if (cursorOnMapX >= (double) minX &&
						cursorOnMapX <= (double) maxX &&
						cursorOnMapY >= (double) minY &&
						cursorOnMapY <= (double) maxY) {
						Main.mouseLeftRelease = false;
						Main.mapFullscreen = false;
						Main.player[Main.myPlayer].UnityTeleport(Main.player[i].position);

						return;
					}
				}
			}

			for (int i = 0; i < Main.npc.Length; i++) {
				// Only check NPCs that are set to townNPC.
				if (!Main.npc[i].townNPC) {
					continue;
				}

				// TODO: use NPC head dimensions instead of 16x16 rect.
				float minX = Main.npc[i].position.X - 8f * mapScale;
				float minY = Main.npc[i].position.Y - 8f * mapScale;
				float maxX = Main.npc[i].position.X + 8f * mapScale;
				float maxY = Main.npc[i].position.Y + 8f * mapScale;

				if (cursorOnMapX >= (double) minX &&
					cursorOnMapX <= (double) maxX &&
					cursorOnMapY >= (double) minY &&
					cursorOnMapY <= (double) maxY) {
					Main.mouseLeftRelease = false;
					Main.mapFullscreen = false;
					Main.NewText(string.Format("{0} teleported to {1}", Main.player[Main.myPlayer].name, Main.npc[i].FullName), 255, 255, 0);
					Main.player[Main.myPlayer].Teleport(Main.npc[i].position);

					return;
				}
			}
		}
	}
}
