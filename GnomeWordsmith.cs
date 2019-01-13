using GnomeWordsmith.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace GnomeWordsmith {
	class GnomeWordsmith : Mod {
		internal static GnomeWordsmith instance;
		internal static bool unityMouseOver = false;
		public UserInterface customResources;
		public ReforgeUI reforgeUI;

		public GnomeWordsmith() {
		}

		public override void Load() {
			instance = this;
			ReforgeUI.visible = false;

			if (!Main.dedServ) {
				// Create Gnome Reforge Interface
				reforgeUI = new ReforgeUI();
				ReforgeUI.visible = true;

				customResources = new UserInterface();
				customResources.SetState(reforgeUI);
			}
		}

		// Insert our interface layer
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

			if (inventoryIndex != -1) {
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"GnomeWordsmith: Reforge UI",
					delegate {
						if (ReforgeUI.visible) {
							customResources.Update(Main._drawInterfaceGameTime);
							reforgeUI.Draw(Main.spriteBatch);
						}
						return true;
					},
					InterfaceScaleType.UI
				));
			}
		}

		public override void PostDrawFullscreenMap(ref string mouseText) {
			if (!Main.player[Main.myPlayer].HasItem(instance.ItemType("PortableWormhole"))) {
				unityMouseOver = false;
				return;
			}

			bool foundTarget = false;
			string text = "";

			float mapWorldScale = Main.mapFullscreenScale / 16f;
			float offsetX = Main.screenWidth / 2 - Main.mapFullscreenPos.X * Main.mapFullscreenScale;
			float offsetY = Main.screenHeight / 2 - Main.mapFullscreenPos.Y * Main.mapFullscreenScale;

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
						// For some reason Main.DrawPlayerHead is protected :(
						// Main.DrawPlayerHead(Main.player[i], playerHeadCenterX, playerHeadCenterY, 2f, Main.UIScale + 0.5f);

						if (!unityMouseOver) {
							Main.PlaySound(12, -1, -1, 1, 1f, 0f);
						}

						foundTarget = true;
						unityMouseOver = true;

						if (Main.mouseLeft && Main.mouseLeftRelease) {
							Main.mouseLeftRelease = false;
							Main.mapFullscreen = false;
							Main.player[Main.myPlayer].UnityTeleport(Main.player[i].position);
						} else if (text == "") {
							text = Language.GetTextValue("Game.TeleportTo", Main.player[i].name);
						}

						break;
					}
				}
			}

			if (!foundTarget) {
				for (int i = 0; i < Main.npc.Length; i++) {
					// Only check active NPCs that are set to townNPC.
					if (!Main.npc[i].active || !Main.npc[i].townNPC) {
						continue;
					}

					int headIndex = NPC.TypeToHeadIndex(Main.npc[i].type);
					if (headIndex <= 0) {
						continue;
					}

					Texture2D headTexture = Main.npcHeadTexture[headIndex];

					float npcHeadCenterX = offsetX + mapWorldScale * (Main.npc[i].position.X + Main.npc[i].width / 2);
					float npcHeadCenterY = offsetY + mapWorldScale * (Main.npc[i].position.Y + Main.npc[i].gfxOffY + Main.npc[i].height / 2);

					float minX = npcHeadCenterX - headTexture.Width / 2 * Main.UIScale;
					float minY = npcHeadCenterY - headTexture.Height / 2 * Main.UIScale;
					float maxX = minX + headTexture.Width * Main.UIScale;
					float maxY = minY + headTexture.Height * Main.UIScale;

					if (Main.mouseX >= minX && Main.mouseX <= maxX && Main.mouseY >= minY && Main.mouseY <= maxY) {
						SpriteEffects effect = SpriteEffects.None;
						if (Main.npc[i].direction > 0) {
							effect = SpriteEffects.FlipHorizontally;
						}

						Main.spriteBatch.Draw(headTexture, new Vector2(npcHeadCenterX, npcHeadCenterY), new Rectangle(0, 0, headTexture.Width, headTexture.Height), Color.White, 0f, new Vector2(headTexture.Width / 2, headTexture.Height / 2), Main.UIScale + 0.5f, effect, 0f);

						if (!unityMouseOver) {
							Main.PlaySound(12, -1, -1, 1, 1f, 0f);
						}

						foundTarget = true;
						unityMouseOver = true;

						if (Main.mouseLeft && Main.mouseLeftRelease) {
							Main.mouseLeftRelease = false;
							Main.mapFullscreen = false;

							Main.NewText(Language.GetTextValue("Game.HasTeleportedTo", Main.player[Main.myPlayer].name, Main.npc[i].FullName), 255, 255, 0);
							Main.player[Main.myPlayer].Teleport(Main.npc[i].position);
						} else if (text == "") {
							text = Language.GetTextValue("Game.TeleportTo", Main.npc[i].FullName);
						}

						break;
					}
				}
			}

			if (!foundTarget && unityMouseOver) {
				unityMouseOver = false;
			}

			if (text != "") {
				mouseText = text;
			}
		}
	}
}
