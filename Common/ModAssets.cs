using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ModCommon
{
    public static class ModAssets
    {
        // Lädt ein Spiel-Asset über seinen Addressables-Schlüssel (GUID aus RoR2BepInExPack.GameAssetPathsBetter).
        // Gibt null zurück statt abzustürzen, falls ein Update den Schlüssel entfernt hat.
        public static T Load<T>(string key) where T : UnityEngine.Object
        {
            try
            {
                return Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ModCommon] Asset {key} ({typeof(T).Name}) nicht ladbar: {e.Message}");
                return null;
            }
        }

        // Lädt ein PNG, das als EmbeddedResource in der Mod-DLL steckt (Ordner "Icons").
        public static Sprite LoadIcon(Assembly assembly, string name)
        {
            string resource = assembly.GetName().Name + ".Icons." + name + ".png";
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                {
                    Debug.LogWarning($"[ModCommon] Icon {resource} fehlt.");
                    return null;
                }

                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                texture.LoadImage(bytes);
                texture.wrapMode = TextureWrapMode.Clamp;
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }
}
