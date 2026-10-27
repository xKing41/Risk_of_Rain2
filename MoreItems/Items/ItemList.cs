using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using static MoreItems.Items.B;

namespace MoreItems.Items
{
    public static class ItemList
    {
        private static readonly ItemTag[] Dmg = { ItemTag.Damage };
        private static readonly ItemTag[] UtilT = { ItemTag.Utility };
        private static readonly ItemTag[] Heal = { ItemTag.Healing };
        private static readonly Dictionary<CharacterBody, float> phoenixReady = new Dictionary<CharacterBody, float>();

        private static float Proc(DamageReport r) => r.damageInfo.procCoefficient;

        public static IEnumerable<ModItem> All() => White().Concat(Green()).Concat(Red()).Concat(Yellow()).Concat(Blue());

        // ===================================================== Gewöhnlich (weiß)
        private static IEnumerable<ModItem> White() => new[]
        {
            Stat("RustyWhetstone", "Rostiger Wetzstein", T1, "✦", "Erhöht den Schaden.",
                $"Erhöht den Schaden um {D("8%")} {S("+8% pro Stack")}.",
                (a, n) => a.damageMultAdd += 0.08f * n, Dmg),
            Stat("FeatherBoots", "Federleichte Stiefel", T1, "⇑", "Springe höher.",
                $"Erhöht die Sprungkraft um {U("15%")} {S("+15% pro Stack")}.",
                (a, n) => a.jumpPowerMultAdd += 0.15f * n, UtilT),
            Stat("CopperPlate", "Kupferplatte", T1, "▣", "Erhöht die Rüstung.",
                $"Erhöht die Rüstung um {H("6")} {S("+6 pro Stack")}.",
                (a, n) => a.armorAdd += 6f * n, UtilT),
            Stat("CoffeeBean", "Kaffeebohne", T1, "◍", "Etwas schneller angreifen und laufen.",
                $"Erhöht die Angriffsgeschwindigkeit um {D("8%")} und die Bewegungsgeschwindigkeit um {U("5%")} {S("je pro Stack")}.",
                (a, n) => { a.attackSpeedMultAdd += 0.08f * n; a.moveSpeedMultAdd += 0.05f * n; }, UtilT),
            Stat("LuckyLadybug", "Glücksmarienkäfer", T1, "✿", "Erhöht die kritische Trefferchance.",
                $"Erhöht die kritische Trefferchance um {D("5%")} {S("+5% pro Stack")}.",
                (a, n) => a.critAdd += 5f * n, Dmg),
            Stat("HerbalTea", "Kräutertee", T1, "❦", "Regeneriere Leben.",
                $"Erhöht die Lebensregeneration um {H("1.2 LP/s")} {S("+1.2 LP/s pro Stack")}.",
                (a, n) => a.baseRegenAdd += 1.2f * n, Heal),
            Stat("BandageRoll", "Verbandsrolle", T1, "✚", "Erhöht das maximale Leben.",
                $"Erhöht das maximale Leben um {H("20")} {S("+20 pro Stack")}.",
                (a, n) => a.baseHealthAdd += 20f * n, Heal),
            Stat("LightBoots", "Leichte Sohlen", T1, "»", "Laufe schneller.",
                $"Erhöht die Bewegungsgeschwindigkeit um {U("7%")} {S("+7% pro Stack")}.",
                (a, n) => a.moveSpeedMultAdd += 0.07f * n, UtilT),
            Stat("IronRing", "Eisenring", T1, "◯", "Etwas Rüstung und Leben.",
                $"Erhöht die Rüstung um {H("3")} und das maximale Leben um {H("10")} {S("je pro Stack")}.",
                (a, n) => { a.armorAdd += 3f * n; a.baseHealthAdd += 10f * n; }, UtilT),
            Stat("SharpeningOil", "Schärföl", T1, "♦", "Etwas mehr Schaden und Krit.",
                $"Erhöht den Schaden um {D("4%")} und die kritische Trefferchance um {D("3%")} {S("je pro Stack")}.",
                (a, n) => { a.damageMultAdd += 0.04f * n; a.critAdd += 3f * n; }, Dmg),
            Stat("Pocketwatch", "Taschenuhr", T1, "◷", "Verringert alle Abklingzeiten leicht.",
                $"Verringert alle Abklingzeiten um {U("4%")} {S("+4% pro Stack, multiplikativ")}.",
                (a, n) => a.cooldownMultAdd -= 1f - Mult(0.96f, n), UtilT),
            Stat("SpareMag", "Ersatzmagazin", T1, "≡", "Kürzere Abklingzeit für den Sekundär-Skill.",
                $"Verringert die Abklingzeit des {U("Sekundär-Skills")} um {U("10%")} {S("+10% pro Stack, multiplikativ")}.",
                (a, n) => a.secondaryCooldownMultAdd -= 1f - Mult(0.9f, n), UtilT),
            Stat("TinyBattery", "Kleine Batterie", T1, "⌁", "Kürzere Abklingzeit für den Utility-Skill.",
                $"Verringert die Abklingzeit des {U("Utility-Skills")} um {U("10%")} {S("+10% pro Stack, multiplikativ")}.",
                (a, n) => a.utilityCooldownMultAdd -= 1f - Mult(0.9f, n), UtilT),
            Stat("ShieldGenerator", "Schildgenerator", T1, "◈", "Erhalte einen kleinen Schild.",
                $"Erhalte einen {H("Schild")} mit {H("15")} {S("+15 pro Stack")} Punkten.",
                (a, n) => a.baseShieldAdd += 15f * n, UtilT),
            StatB("VeteranMedal", "Veteranenmedaille", T1, "★", "Mehr Schaden mit jedem Level.",
                $"Erhöht den Schaden um {D("1%")} {S("+1% pro Stack")} pro Charakterlevel.",
                (b, a, n) => a.damageMultAdd += 0.01f * n * b.level, Dmg),
            Stat("SoldiersRation", "Soldatenration", T1, "♨", "Erhöht das maximale Leben prozentual.",
                $"Erhöht das maximale Leben um {H("5%")} {S("+5% pro Stack")}.",
                (a, n) => a.healthMultAdd += 0.05f * n, Heal),
            Stat("GripTape", "Griffband", T1, "⊞", "Etwas Angriffstempo und Rüstung.",
                $"Erhöht die Angriffsgeschwindigkeit um {D("6%")} und die Rüstung um {H("4")} {S("je pro Stack")}.",
                (a, n) => { a.attackSpeedMultAdd += 0.06f * n; a.armorAdd += 4f * n; }, Dmg),
            Stat("RunningShoes", "Laufschuhe", T1, "≫", "Sprinte schneller.",
                $"Erhöht die Sprintgeschwindigkeit um {U("15%")} {S("+15% pro Stack")}.",
                (a, n) => a.sprintSpeedAdd += 0.15f * n, UtilT),
            Stat("BrassKnuckles", "Schlagring", T1, "⊛", "Kritische Treffer verursachen mehr Schaden.",
                $"Erhöht den kritischen Schaden um {D("10%")} {S("+10% pro Stack")}.",
                (a, n) => a.critDamageMultAdd += 0.1f * n, Dmg),

            Hit("VampireFang", "Vampirzahn", T1, "♥", "Heile dich um einen Teil des verursachten Schadens.",
                $"Treffer {H("heilen")} dich um {H("1.5%")} {S("+1.5% pro Stack")} des verursachten Schadens.", 100f,
                (a, v, r, n) => Fx.Heal(a, r.damageDealt * 0.015f * n * Proc(r)), Heal),
            Hit("Matchstick", "Streichholz", T1, "♠", "Chance, Gegner in Brand zu setzen.",
                $"{D("8%")} Chance, Gegner für 3 s {D("in Brand")} zu setzen {S("+50% Brandschaden pro Stack")}.", 8f,
                (a, v, r, n) => Fx.Dot(a, v, DotController.DotIndex.Burn, 3f, 0.5f + 0.5f * n), Dmg),
            Hit("FrostShard", "Frostsplitter", T1, "❄", "Chance, Gegner zu verlangsamen.",
                $"{U("10%")} Chance, Gegner für {U("2 s")} {S("+1 s pro Stack")} um 60% zu {U("verlangsamen")}.", 10f,
                (a, v, r, n) => Fx.Buff(v, RoR2Content.Buffs.Slow60, 1f + n), UtilT),
            Hit("StaticCharge", "Statische Ladung", T1, "ϟ", "Chance, einen Blitz auszulösen.",
                $"{D("8%")} Chance auf einen Blitz für {D("60%")} {S("+60% pro Stack")} Gesamtschaden.", 8f,
                (a, v, r, n) => Fx.Lightning(a, r.damageInfo.position, Fx.HurtBoxOf(v), r.damageInfo.damage * 0.6f * n), Dmg),
            Hit("Slingshot", "Kieselschleuder", T1, "●", "Chance auf einen zusätzlichen Treffer.",
                $"{D("10%")} Chance auf einen Zusatztreffer für {D("50%")} {S("+50% pro Stack")} Basisschaden.", 10f,
                (a, v, r, n) => Fx.Damage(a, v, a.damage * 0.5f * n), Dmg),
            Hit("GoldTooth", "Goldzahn", T1, "$", "Treffer geben manchmal Gold.",
                $"{U("5%")} Chance, bei einem Treffer {U("1 Gold")} {S("+1 pro Stack")} zu erhalten. Skaliert mit der Zeit.", 5f,
                (a, v, r, n) => Fx.Gold(a, n), ItemTag.Utility, ItemTag.AIBlacklist),

            Kill("PowderHorn", "Pulverhorn", T1, "✸", "Getötete Gegner explodieren.",
                $"Getötete Gegner explodieren im Umkreis von {D("4 m")} {S("+1.5 m pro Stack")} für {D("100%")} Basisschaden.", 100f,
                (k, r, n) => Fx.Blast(k, r.victimBody ? r.victimBody.corePosition : k.corePosition, 2.5f + 1.5f * n, k.damage, false, true), ItemTag.Damage, ItemTag.OnKillEffect),
            Kill("HuntersTrophy", "Jagdtrophäe", T1, "✪", "Kills heilen dich ein wenig.",
                $"Jeder Kill {H("heilt")} dich um {H("8")} {S("+8 pro Stack")} Leben.", 100f,
                (k, r, n) => Fx.Heal(k, 8f * n), ItemTag.Healing, ItemTag.OnKillEffect),

            Hurt("ThornVest", "Dornenweste", T1, "✶", "Wirf einen Teil des erlittenen Schadens zurück.",
                $"Wenn du getroffen wirst, erleidet der Angreifer {D("15%")} {S("+15% pro Stack")} des Schadens.", 100f,
                (a, v, r, n) => { if (a) Fx.Damage(v, a, r.damageDealt * 0.15f * n); }, Dmg),
            Hurt("PaddedJacket", "Gepolsterte Jacke", T1, "▢", "Treffer geben dir etwas Barriere.",
                $"Wenn du getroffen wirst, erhältst du {H("2%")} {S("+2% pro Stack")} deines maximalen Lebens als {H("Barriere")}.", 100f,
                (a, v, r, n) => Fx.Barrier(v, v.healthComponent.fullCombinedHealth * 0.02f * n), Heal),
            Hurt("AdrenalineShot", "Adrenalinspritze", T1, "⚕", "Chance, nach einem Treffer schneller zu werden.",
                $"{U("15%")} Chance, nach einem Treffer für {U("2 s")} {S("+1 s pro Stack")} massiv schneller zu laufen.", 15f,
                (a, v, r, n) => Fx.Buff(v, RoR2Content.Buffs.CloakSpeed, 1f + n), UtilT),

            Cond("CampfireCharm", "Lagerfeuer-Amulett", T1, "♣", "Regeneriere schneller, wenn du stillstehst.",
                $"Wenn du {U("1 s stillstehst")}, erhältst du {H("+3 LP/s")} {S("+3 LP/s pro Stack")} Regeneration.",
                b => b.notMovingStopwatch > 1f, (a, n) => a.baseRegenAdd += 3f * n, Heal),
            Cond("RunnersBand", "Läuferband", T1, "⊃", "Schneller beim Sprinten.",
                $"Beim {U("Sprinten")}: {U("+12%")} {S("+12% pro Stack")} Bewegungsgeschwindigkeit.",
                b => b.isSprinting, (a, n) => a.moveSpeedMultAdd += 0.12f * n, UtilT),
            Cond("KitesTail", "Drachenschwanz", T1, "∿", "Mehr Schaden in der Luft.",
                $"In der {U("Luft")}: {D("+15%")} {S("+15% pro Stack")} Schaden.",
                b => b.characterMotor && !b.characterMotor.isGrounded, (a, n) => a.damageMultAdd += 0.15f * n, Dmg),
            Cond("FirstAidKit", "Erste-Hilfe-Kasten", T1, "⊕", "Regeneration bei wenig Leben.",
                $"Unter {H("50% Leben")}: {H("+4 LP/s")} {S("+4 LP/s pro Stack")} Regeneration.",
                b => b.healthComponent.combinedHealthFraction < 0.5f, (a, n) => a.baseRegenAdd += 4f * n, Heal),
            Cond("CalmMind", "Ruhiger Geist", T1, "☯", "Schneller außerhalb des Kampfes.",
                $"Außerhalb des Kampfes: {U("+10%")} {S("+10% pro Stack")} Bewegungsgeschwindigkeit.",
                b => b.outOfCombat, (a, n) => a.moveSpeedMultAdd += 0.1f * n, UtilT),
            Cond("GuardianAngel", "Kleiner Schutzengel", T1, "☥", "Mehr Rüstung bei wenig Leben.",
                $"Unter {H("25% Leben")}: {H("+20")} {S("+20 pro Stack")} Rüstung.",
                b => b.healthComponent.combinedHealthFraction < 0.25f, (a, n) => a.armorAdd += 20f * n, UtilT),
            Cond("WarmScarf", "Warmer Schal", T1, "∾", "Regeneration außerhalb des Kampfes.",
                $"Außerhalb des Kampfes: {H("+3 LP/s")} {S("+3 LP/s pro Stack")} Regeneration.",
                b => b.outOfCombat, (a, n) => a.baseRegenAdd += 3f * n, Heal),

            Pulse("SparkPlug", "Zündkerze", T1, "⚡", "Zappt regelmäßig einen Gegner in der Nähe.",
                $"Alle {U("3 s")} trifft ein Blitz den nächsten Gegner im Umkreis von 20 m für {D("100%")} {S("+100% pro Stack")} Basisschaden.", 3f,
                (b, n) => { var t = Fx.Enemies(b, b.corePosition, 20f, 1); if (t.Count > 0) Fx.Lightning(b, b.corePosition, t[0], b.damage * n); }, Dmg),
            Pulse("MedicinalHerbs", "Heilkräuter", T1, "☘", "Heilt dich regelmäßig.",
                $"Alle {U("5 s")} heilst du dich um {H("2%")} {S("+2% pro Stack")} deines maximalen Lebens.", 5f,
                (b, n) => Fx.HealFraction(b, 0.02f * n), Heal),
        };

