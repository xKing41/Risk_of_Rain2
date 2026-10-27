using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoreItems.Items
{
    // Grundlage für alle Items: Definition, Texte und Registrierung bei R2API.
    public abstract class ItemBase
    {
        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract string Pickup { get; }
        public abstract string Description { get; }
        public virtual string Lore => "";
        public abstract ItemTier Tier { get; }
        public virtual ItemTag[] Tags => new ItemTag[0];

        public ItemDef ItemDef { get; private set; }

        public void Register()
        {
            string token = "MOREITEMS_" + Name.ToUpperInvariant();

            ItemDef = ScriptableObject.CreateInstance<ItemDef>();
            ItemDef.name = "MoreItems" + Name;
            ItemDef.nameToken = token + "_NAME";
            ItemDef.pickupToken = token + "_PICKUP";
            ItemDef.descriptionToken = token + "_DESC";
            ItemDef.loreToken = token + "_LORE";
#pragma warning disable CS0618 // deprecatedTier ist der von R2API unterstützte Weg, die Seltenheit zu setzen
            ItemDef.deprecatedTier = Tier;
#pragma warning restore CS0618
            ItemDef.tags = Tags;
            ItemDef.canRemove = true;
            ItemDef.hidden = false;
            // Platzhalter, bis wir eigene Icons und Modelle haben
            ItemDef.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            ItemDef.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

            LanguageAPI.Add(ItemDef.nameToken, DisplayName);
            LanguageAPI.Add(ItemDef.pickupToken, Pickup);
            LanguageAPI.Add(ItemDef.descriptionToken, Description);
            LanguageAPI.Add(ItemDef.loreToken, Lore);

            ItemAPI.Add(new CustomItem(ItemDef, new ItemDisplayRuleDict(null)));
            Hooks();
        }

        // Hier hängt sich jedes Item in die Spiel-Events ein.
        protected abstract void Hooks();

        protected int GetCount(CharacterBody body)
        {
            return body && body.inventory ? body.inventory.GetItemCount(ItemDef) : 0;
        }

        // Gleiche Formel wie bei vanilla-Items: nähert sich 1 an, erreicht es aber nie.
        protected static float Hyperbolic(float perStack, int count)
        {
            return 1f - 1f / (1f + perStack * count);
        }
    }
}
