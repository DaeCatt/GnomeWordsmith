using Microsoft.Xna.Framework.Graphics;
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
			if (!Main.mapFullscreen && item.type != mod.ItemType("PortableWormhole"))
				return;

			if (!Main.mouseLeft || !Main.mouseLeftRelease)
				return;

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

					float minX = Main.player[i].position.X - 14f * mapScale;
					float minY = Main.player[i].position.Y - 14f * mapScale;
					float maxX = Main.player[i].position.X + 14f * mapScale;
					float maxY = Main.player[i].position.Y + 14f * mapScale;

					if (cursorOnMapX >= (double) minX &&
						cursorOnMapX <= (double) maxX &&
						cursorOnMapY >= (double) minY &&
						cursorOnMapY <= (double) maxY) {
						Main.mouseLeftRelease = false;
						Main.mapFullscreen = false;
						Main.player[Main.myPlayer].UnityTeleport(Main.player[i].position);
						// hoverTarget = Main.player[i].name;

						return;
					}
				}
			}

			for (int i = 0; i < Main.npc.Length; i++) {
				// Only check NPCs that are set to townNPC.
				if (!Main.npc[i].active || !Main.npc[i].townNPC) {
					continue;
				}

				int headIndex = NPC.TypeToHeadIndex(Main.npc[i].type);
				if (headIndex <= 0) {
					continue;
				}

				float halfWidth = Main.npcHeadTexture[headIndex].Width / 2;
				float halfHeight = Main.npcHeadTexture[headIndex].Height / 2;

				float minX = Main.npc[i].position.X - halfWidth * mapScale;
				float minY = Main.npc[i].position.Y - halfHeight * mapScale;
				float maxX = Main.npc[i].position.X + halfWidth * mapScale;
				float maxY = Main.npc[i].position.Y + halfHeight * mapScale;

				if (cursorOnMapX >= (double) minX &&
					cursorOnMapX <= (double) maxX &&
					cursorOnMapY >= (double) minY &&
					cursorOnMapY <= (double) maxY) {
					Main.mouseLeftRelease = false;
					Main.mapFullscreen = false;

					Main.NewText(Language.GetTextValue("Game.HasTeleportedTo", Main.player[Main.myPlayer].name, Main.npc[i].FullName), 255, 255, 0);
					Main.player[Main.myPlayer].Teleport(Main.npc[i].position);
					// hoverTarget = Main.npc[i].FullName;

					return;
				}
			}
		}
	}
}