        // ===================================================== Selten (grün)
        private static IEnumerable<ModItem> Green() => new[]
        {
            Stat("HourglassShard", "Sanduhrsplitter", T2, "∑", "Verringert alle Abklingzeiten.",
                $"Verringert alle Abklingzeiten um {U("8%")} {S("+8% pro Stack, multiplikativ")}.",
                (a, n) => a.cooldownMultAdd -= 1f - Mult(0.92f, n), UtilT),
            Stat("ShieldBattery", "Schildbatterie", T2, "◫", "Erhalte einen aufladbaren Schild.",
                $"Erhalte einen {H("Schild")} mit {H("40")} {S("+40 pro Stack")} Punkten.",
                (a, n) => a.baseShieldAdd += 40f * n, UtilT),
            Stat("BerserkerTooth", "Berserkerzahn", T2, "▼", "Mehr Schaden und Tempo, aber weniger Rüstung.",
                $"Erhöht den Schaden um {D("15%")} und die Angriffsgeschwindigkeit um {D("12%")} {S("je pro Stack")}. Verringert die Rüstung um {X("5")} {S("+5 pro Stack")}.",
                (a, n) => { a.damageMultAdd += 0.15f * n; a.attackSpeedMultAdd += 0.12f * n; a.armorAdd -= 5f * n; }, Dmg),
            Stat("KnightsArmor", "Ritterrüstung", T2, "▩", "Viel Rüstung, etwas langsamer.",
                $"Erhöht die Rüstung um {H("15")} {S("+15 pro Stack")}. Verringert die Bewegungsgeschwindigkeit um {X("4%")} {S("+4% pro Stack")}.",
                (a, n) => { a.armorAdd += 15f * n; a.moveSpeedReductionMultAdd += 0.04f * n; }, UtilT),
            Stat("FencersFoil", "Fechtdegen", T2, "†", "Krit und Angriffstempo.",
                $"Erhöht die kritische Trefferchance um {D("10%")} und die Angriffsgeschwindigkeit um {D("10%")} {S("je pro Stack")}.",
                (a, n) => { a.critAdd += 10f * n; a.attackSpeedMultAdd += 0.1f * n; }, Dmg),
            Stat("SecondHeart", "Zweites Herz", T2, "♡", "Viel mehr Leben und Regeneration.",
                $"Erhöht das maximale Leben um {H("60")} und die Regeneration um {H("1 LP/s")} {S("je pro Stack")}.",
                (a, n) => { a.baseHealthAdd += 60f * n; a.baseRegenAdd += 1f * n; }, Heal),
            Stat("SpringCoil", "Sprungfeder", T2, "§", "Zusätzlicher Sprung.",
                $"Erhalte {U("+1")} {S("+1 pro Stack")} Sprung in der Luft.",
                (a, n) => a.jumpCountAdd += n, UtilT),
            Stat("FourLeafClover", "Vierblättriges Kleeblatt", T2, "♧", "Mehr Glück.",
                $"Alle zufälligen Effekte werden {U("einmal")} {S("+1 pro Stack")} zusätzlich zu deinen Gunsten gewürfelt.",
                (a, n) => a.luckAdd += n, UtilT),
            Stat("RuneOfPower", "Rune der Macht", T2, "Ψ", "Mehr Schaden und kritischer Schaden.",
                $"Erhöht den Schaden um {D("10%")} und den kritischen Schaden um {D("20%")} {S("je pro Stack")}.",
                (a, n) => { a.damageMultAdd += 0.1f * n; a.critDamageMultAdd += 0.2f * n; }, Dmg),
            Stat("OverclockedChip", "Übertakteter Chip", T2, "▤", "Kürzere Abklingzeit für den Spezial-Skill.",
                $"Verringert die Abklingzeit des {U("Spezial-Skills")} um {U("15%")} {S("+15% pro Stack, multiplikativ")}.",
                (a, n) => a.specialCooldownMultAdd -= 1f - Mult(0.85f, n), UtilT),

            Hit("EchoBlade", "Echoklinge", T2, "⚔", "Chance, Treffer zu wiederholen.",
                $"{D("10%")} {S("+10% pro Stack")} Chance, dass ein Treffer seinen Schaden {D("noch einmal")} verursacht.", 100f,
                (a, v, r, n) => { if (Util.CheckRoll(Mathf.Min(100f, 10f * n) * Proc(r), a.master)) Fx.Damage(a, v, r.damageDealt, r.damageInfo.crit); }, Dmg),
            Hit("SerratedEdge", "Sägezahnklinge", T2, "⋚", "Chance auf Blutung.",
                $"{D("15%")} {S("+15% pro Stack")} Chance, Gegner {D("bluten")} zu lassen.", 100f,
                (a, v, r, n) => { if (Util.CheckRoll(Mathf.Min(100f, 15f * n) * Proc(r), a.master)) Fx.Dot(a, v, DotController.DotIndex.Bleed, 3f); }, Dmg),
            Hit("Firebrand", "Brandeisen", T2, "♨", "Chance, Gegner stark zu verbrennen.",
                $"{D("15%")} Chance, Gegner für 4 s {D("in Brand")} zu setzen {S("+100% Brandschaden pro Stack")}.", 15f,
                (a, v, r, n) => Fx.Dot(a, v, DotController.DotIndex.Burn, 4f, n), Dmg),
            Hit("ChainBracer", "Kettenarmreif", T2, "∞", "Chance auf einen Kettenblitz.",
                $"{D("15%")} Chance auf einen Kettenblitz für {D("80%")} Gesamtschaden, der {D("2")} {S("+1 pro Stack")} Mal überspringt.", 15f,
                (a, v, r, n) => Fx.Lightning(a, r.damageInfo.position, Fx.HurtBoxOf(v), r.damageInfo.damage * 0.8f, 1 + n), Dmg),
            Hit("SeekerRounds", "Sucherpatronen", T2, "➶", "Chance, eine Rakete abzufeuern.",
                $"{D("12%")} Chance, eine Rakete für {D("150%")} {S("+150% pro Stack")} Gesamtschaden abzufeuern.", 12f,
                (a, v, r, n) => Fx.Missile(a, Fx.HurtBoxOf(v), r.damageInfo.damage * 1.5f * n), Dmg),
            Hit("Concussion", "Erschütterung", T2, "◉", "Chance, Gegner zu betäuben.",
                $"{U("6%")} {S("+6% pro Stack")} Chance, Gegner für 1 s zu {U("betäuben")}.", 100f,
                (a, v, r, n) => { if (Util.CheckRoll(Fx.Hyperbolic(0.064f, n) * 100f * Proc(r), a.master)) Fx.Damage(a, v, 0.01f, false, DamageType.Stun1s); }, UtilT),
            Hit("GroundingWire", "Erdungskabel", T2, "⊥", "Chance, Gegner zu schwächen.",
                $"{U("10%")} Chance, Gegner für {U("3 s")} {S("+2 s pro Stack")} zu {U("verkrüppeln")} (weniger Rüstung und Tempo).", 10f,
                (a, v, r, n) => Fx.Buff(v, RoR2Content.Buffs.Cripple, 1f + 2f * n), UtilT),
            Hit("BindingChain", "Fesselkette", T2, "∝", "Chance, Gegner festzuhalten.",
                $"{U("8%")} Chance, Gegner für {U("1 s")} {S("+0.5 s pro Stack")} an Ort und Stelle zu {U("fesseln")}.", 8f,
                (a, v, r, n) => Fx.Buff(v, RoR2Content.Buffs.Entangle, 0.5f + 0.5f * n), UtilT),

            Kill("ProspectorsPick", "Goldgräberhacke", T2, "⚒", "Kills geben zusätzliches Gold.",
                $"Jeder Kill gibt {U("2 Gold")} {S("+2 pro Stack")}. Skaliert mit der Zeit.", 100f,
                (k, r, n) => Fx.Gold(k, 2f * n), ItemTag.Utility, ItemTag.OnKillEffect, ItemTag.AIBlacklist),
            Kill("SoulCollector", "Seelensammler", T2, "☉", "Kills heilen dich.",
                $"Jeder Kill {H("heilt")} dich um {H("4%")} {S("+4% pro Stack")} deines maximalen Lebens.", 100f,
                (k, r, n) => Fx.HealFraction(k, 0.04f * n), ItemTag.Healing, ItemTag.OnKillEffect),
            Kill("Bloodlust", "Blutrausch", T2, "☄", "Kills machen dich schneller.",
                $"Jeder Kill gibt dir für {U("2 s")} {S("+1 s pro Stack")} {D("+30% Angriffs-")} und {U("Bewegungsgeschwindigkeit")}.", 100f,
                (k, r, n) => Fx.Buff(k, RoR2Content.Buffs.WarCryBuff, 1f + n), ItemTag.Damage, ItemTag.OnKillEffect),
            Kill("ChainReaction", "Kettenreaktion", T2, "✺", "Getötete Gegner explodieren heftig.",
                $"Getötete Gegner explodieren im Umkreis von {D("8 m")} für {D("150%")} {S("+150% pro Stack")} Basisschaden.", 100f,
                (k, r, n) => Fx.Blast(k, r.victimBody ? r.victimBody.corePosition : k.corePosition, 8f, k.damage * 1.5f * n), ItemTag.Damage, ItemTag.OnKillEffect),
            Kill("SoulBattery", "Seelenbatterie", T2, "⌬", "Kills geben Barriere.",
                $"Jeder Kill gibt {H("5%")} {S("+5% pro Stack")} deines maximalen Lebens als {H("Barriere")}.", 100f,
                (k, r, n) => Fx.Barrier(k, k.healthComponent.fullCombinedHealth * 0.05f * n), ItemTag.Healing, ItemTag.OnKillEffect),

            Hurt("MirrorShield", "Spiegelschild", T2, "◐", "Treffer lösen Gegenblitze aus.",
                $"{D("25%")} Chance, nach einem Treffer {D("2")} {S("+1 pro Stack")} Gegner in der Nähe für {D("150%")} Basisschaden zu zappen.", 25f,
                (a, v, r, n) => { foreach (var t in Fx.Enemies(v, v.corePosition, 25f, 1 + n)) Fx.Lightning(v, v.corePosition, t, v.damage * 1.5f); }, Dmg),
            Hurt("EmergencyBarrier", "Notfallbarriere", T2, "⬡", "Chance auf Barriere nach Treffern.",
                $"{H("20%")} Chance, nach einem Treffer {H("10%")} {S("+10% pro Stack")} deines maximalen Lebens als {H("Barriere")} zu erhalten.", 20f,
                (a, v, r, n) => Fx.Barrier(v, v.healthComponent.fullCombinedHealth * 0.1f * n), Heal),
            Hurt("ExplosiveVest", "Sprengweste", T2, "✹", "Chance, nach Treffern zu explodieren.",
                $"{D("15%")} Chance, nach einem Treffer im Umkreis von {D("8 m")} für {D("250%")} {S("+250% pro Stack")} Basisschaden zu explodieren.", 15f,
                (a, v, r, n) => Fx.Blast(v, v.corePosition, 8f, v.damage * 2.5f * n, false, true), Dmg),

            Cond("WarDrum", "Kriegstrommel", T2, "◎", "Mehr Schaden bei vollem Leben.",
                $"Über {H("90% Leben")}: {D("+20%")} {S("+20% pro Stack")} Schaden.",
                b => b.healthComponent.combinedHealthFraction >= 0.9f, (a, n) => a.damageMultAdd += 0.2f * n, Dmg),
            Cond("LastStand", "Letztes Gefecht", T2, "⚑", "Stärker bei wenig Leben.",
                $"Unter {H("35% Leben")}: {D("+40%")} Angriffsgeschwindigkeit und {H("+30")} Rüstung {S("je pro Stack")}.",
                b => b.healthComponent.combinedHealthFraction < 0.35f, (a, n) => { a.attackSpeedMultAdd += 0.4f * n; a.armorAdd += 30f * n; }, Dmg),
            Cond("CloudWalkers", "Wolkenläufer", T2, "☁", "Schneller in der Luft.",
                $"In der {U("Luft")}: {U("+30%")} Bewegungs- und {D("+20%")} Angriffsgeschwindigkeit {S("je pro Stack")}.",
                b => b.characterMotor && !b.characterMotor.isGrounded, (a, n) => { a.moveSpeedMultAdd += 0.3f * n; a.attackSpeedMultAdd += 0.2f * n; }, UtilT),
            Cond("ShieldCore", "Schildkern", T2, "⊗", "Mehr Schaden, solange du Schild hast.",
                $"Solange du {H("Schild")} hast: {D("+15%")} {S("+15% pro Stack")} Schaden.",
                b => b.healthComponent.shield > 0f, (a, n) => a.damageMultAdd += 0.15f * n, Dmg),
            Cond("BattleHymn", "Kampfhymne", T2, "♫", "Schneller angreifen im Kampf.",
                $"Im Kampf: {D("+15%")} {S("+15% pro Stack")} Angriffsgeschwindigkeit.",
                b => !b.outOfCombat, (a, n) => a.attackSpeedMultAdd += 0.15f * n, Dmg),

            Pulse("TeslaCoil", "Teslaspule", T2, "⚡", "Zappt ständig Gegner in der Nähe.",
                $"Jede Sekunde treffen Blitze {D("2")} {S("+1 pro Stack")} Gegner im Umkreis von 25 m für {D("80%")} Basisschaden.", 1f,
                (b, n) => { foreach (var t in Fx.Enemies(b, b.corePosition, 25f, 1 + n)) Fx.Lightning(b, b.corePosition, t, b.damage * 0.8f); }, Dmg),
            Pulse("HealingTotem", "Heiltotem", T2, "⚚", "Heilt regelmäßig dich und Verbündete.",
                $"Alle {U("4 s")} werden du und Verbündete im Umkreis von 20 m um {H("4%")} {S("+4% pro Stack")} geheilt.", 4f,
                (b, n) => { foreach (var ally in Fx.Allies(b, 20f)) Fx.HealFraction(ally, 0.04f * n); }, Heal),
            Pulse("MissilePod", "Raketenwerfer", T2, "➹", "Feuert regelmäßig Raketen.",
                $"Alle {U("3 s")} feuerst du {D("1")} {S("+1 pro Stack")} Rakete auf Gegner in der Nähe für {D("200%")} Basisschaden.", 3f,
                (b, n) => { var t = Fx.Enemies(b, b.corePosition, 40f, 3); for (int i = 0; i < n && t.Count > 0; i++) Fx.Missile(b, t[i % t.Count], b.damage * 2f); }, Dmg),
            Pulse("MedicPouch", "Medizinbeutel", T2, "⊞", "Regelmäßig Barriere.",
                $"Alle {U("10 s")} erhältst du {H("10%")} {S("+10% pro Stack")} deines maximalen Lebens als {H("Barriere")}.", 10f,
                (b, n) => Fx.Barrier(b, b.healthComponent.fullCombinedHealth * 0.1f * n), Heal),
        };

