using GnomeWordsmith.Items;
using GnomeWordsmith.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace GnomeWordsmith {
	class GnomeWordsmith : Mod {
		internal static GnomeWordsmith instance;
		public UserInterface customResources;
		public ReforgeUI reforgeUI;
		public GnomeWordsmithGlobalItem gnomeSmithGlobalItem;

		public GnomeWordsmith() {
		}

		// Create instance of our global item to allow for teleports.
		public override void Load() {
			instance = this;
			gnomeSmithGlobalItem = (GnomeWordsmithGlobalItem) GetGlobalItem("GnomeWordsmithGlobalItem");
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
	}
}
