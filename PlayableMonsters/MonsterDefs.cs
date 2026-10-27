using System.Collections.Generic;
using UnityEngine;

namespace PlayableMonsters
{
    public class MonsterDef
    {
        public string Name;
        public string BodyKey;
        public string DisplayName;
        public string Subtitle;
        public string Description;
        public Color Color;
        public float Health = 110f;
        public float Damage = 12f;
        public float Armor;
        public float MoveSpeed = 7f;
        public float JumpPower = 15f;
        public float Regen = 1f;
        // Kamera-Abstand relativ zu Commando, für große Monster > 1.
        public float CameraScale = 1f;
        // MoreSkills-Skills für Slots, die das Monster nicht selbst hat.
        public string Primary = "ArcaneBolt";
        public string Secondary = "FragGrenade";
        public string Utility = "Dash";
        public string Special = "Thunderstorm";
    }

    public static class MonsterDefs
    {
        // Body-Keys: Addressables-GUIDs aus RoR2BepInExPack.GameAssetPathsBetter.
        public static readonly List<MonsterDef> All = new List<MonsterDef>
        {
            new MonsterDef { Name = "Lemurian", BodyKey = "7dbddf7b5091ed5498164334ac1d43c7", DisplayName = "Lemurianer", Subtitle = "Der Feuerspucker",
                Color = new Color(0.85f, 0.45f, 0.75f), Health = 120f, Damage = 12f, Armor = 5f, Utility = "Dash", Special = "Meteor",
                Description = "Ein flinker Echsenkrieger, der Feuerbälle spuckt und Gegner beißt." },
            new MonsterDef { Name = "ElderLemurian", BodyKey = "aeb788f5fc372714a8827b167aee0594", DisplayName = "Älterer Lemurianer", Subtitle = "Der Flammenfürst",
                Color = new Color(0.3f, 0.3f, 0.35f), Health = 220f, Damage = 14f, Armor = 20f, MoveSpeed = 6f, JumpPower = 13f, CameraScale = 2.2f, Utility = "GroundSlam", Special = "Meteor",
                Description = "Ein riesiger Lemurianer, der Feuerkugeln regnen lässt und Flammen speit." },
            new MonsterDef { Name = "StoneGolem", BodyKey = "f21a0264f74389246acbbc5e281092b1", DisplayName = "Steingolem", Subtitle = "Der Laserkoloss",
                Color = new Color(0.6f, 0.6f, 0.55f), Health = 200f, Damage = 13f, Armor = 20f, MoveSpeed = 6f, JumpPower = 13f, CameraScale = 1.7f, Utility = "Barrier", Special = "Shockwave",
                Description = "Langsam, aber zäh. Klatscht Gegner zu Boden und feuert einen tödlichen Laser." },
            new MonsterDef { Name = "Beetle", BodyKey = "f2ae1aaca82b21c4e8a55edcf1312bc8", DisplayName = "Käfer", Subtitle = "Der Krabbler",
                Color = new Color(0.55f, 0.4f, 0.75f), Health = 110f, Damage = 12f, Armor = 10f, MoveSpeed = 7.5f, Secondary = "Shockwave", Utility = "Dash", Special = "Berserk",
                Description = "Ein kleiner Käfer mit kräftigem Kopfstoß. Unterschätzt ihn nicht." },
            new MonsterDef { Name = "BeetleGuard", BodyKey = "e6d4f1c72713ae2479198584bfa637a4", DisplayName = "Käferwächter", Subtitle = "Der Panzer",
                Color = new Color(0.45f, 0.3f, 0.65f), Health = 210f, Damage = 13f, Armor = 30f, MoveSpeed = 6.5f, CameraScale = 1.8f, Utility = "GroundSlam", Special = "Berserk",
                Description = "Schwer gepanzert. Schlägt Gegner nieder und schleudert Schockwellen." },
            new MonsterDef { Name = "Imp", BodyKey = "3e09a3664cb1cef47aad9e234268a20d", DisplayName = "Imp", Subtitle = "Der Schattenklinger",
                Color = new Color(0.75f, 0.15f, 0.2f), Health = 100f, Damage = 13f, MoveSpeed = 8.5f, JumpPower = 17f, Secondary = "FrostLance", Utility = "Blink", Special = "Berserk",
                Description = "Blitzschnell und tödlich im Nahkampf. Teleportiert sich hinter seine Beute." },
            new MonsterDef { Name = "ClayTemplar", BodyKey = "a59e219f5c78ea949a7103a148d1ba0e", DisplayName = "Lehmtempler", Subtitle = "Das Teermaschinengewehr",
                Color = new Color(0.55f, 0.4f, 0.25f), Health = 190f, Damage = 12f, Armor = 15f, MoveSpeed = 6.5f, CameraScale = 1.6f, Utility = "Barrier", Special = "MissileVolley",
                Description = "Mäht Gegner mit seiner Teer-Minigun nieder und verlangsamt sie." },
            new MonsterDef { Name = "BighornBison", BodyKey = "93e318ca0f6b60840881c5aedaf03bca", DisplayName = "Dickhornbison", Subtitle = "Der Rammbock",
                Color = new Color(0.5f, 0.35f, 0.25f), Health = 180f, Damage = 13f, Armor = 15f, MoveSpeed = 7.5f, CameraScale = 1.6f, Secondary = "Shockwave", Utility = "Dash", Special = "Berserk",
                Description = "Stürmt mit gesenkten Hörnern durch Gegnerhorden." },
            new MonsterDef { Name = "Parent", BodyKey = "b39683568507e2e498f005d3b225e52d", DisplayName = "Urahn", Subtitle = "Der Riese",
                Color = new Color(0.8f, 0.7f, 0.55f), Health = 260f, Damage = 15f, Armor = 10f, MoveSpeed = 6f, JumpPower = 12f, CameraScale = 2.6f, Secondary = "Shockwave", Utility = "Blink", Special = "GroundSlam",
                Description = "Ein uralter Riese, der Gegner mit gewaltigen Schlägen zerquetscht." },
            new MonsterDef { Name = "VoidReaver", BodyKey = "4ac56d0b387fe344bba43f086ba72fe7", DisplayName = "Leerenschnitter", Subtitle = "Der Weltenfresser",
                Color = new Color(0.55f, 0.3f, 0.85f), Health = 230f, Damage = 14f, Armor = 15f, MoveSpeed = 6f, JumpPower = 13f, CameraScale = 2.2f, Utility = "Blink", Special = "Thunderstorm",
                Description = "Beschwört Leerenbomben, die alles verschlingen." },
            new MonsterDef { Name = "LunarGolem", BodyKey = "f4f554568acdb984784da796193f436f", DisplayName = "Mond-Chimäre (Golem)", Subtitle = "Die Mondfestung",
                Color = new Color(0.4f, 0.6f, 0.9f), Health = 220f, Damage = 13f, Armor = 25f, MoveSpeed = 6f, JumpPower = 13f, CameraScale = 1.8f, Utility = "Barrier", Special = "MissileVolley",
                Description = "Eine Festung aus Mondgestein mit Zwillingskanonen und Schutzschild." },
            new MonsterDef { Name = "MiniMushrum", BodyKey = "5744f0917b24e9d4bb5c894cec04b5a6", DisplayName = "Mini-Pilzling", Subtitle = "Der Sporenheiler",
                Color = new Color(0.6f, 0.8f, 0.4f), Health = 140f, Damage = 11f, Armor = 10f, MoveSpeed = 6.5f, Utility = "Barrier", Special = "HealingNova",
                Description = "Verschießt Sporen und hält das Team mit Heilung am Leben." },
        };
    }
}