        // ===================================================== Legendär (rot)
        private static IEnumerable<ModItem> Red() => new[]
        {
            Stat("TitansHeart", "Herz des Titanen", T3, "❤", "Massiv mehr Leben.",
                $"Erhöht das maximale Leben um {H("60%")} {S("+60% pro Stack")}.",
                (a, n) => a.healthMultAdd += 0.6f * n, Heal),
            Stat("StormCrown", "Krone des Sturms", T3, "♛", "Massiv schneller angreifen und laufen.",
                $"Erhöht Angriffs- und Bewegungsgeschwindigkeit um je {U("30%")} {S("+30% pro Stack")}.",
                (a, n) => { a.attackSpeedMultAdd += 0.3f * n; a.moveSpeedMultAdd += 0.3f * n; }, UtilT),
            Stat("Juggernaut", "Juggernaut", T3, "■", "Rüstung und Leben satt.",
                $"Erhöht die Rüstung um {H("40")} und das maximale Leben um {H("40%")} {S("je pro Stack")}.",
                (a, n) => { a.armorAdd += 40f * n; a.healthMultAdd += 0.4f * n; }, UtilT),
            Stat("EternalAmmo", "Ewige Munition", T3, "∞", "Deutlich kürzere Abklingzeiten.",
                $"Verringert alle Abklingzeiten um {U("25%")} {S("+25% pro Stack, multiplikativ")}.",
                (a, n) => a.cooldownMultAdd -= 1f - Mult(0.75f, n), UtilT),
            Stat("AssassinsMark", "Mal des Assassinen", T3, "☠", "Kritische Treffer werden verheerend.",
                $"Erhöht die kritische Trefferchance um {D("25%")} und den kritischen Schaden um {D("50%")} {S("je pro Stack")}.",
                (a, n) => { a.critAdd += 25f * n; a.critDamageMultAdd += 0.5f * n; }, Dmg),

            Hit("ExecutionersAxe", "Henkersbeil", T3, "⚔", "Richte geschwächte Gegner sofort hin.",
                $"Treffer {D("töten")} Gegner (keine Bosse) mit weniger als {D("15%")} {S("+15% pro Stack, hyperbolisch")} Leben sofort.", 100f,
                (a, v, r, n) => { if (!v.isBoss && v.healthComponent.alive && v.healthComponent.combinedHealthFraction < Fx.Hyperbolic(0.1765f, n)) Fx.Damage(a, v, v.healthComponent.fullCombinedHealth); }, Dmg),
            Hit("ThunderGod", "Hammer des Donnergotts", T3, "⚡", "Chance, Blitzschläge herabzurufen.",
                $"{D("20%")} Chance, einen Blitz einschlagen zu lassen: {D("500%")} {S("+500% pro Stack")} Basisschaden im Umkreis von 6 m.", 20f,
                (a, v, r, n) => { Fx.LightningStrike(v.corePosition); Fx.Blast(a, v.corePosition, 6f, a.damage * 5f * n); }, Dmg),
            Hit("VampireLord", "Vampirfürst", T3, "♥", "Heile dich um viel Schaden.",
                $"Treffer {H("heilen")} dich um {H("8%")} {S("+8% pro Stack")} des verursachten Schadens.", 100f,
                (a, v, r, n) => Fx.Heal(a, r.damageDealt * 0.08f * n * Proc(r)), Heal),
            Hit("DeathWarrant", "Todesurteil", T3, "⚰", "Chance, Gegner zu markieren.",
                $"{D("15%")} Chance, Gegner für {D("7 s")} {S("+3 s pro Stack")} zu {D("markieren")}: Sie erleiden {D("50%")} mehr Schaden.", 15f,
                (a, v, r, n) => Fx.Buff(v, RoR2Content.Buffs.DeathMark, 4f + 3f * n), Dmg),
            Hit("EternalFlame", "Ewiges Feuer", T3, "♨", "Alle Treffer können verbrennen.",
                $"{D("30%")} Chance, Gegner für 4 s mit {D("200%")} {S("+200% pro Stack")} Brandschaden {D("in Brand")} zu setzen.", 30f,
                (a, v, r, n) => Fx.Dot(a, v, DotController.DotIndex.Burn, 4f, 2f * n), Dmg),

            Kill("MidasHand", "Midashand", T3, "⚜", "Kills geben viel Gold.",
                $"Jeder Kill gibt {U("10 Gold")} {S("+10 pro Stack")}. Skaliert mit der Zeit.", 100f,
                (k, r, n) => Fx.Gold(k, 10f * n), ItemTag.Utility, ItemTag.OnKillEffect, ItemTag.AIBlacklist),

            Pulse("Supernova", "Supernova", T3, "☀", "Explodiert regelmäßig.",
                $"Alle {U("8 s")} explodierst du im Umkreis von {D("25 m")} für {D("600%")} {S("+600% pro Stack")} Basisschaden.", 8f,
                (b, n) => { if (Fx.Enemies(b, b.corePosition, 25f, 1).Count > 0) Fx.Blast(b, b.corePosition, 25f, b.damage * 6f * n); }, Dmg),
            Pulse("PhoenixFeather", "Phönixfeder", T3, "♆", "Rettet dich bei wenig Leben.",
                $"Fällst du unter {H("25% Leben")}, wirst du um {H("50%")} geheilt. Abklingzeit {U("60 s")} {S("-25% pro Stack")}.", 0.2f,
                (b, n) =>
                {
                    phoenixReady.TryGetValue(b, out float ready);
                    if (Time.fixedTime < ready || b.healthComponent.combinedHealthFraction >= 0.25f)
                        return;
                    Fx.HealFraction(b, 0.5f);
                    Fx.Blast(b, b.corePosition, 10f, b.damage * 3f, false, true);
                    phoenixReady[b] = Time.fixedTime + 60f * Mult(0.75f, n - 1);
                }, Heal),
            Pulse("StormRunner", "Sturmläufer", T3, "≋", "Blitze beim Sprinten.",
                $"Beim {U("Sprinten")} trifft alle {U("0.5 s")} ein Blitz einen Gegner im Umkreis von 25 m für {D("150%")} {S("+150% pro Stack")} Basisschaden.", 0.5f,
                (b, n) => { if (!b.isSprinting) return; var t = Fx.Enemies(b, b.corePosition, 25f, 1); if (t.Count > 0) Fx.Lightning(b, b.corePosition, t[0], b.damage * 1.5f * n); }, Dmg),
        };

