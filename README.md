Valheim Mod that changes the way Vegvisirs works. Now they will point at the direction of the altar it is supposed to reveal.

There are many options you can toggle to your own taste in the config file such as:
- enforcing server sync (default: on)
- turn the camera to the altar direction (default: on)
- turn the camera to the north (default: off)
- give precise angle to the altar (default: off)
- mark the location on the map (default: on)
- restricting opening the world map to interacting with the cartography table. Also disables the minimap. (default: off)
- localization, that can be server synced or nor (default: on)

Previous versions of this mod were meant to be used with Aedenthorn's <a href="https://www.nexusmods.com/valheim/mods/1739">Cartography Table Map Restriction</a>. 
Be warned that now this mod's version 1.0.2+ already have Aedenthorn's mod features implemented (check config file to disable it).

<img src="https://github.com/TwentyOneZ/Vegvisir_Directions/blob/main/vegvisir.jpg?raw=true">

I also suggest you to use a <a href="https://valheim.thunderstore.io/package/Fragnarok/ImmersiveCompass/">compass mod</a>, according to your flavor.

Inspired by <a href="https://valheim.thunderstore.io/package/Noctambulant_Blade/True_Vegvisirs/">True Vegvisirs</a> from Noctambulant_Blade and Aedenthorn's <a href="https://www.nexusmods.com/valheim/mods/1739">Cartography Table Map Restriction</a>.

<h2>Installation</h2>
<ol>
<li>Download and install <a href="https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager">Thunderstore Mod Manager</a> or <a href="https://valheim.thunderstore.io/package/ebkr/r2modman/">r2modman</a></li>
<li>Click <strong>Install with Mod Manager</strong> button on top of the page. Allow the browser to open the Mod Manager.</li>
<li>Run the game via the mod manager, selecting it, choosing a profile and clicking on "start modded".</li>
</ol>

<h2>Known conflicts:</h2>
To resolve the issue of Wishbone status not working on map restrictions when also using EpicLoot mod, set the following option in the twentyOneZ.VegvisirDirections.cfg file:
`Map enabled with this effect = CustomWishboneFinder`
Thanks to [melchitherabbit](https://github.com/melchitherabbit) for this one!

<h3>Changelog 1.0.2</h3>
<ul>
<li>Implemented ServerSync (thanks <a href="https://github.com/blaxxun-boop/ServerSync">blaxxun-boop</a>).</li>
<li>Implemented configurable Map access restriction to the cartography table (thanks <a href="https://www.nexusmods.com/valheim/mods/1739">Aedenthorn</a>).</li>
<li>Improved default phrases for each direction (thanks ChatGPT).</li>
</ul>

<h3>Changelog 1.0.1</h3>
<ul>
<li>Improved icon (thanks StableDiffusion).</li>
<li>Updated Readme.</li>
</ul>
