using GnomeWordsmith.UI;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace GnomeWordsmith.NPCs {
	[AutoloadHead]
	public class GnomeWordsmithNPC : ModNPC {
		public override string Texture {
			get {
				return "GnomeWordsmith/NPCs/GnomeWordsmithNPC";
			}
		}

		public override string[] AltTextures {
			get {
				return new string[] { "GnomeWordsmith/NPCs/GnomeWordsmithNPC_Party" };
			}
		}

		public override bool Autoload(ref string name) {
			name = "GnomeWordsmith";
			return mod.Properties.Autoload;
		}

		// TODO: Investigate adding an attack
		public override void SetStaticDefaults() {
			Main.npcFrameCount[npc.type] = 26;
			NPCID.Sets.AttackFrameCount[npc.type] = 5;
			NPCID.Sets.DangerDetectRange[npc.type] = 700;
			NPCID.Sets.AttackType[npc.type] = 0;
			NPCID.Sets.AttackTime[npc.type] = 15;
			NPCID.Sets.AttackAverageChance[npc.type] = 45;
			NPCID.Sets.HatOffsetY[npc.type] = 8;
		}

		public override void SetDefaults() {
			npc.townNPC = true;
			npc.friendly = true;
			npc.width = 18;
			npc.height = 40;
			npc.aiStyle = 7;
			npc.damage = 10;
			// Gnome Wordsmith has higher defense and health than other TownNPCs
			npc.defense = 30;
			npc.lifeMax = 500;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = 0.5f;
			animationType = NPCID.Guide;
		}

		// TODO: Investigate other hit effects
		public override void HitEffect(int hitDirection, double damage) {
			int num = npc.life > 0 ? 1 : 5;
			for (int k = 0; k < num; k++) {
				Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType("Sparkle"));
			}
		}

		// Only spawn if both the Goblin and the Steampunker are in your world.
		public override bool CanTownNPCSpawn(int numTownNPCs, int money) {
			int goblinkTinkerer = NPC.FindFirstNPC(NPCID.GoblinTinkerer);
			int steampunker = NPC.FindFirstNPC(NPCID.Steampunker);
			return goblinkTinkerer >= 0 && steampunker >= 0;
		}

		// TODO: Add localization support
		public override string TownNPCName() {
			WeightedRandom<string> name = new WeightedRandom<string>();

			name.Add("Arne");
			name.Add("Egil");
			name.Add("Gunnar");
			name.Add("Harald");
			name.Add("Hjalmar");
			name.Add("Leif");
			name.Add("Ragnar");
			name.Add("Nisse", 0.5);

			return name.Get();
		}

		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			chat.Add(Language.GetTextValue("Mods.GnomeWordsmith.NPCChat.GnomeWordsmith.Rand0"));
			chat.Add(Language.GetTextValue("Mods.GnomeWordsmith.NPCChat.GnomeWordsmith.Rand1"));
			chat.Add(Language.GetTextValue("Mods.GnomeWordsmith.NPCChat.GnomeWordsmith.Rand2"));
			int goblinkTinkerer = NPC.FindFirstNPC(NPCID.GoblinTinkerer);
			if (goblinkTinkerer >= 0) {
				chat.Add(Language.GetTextValue("Mods.GnomeWordsmith.NPCChat.GnomeWordsmith.GoblinTinkerer", Main.npc[goblinkTinkerer].GivenName));
			}

			int steampunker = NPC.FindFirstNPC(NPCID.Steampunker);
			if (steampunker >= 0) {
				chat.Add(Language.GetTextValue("Mods.GnomeWordsmith.NPCChat.GnomeWordsmith.Steampunker", Main.npc[steampunker].GivenName));
			}

			if (Main.bloodMoon) {
				chat.Add(Language.GetTextValue("Mods.GnomeWordsmith.NPCChat.GnomeWordsmith.BloodMoon"));
			}

			return chat.Get();
		}

		// TODO: Steal "Reforge" text from Vanilla interface text.
		public override void SetChatButtons(ref string button, ref string button2) {
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = Language.GetTextValue("LegacyInterface.19");
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			if (firstButton) {
				shop = true;
			} else {
				// TODO: Turn interface opening into a method?

				// Close chat window
				Main.playerInventory = true;
				Main.npcChatText = "";
				Main.PlaySound(SoundID.MenuTick);
				ReforgeUI.visible = true;
			}
		}

		// TODO: Add more "gnome-themed" items to shop.
		public override void SetupShop(Chest shop, ref int nextSlot) {
			shop.item[nextSlot++].SetDefaults(ItemID.FallenStar);
			shop.item[nextSlot++].SetDefaults(mod.ItemType("PortableWormhole"));
		}

		public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
			damage = 60;
			knockback = 4f;
		}

		public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
			cooldown = 5;
			randExtraCooldown = 2;
		}

		public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
			projType = ProjectileID.StarWrath;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
			multiplier = 8f;
			randomOffset = 1f;
		}
	}
}
