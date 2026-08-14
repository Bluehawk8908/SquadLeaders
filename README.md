# Squad Leaders
A mod for Gunner Heat PC! that cosmetically distinguishes platoon leaders and squad/section leaders from other infantry  

**Installation**  
Requires MelonLoader, intended for version 0.6.1.  
Extract the zip into the /mods/ folder such that the PNG textures are housed in their own sub-folder /SquadLeaders/, and the DLL file is separate.

**Configuration**  
For compatibility with the Soviet uniform tweaks mod, Soviet Airborne and the Canadian Leopards mods, there are options in the config file for choosing which style of shoulder-straps Soviet squad leaders should have, and to enable Canadian squad leaders (if you don't enable this, you'll get Bundeswehr Heer NCOs running around in Canadian squads!)  

The "LeaderPromoted" MonoBehaviour component contains rank information in a public field as an enum, which may be used by other mod authors if they wish to hook into the squad system. The potential values are: SquadLeader, PlatoonSgt, PlatoonLeader.
