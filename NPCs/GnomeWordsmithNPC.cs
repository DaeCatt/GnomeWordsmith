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
			name = "Gnome Wordsmith";
			return mod.Properties.Autoload;
		}

		// TODO: Investigate adding an attack
		public override void SetStaticDefaults() {
			Main.npcFrameCount[npc.type] = 26;
			NPCID.Sets.AttackFrameCount[npc.type] = 5;
			NPCID.Sets.DangerDetectRange[npc.type] = 700;
			NPCID.Sets.AttackType[npc.type] = -1; // No attack
			NPCID.Sets.AttackTime[npc.type] = 30;
			NPCID.Sets.AttackAverageChance[npc.type] = 30;
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

		// TODO: Add localization support
		public override string GetChat() {
			WeightedRandom<string> chat = new WeightedRandom<string>();

			chat.Add("Forging's an ancient and precise craft. Goblins are just scratching the surface.");

			return chat.Get();
		}

		// TODO: Steal "Reforge" text from Vanilla interface text.
		public override void SetChatButtons(ref string button, ref string button2) {
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = "Reforge";
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop) {
			if (firstButton) {
				shop = true;
			} else {
				// TODO: Turn interface opening into a method?

				// Close chat window
				Main.npcChatText = "";
				// Open player inventory
				Main.playerInventory = true;
				// Set self to "owner" of UI
				ReforgeUI.ownerNPC = npc;
				// Open UI
				ReforgeUI.visible = true;
			}
		}

		// TODO: Add more "gnome-themed" items to shop.
		public override void SetupShop(Chest shop, ref int nextSlot) {
			shop.item[nextSlot].SetDefaults(mod.ItemType("PortableWormhole"));
			nextSlot++;
		}

		/**
		 * From https://github.com/hamstar0/tml-rewards-mod/blob/master/NPCs/WayfarerTownNPC.cs
		 */

		/*
		private bool IsFiring = false;

		public override void AI() {
			if( npc.ai[0] == 12 ) {
				if( !IsFiring ) {
					IsFiring = true;
					Main.PlaySound( SoundID.Item11, npc.position );
				}
			} else {
				if( IsFiring ) {
					IsFiring = false;
				}
			}
		}

		public override void DrawTownAttackGun( ref float scale, ref int item, ref int closeness ) {
			item = ItemID.Boomstick;
			scale = 0.75f;

			if( npc.ai[2] < -0.1f ) {
				closeness = 28;
			}
		}

		public override void TownNPCAttackStrength( ref int damage, ref float knockback ) {
			if( Main.hardMode ) {
				damage = 50;
				knockback = 4f;
			} else {
				damage = 20;
				knockback = 4f;
			}
		}

		public override void TownNPCAttackCooldown( ref int cooldown, ref int randExtraCooldown ) {
			cooldown = 10;
			randExtraCooldown = 5;
		}

		public override void TownNPCAttackProj( ref int projType, ref int attackDelay ) {
			projType = ProjectileID.Bullet;
			attackDelay = 1;
		}

		public override void TownNPCAttackProjSpeed( ref float multiplier, ref float gravityCorrection, ref float randomOffset ) {
			multiplier = 20f;
			randomOffset = 0f;
		}
		*/
	}
}