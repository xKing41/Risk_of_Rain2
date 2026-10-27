# Risk of Rain 2 Mods

Mods für Risk of Rain 2, gebaut mit [BepInEx](https://github.com/BepInEx/BepInEx).

## Mods

| Mod | Beschreibung |
| --- | --- |
| [KillBonus](KillBonus) | Kills haben eine Chance, Bonus-Gold zu geben (konfigurierbar). |
| [MoreItems](MoreItems) | Neue Items, aktuell 18 (Platzhalter-Icons). |

## Einrichtung

1. [.NET SDK](https://dotnet.microsoft.com/download) installieren (8.0 oder neuer).
2. Im [r2modman](https://thunderstore.io/package/ebkr/r2modman/)-Profil **BepInExPack** installieren.
3. Bauen:
   ```
   dotnet build KillBonus -c Release
   ```
   Die Spiel-DLLs kommen als NuGet-Paket (`RiskOfRain2.GameLibs`), das Spiel muss zum Bauen nicht installiert sein.
4. `KillBonus/bin/Release/netstandard2.1/KillBonus.dll` in den Ordner
   `BepInEx/plugins/` deines r2modman-Profils kopieren und das Spiel modded starten.

Für **MoreItems** genauso (`dotnet build MoreItems -c Release`). Zusätzlich brauchst du
im r2modman-Profil diese Mods: **R2API Items**, **R2API Language**, **R2API RecalculateStats**.

## Roadmap

| Etappe | Inhalt | Braucht Unity? |
| --- | --- | --- |
| 1 | Item-Framework + erste Items | nein |
| 2 | Hunderte Items in Batches, eigene Icons | nein (Icons: Bildprogramm) |
| 3 | Zusätzliche Skills für alle Survivors (auch modded) | nein, nutzt vorhandene Animationen |
| 4 | Neue Gegner und Bosse auf Basis vorhandener Modelle | nein |
| 5 | Komplett neue Survivors, Gegner, Bosse | ja (Modelle, Animationen, AssetBundles) |
| 6 | Neue Maps | ja (Unity-Szenen) |

## Für Thunderstore packen

Zip mit diesen Dateien: `manifest.json`, `README.md` (beide aus `KillBonus/thunderstore/`),
`KillBonus.dll` und ein 256×256 `icon.png`.
