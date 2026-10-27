using System;
using R2API;
using RoR2;
using UnityEngine;

namespace MoreItems.Items
{
    public enum Kind { Stat, OnHit, OnKill, OnHurt, Conditional, Pulse }

    // Ein Item = ein Eintrag in ItemList. Welche Felder genutzt werden, hängt von Kind ab.
    public class ModItem
    {
        public string Name;
        public string DisplayName;
        public ItemTier Tier;
        public string Symbol;
        public string Pickup;
        public string Description;
        public ItemTag[] Tags = new ItemTag[0];
        public Kind Kind;

        // Stat und Conditional: Werte pro Stack ändern.
        public Action<CharacterBody, RecalculateStatsAPI.StatHookEventArgs, int> Stats;

        // OnHit: Basis-Chance in Prozent (wird mit procCoefficient und Glück verrechnet; 100 = immer).
        public float Chance = 100f;
        // OnHit/OnHurt: (Angreifer, Opfer, Report, Stacks). OnHurt: Angreifer ist der Gegner, Opfer der Itemträger.
        public Action<CharacterBody, CharacterBody, DamageReport, int> OnDamage;
        // OnKill: (Killer, Report, Stacks)
        public Action<CharacterBody, DamageReport, int> OnKill;

        // Conditional: Item wirkt nur, solange die Bedingung gilt (wird auf dem Server geprüft und als Buff angezeigt).
        public Func<CharacterBody, bool> Condition;
        public BuffDef ConditionBuff;

        // Pulse: alle Interval Sekunden auf dem Server ausführen.
        public float Interval = 1f;
        public Action<CharacterBody, int> OnPulse;

        public ItemDef ItemDef;
        public Sprite Icon;

        public int Count(CharacterBody body)
        {
            return body && body.inventory && ItemDef ? body.inventory.GetItemCount(ItemDef) : 0;
        }
    }
}