        // ===================================================== Boss (gelb)
        private static IEnumerable<ModItem> Yellow() => new[]
        {
            Stat("GolemCore", "Golemkern", Boss, "⬢", "Leben und Rüstung.",
                $"Erhöht das maximale Leben um {H("25%")} und die Rüstung um {H("10")} {S("je pro Stack")}.",
                (a, n) => { a.healthMultAdd += 0.25f * n; a.armorAdd += 10f * n; }, UtilT),
            Stat("AurelioniteShard", "Aurelionit-Splitter", Boss, "◆", "Schaden und Rüstung.",
                $"Erhöht den Schaden um {D("20%")} und die Rüstung um {H("20")} {S("je pro Stack")}.",
                (a, n) => { a.damageMultAdd += 0.2f * n; a.armorAdd += 20f * n; }, Dmg),
            Hit("WispEmber", "Irrlichtglut", Boss, "✧", "Chance auf Feuerexplosionen.",
                $"{D("10%")} Chance auf eine Feuerexplosion im Umkreis von 5 m für {D("250%")} {S("+250% pro Stack")} Basisschaden.", 10f,
                (a, v, r, n) => Fx.Blast(a, v.corePosition, 5f, a.damage * 2.5f * n, false, true), Dmg),
            Hit("TitanFist", "Titanenfaust", Boss, "Ω", "Chance auf einen gewaltigen Schlag.",
                $"{D("5%")} Chance, Gegner mit einer Steinfaust zu treffen: {D("400%")} {S("+400% pro Stack")} Basisschaden im Umkreis von 8 m.", 5f,
                (a, v, r, n) => Fx.Blast(a, v.corePosition, 8f, a.damage * 4f * n), Dmg),
            Kill("BeetleCrown", "Käferkrone", Boss, "♚", "Kills rufen Schwarmkraft.",
                $"Jeder Kill gibt dir für {U("3 s")} {S("+1 s pro Stack")} {D("Energie")} (schnellere Angriffe).", 100f,
                (k, r, n) => Fx.Buff(k, RoR2Content.Buffs.Energized, 2f + n), ItemTag.Damage, ItemTag.OnKillEffect),
            Pulse("ScavengerSack", "Plünderersack", Boss, "⚱", "Regelmäßig Gold.",
                $"Alle {U("30 s")} erhältst du {U("25 Gold")} {S("+25 pro Stack")}. Skaliert mit der Zeit.", 30f,
                (b, n) => Fx.Gold(b, 25f * n), ItemTag.Utility, ItemTag.AIBlacklist),
        };

