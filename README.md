# Gnome Wordsmith 0.99.x

Adds the Gnome Wordsmith NPC which allows you to Reforge items by selecting the prefix you want.
The Gnome Wordsmith also sells a Portable Wormhole, which functions like a re-usable Wormhole, but which can also be used to teleport to Town NPCs.

The Gnome Wordsmith can only spawn once both the Goblin Tinkerer and the Steampunker have joined your town.

![Image of Gnome Wordsmith's Head](NPCs/GnomeWordsmithNPC_Head.png)

## Known Issues

### Should be fixed for 1.0 release

- No Gamepad support. This interface requires you to use a mouse to navigate.
- No scaling support. Certain (small) resolutions might suffer from having the interface go off-screen. This should only be an issue for accessories, which have the most possible prefixes.
- No support for mod prefixes. Introducing support should be pretty easy, but ties into the above issue of parts of the interface potentially appearing off-screen.
- The Gnome Wordsmith will not attack enemies. This is partially because it's a "short" NPCs, so some attack animations appear odd.
- Portable Wormhole doesn't take NPC / Player head size into account.
- Portable Wormhole teleportation happens immediately. A delay might be introduced.

### tModLoader changes required:

- While using the gnome reforge interface the inventory crafting interface will occasionally flash in for 1 frame. Fixing this would require introducing a new variable in tModLoader to allow hiding the inventory crafting interface. As of right now the game only hides the inventory crafting interface is the Goblin Tinkerer Reforge interface is also open.

## Thanks to:

- tModLoader Discord for being helpful.
- Infinity - Endless Items by LolKat & DragonHunter003. I disassembled this mod to learn how the infinite wormhole potion works. I ended up writing my own code to handle the Portable Wormhole :)
