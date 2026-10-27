# Risk of Rain 2 Mods

Mods für Risk of Rain 2, gebaut mit [BepInEx](https://github.com/BepInEx/BepInEx) und [R2API](https://github.com/risk-of-thunder/R2API).

## Mods

| Mod | Inhalt |
| --- | --- |
| [MoreItems](MoreItems) | **102 neue Items** in allen Seltenheiten (weiß, grün, rot, Boss, Mond) mit eigenen Icons |
| [MoreSkills](MoreSkills) | **16 neue Skills**, die jeder Survivor zusätzlich wählen kann, auch Survivors aus anderen Mods |
| [PlayableMonsters](PlayableMonsters) | **12 neue Survivors**: Lemurianer, Golem, Käfer, Imp, Leerenschnitter und mehr, mit Original-Modellen und -Angriffen |
| [MoreMonsters](MoreMonsters) | **6 neue Elite-Typen** und **12 neue Gegner/Bosse** (Käferkönig, Urgolem, Leerentitan, ...) |
| [KillBonus](KillBonus) | Kleines Beispiel: Kills geben manchmal Bonus-Gold |

## Installieren (ohne selbst zu bauen)

1. Auf GitHub unter **Actions** → letzter grüner **Build**-Lauf → Artefakt **mods** herunterladen und entpacken.
   Darin liegt pro Mod ein Thunderstore-Zip.
2. In [r2modman](https://thunderstore.io/package/ebkr/r2modman/) ein Profil anlegen und diese Abhängigkeiten installieren:
   **BepInExPack**, **R2API ContentManagement**, **R2API Director**, **R2API Elites**, **R2API Items**,
   **R2API Language**, **R2API Prefab**, **R2API RecalculateStats**.
3. In r2modman: **Settings → Import local mod** und jedes Zip auswählen.
   PlayableMonsters braucht MoreSkills.
4. **Start modded**.

Jede Mod hat eine Config unter `BepInEx/config/xking41.<Mod>.cfg`. Dort lässt sich jedes Item, jeder Skill,
Survivor, Elite-Typ und Gegner einzeln abschalten.

## Selbst bauen

```
dotnet build MoreItems -c Release
```

Die Spiel-DLLs kommen als NuGet-Paket, das Spiel muss zum Bauen nicht installiert sein.
Jeder Push baut automatisch alle Mods (siehe `.github/workflows/build.yml`).

## Aufbau

- `Common/`: geteilter Code (Asset-Laden, Icons, Asset-Schlüssel des Spiels)
- `tools/`: Icon-Generator (`python3 tools/item_icons.py` erzeugt alle Item-Icons neu)
- Neue Items: ein Eintrag in `MoreItems/Items/ItemList.cs`, danach `tools/item_icons.py` ausführen
- Neue Skills: State in `MoreSkills/States/`, Eintrag in `MoreSkills/SkillCatalogEntries.cs`
- Neue Survivors: Eintrag in `PlayableMonsters/MonsterDefs.cs`
- Neue Gegner/Elites: Eintrag in `MoreMonsters/Variants.cs` bzw. `MoreMonsters/Elites.cs`

## Noch offen

- Echte 3D-Modelle für Items (aktuell Fragezeichen-Modell im Spiel, aber eigene Icons)
- Neue Maps und komplett neue Modelle: brauchen den Unity-Editor, um AssetBundles zu bauen
