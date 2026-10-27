using System;
using System.Collections.Generic;
using EntityStates;
using MoreSkills.States;

namespace MoreSkills
{
    public enum Slot { Primary, Secondary, Utility, Special }

    public class SkillEntry
    {
        public string Name;
        public string DisplayName;
        public string Description;
        public Type State;
        public Slot Slot;
        public string Machine = "Weapon";
        public float Cooldown;
        public int Stocks = 1;
        public InterruptPriority Priority = InterruptPriority.Skill;
    }

    public static class SkillCatalogEntries
    {
        private const string Dmg = "<style=cIsDamage>";
        private const string Util = "<style=cIsUtility>";
        private const string Heal = "<style=cIsHealing>";
        private const string End = "</style>";

        public static readonly List<SkillEntry> All = new List<SkillEntry>
        {
            // --- Primär ---
            new SkillEntry { Name = "ArcaneBolt", DisplayName = "Arkanbolzen", Slot = Slot.Primary, State = typeof(ArcaneBolt), Priority = InterruptPriority.Any,
                Description = $"Feuere einen Feuerbolzen für {Dmg}240% Schaden{End}." },
            new SkillEntry { Name = "ChainLightning", DisplayName = "Kettenblitz", Slot = Slot.Primary, State = typeof(ChainLightning), Priority = InterruptPriority.Any,
                Description = $"Ein Blitz trifft das anvisierte Ziel für {Dmg}160% Schaden{End} und springt auf {Dmg}3{End} weitere Gegner über." },
            new SkillEntry { Name = "PrecisionShot", DisplayName = "Präzisionsschuss", Slot = Slot.Primary, State = typeof(PrecisionShot), Priority = InterruptPriority.Any,
                Description = $"Ein langsamer, präziser Schuss für {Dmg}320% Schaden{End}." },
            new SkillEntry { Name = "Scattershot", DisplayName = "Streufeuer", Slot = Slot.Primary, State = typeof(Scattershot), Priority = InterruptPriority.Any,
                Description = $"Feuere {Dmg}6{End} Kugeln für je {Dmg}90% Schaden{End} auf kurze Distanz." },

            // --- Sekundär ---
            new SkillEntry { Name = "FragGrenade", DisplayName = "Splittergranate", Slot = Slot.Secondary, State = typeof(FragGrenade), Cooldown = 5f, Stocks = 2,
                Description = $"Wirf eine Granate, die für {Dmg}600% Schaden{End} explodiert. Hält {Util}2 Ladungen{End}." },
            new SkillEntry { Name = "MissileVolley", DisplayName = "Raketensalve", Slot = Slot.Secondary, State = typeof(MissileVolley), Cooldown = 6f,
                Description = $"Feuere {Dmg}4{End} zielsuchende Raketen für je {Dmg}250% Schaden{End}." },
            new SkillEntry { Name = "Shockwave", DisplayName = "Schockwelle", Slot = Slot.Secondary, State = typeof(Shockwave), Cooldown = 6f,
                Description = $"Stoße alle Gegner in der Nähe für {Dmg}500% Schaden{End} zurück." },
            new SkillEntry { Name = "FrostLance", DisplayName = "Frostlanze", Slot = Slot.Secondary, State = typeof(FrostLance), Cooldown = 5f,
                Description = $"{Util}Gefrierend{End}. Ein Eisstrahl für {Dmg}400% Schaden{End}." },

            // --- Utility ---
            new SkillEntry { Name = "Dash", DisplayName = "Sprint-Dash", Slot = Slot.Utility, State = typeof(Dash), Machine = "Body", Cooldown = 4f, Stocks = 2, Priority = InterruptPriority.PrioritySkill,
                Description = $"Sprinte ein kurzes Stück in Laufrichtung und werde dabei {Util}unverwundbar{End}. Hält {Util}2 Ladungen{End}." },
            new SkillEntry { Name = "Blink", DisplayName = "Blinzeln", Slot = Slot.Utility, State = typeof(Blink), Machine = "Body", Cooldown = 5f, Priority = InterruptPriority.PrioritySkill,
                Description = $"{Util}Teleportiere{End} dich bis zu 20 m in Blickrichtung." },
            new SkillEntry { Name = "GroundSlam", DisplayName = "Erdstoß", Slot = Slot.Utility, State = typeof(GroundSlam), Machine = "Body", Cooldown = 7f, Priority = InterruptPriority.PrioritySkill,
                Description = $"Springe hoch und schlage für {Dmg}600% Schaden{End} auf dem Boden ein." },
            new SkillEntry { Name = "Barrier", DisplayName = "Schutzbarriere", Slot = Slot.Utility, State = typeof(Barrier), Cooldown = 10f,
                Description = $"Erhalte eine {Heal}Barriere{End} in Höhe von {Heal}30%{End} deines maximalen Lebens." },

            // --- Spezial ---
            new SkillEntry { Name = "Thunderstorm", DisplayName = "Gewittersturm", Slot = Slot.Special, State = typeof(Thunderstorm), Cooldown = 12f,
                Description = $"Blitze treffen bis zu {Dmg}10{End} Gegner im Umkreis von 40 m für je {Dmg}600% Schaden{End}." },
            new SkillEntry { Name = "HealingNova", DisplayName = "Heilnova", Slot = Slot.Special, State = typeof(HealingNova), Cooldown = 15f,
                Description = $"{Heal}Heile{End} dich und alle Verbündeten im Umkreis von 25 m um {Heal}35%{End} ihres maximalen Lebens." },
            new SkillEntry { Name = "Berserk", DisplayName = "Berserker", Slot = Slot.Special, State = typeof(Berserk), Cooldown = 18f,
                Description = $"Erhalte für 8 s {Dmg}+50% Angriffsgeschwindigkeit{End} und {Util}+30% Bewegungsgeschwindigkeit{End}." },
            new SkillEntry { Name = "Meteor", DisplayName = "Meteor", Slot = Slot.Special, State = typeof(Meteor), Cooldown = 10f,
                Description = $"Lass einen Meteor auf den Zielpunkt stürzen: {Dmg}1200% Schaden{End} im Umkreis von 10 m." },
        };
    }
}