        // ===================================================== Mond (blau) – starke Effekte mit Nachteil
        private static IEnumerable<ModItem> Blue() => new[]
        {
            Stat("GlassCannon", "Glaskanone", Lunar, "◇", "Doppelter Schaden... halbes Leben.",
                $"Erhöht den Schaden um {D("100%")} {S("+100% pro Stack")}. {X("Halbiert")} dein maximales Leben {S("pro Stack")}.",
                (a, n) => { a.damageMultAdd += 1f * n; a.healthTotalMult *= Mult(0.5f, n); }, Dmg),
            Hit("BloodPact", "Blutpakt", Lunar, "♰", "Lebensraub... ohne Regeneration.",
                $"Treffer {H("heilen")} dich um {H("4%")} {S("+4% pro Stack")} des Schadens. Deine Regeneration wird {X("abgeschaltet")}.", 100f,
                (a, v, r, n) => Fx.Heal(a, r.damageDealt * 0.04f * n * Proc(r)), Heal),
            Stat("BerserkersOath", "Berserkerschwur", Lunar, "⚔", "Rasende Angriffe... ohne Deckung.",
                $"Erhöht die Angriffsgeschwindigkeit um {D("50%")} {S("+50% pro Stack")}. Verringert die Rüstung um {X("25")} {S("+25 pro Stack")}.",
                (a, n) => { a.attackSpeedMultAdd += 0.5f * n; a.armorAdd -= 25f * n; }, Dmg),
            Stat("HastyRhythm", "Hektischer Rhythmus", Lunar, "♪", "Halbe Abklingzeiten... weniger Schaden.",
                $"Halbiert alle {U("Abklingzeiten")} {S("pro Stack")}. Verringert den Schaden um {X("25%")} {S("multiplikativ pro Stack")}.",
                (a, n) => { a.cooldownMultAdd -= 1f - Mult(0.5f, n); a.damageTotalMult *= Mult(0.75f, n); }, UtilT),
            Stat("GiantsGrowth", "Riesenwuchs", Lunar, "▲", "Werde riesig.",
                $"Verdoppelt dein maximales Leben {S("pro Stack")} und macht dich {U("größer")}. Verringert die Bewegungsgeschwindigkeit um {X("20%")} {S("multiplikativ pro Stack")}.",
                (a, n) => { a.healthTotalMult *= Mult(2f, n); a.desiredScaleAdd += 0.3f * n; a.moveSpeedTotalMult *= Mult(0.8f, n); }, Heal),
            Stat("IcarusFeather", "Ikarusfeder", Lunar, "⇈", "Fliege fast... aber zerbrechlich.",
                $"Erhalte {U("+2")} {S("+2 pro Stack")} Sprünge und {U("+30%")} Sprungkraft. Verringert das maximale Leben um {X("20%")} {S("multiplikativ pro Stack")}.",
                (a, n) => { a.jumpCountAdd += 2 * n; a.jumpPowerMultAdd += 0.3f * n; a.healthTotalMult *= Mult(0.8f, n); }, UtilT),
            Kill("CursedCoin", "Verfluchte Münze", Lunar, "¤", "Mehr Gold... mehr Schmerz.",
                $"Jeder Kill gibt {U("5 Gold")} {S("+5 pro Stack")}. Verringert die Rüstung um {X("15")} {S("+15 pro Stack")}.", 100f,
                (k, r, n) => Fx.Gold(k, 5f * n), ItemTag.Utility, ItemTag.AIBlacklist),
            Stat("BrittleTime", "Brüchige Zeit", Lunar, "⌘", "Schneller als die Zeit... aber zerbrechlich.",
                $"Erhöht Angriffs- und Bewegungsgeschwindigkeit um {U("40%")} {S("+40% pro Stack")}. Verringert das maximale Leben um {X("40%")} {S("multiplikativ pro Stack")}.",
                (a, n) => { a.attackSpeedMultAdd += 0.4f * n; a.moveSpeedMultAdd += 0.4f * n; a.healthTotalMult *= Mult(0.6f, n); }, UtilT),
        };

        // Nachteile von Items, die nicht über ihren eigenen Typ abgebildet werden.
        public static void ApplyDrawbacks(CharacterBody body, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (!body.inventory)
                return;

            if (bloodPact != null && bloodPact.Count(body) > 0)
                args.regenTotalMult = 0f;
            if (cursedCoin != null)
                args.armorAdd -= 15f * cursedCoin.Count(body);
        }

        private static ModItem bloodPact, cursedCoin;

        public static void HookDrawbacks()
        {
            bloodPact = ItemSystem.Registered.FirstOrDefault(i => i.Name == "BloodPact");
            cursedCoin = ItemSystem.Registered.FirstOrDefault(i => i.Name == "CursedCoin");
            R2API.RecalculateStatsAPI.GetStatCoefficients += ApplyDrawbacks;
        }
    }
}
