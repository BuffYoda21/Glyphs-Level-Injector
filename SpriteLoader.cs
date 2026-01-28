using System.IO;
using MelonLoader;
using UnityEngine;

namespace LevelInjector {
    public static class SpriteLoader {
        public static Sprite LoadSpriteFromFile(string path, int width, int height) {
            Texture2D tex = new Texture2D(
                width,
                height,
                TextureFormat.RGBA32,
                false
            );
            string[] splitPath = path.Split(Path.DirectorySeparatorChar);
            tex.name = splitPath[splitPath.Length - 1];
            tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            if (!File.Exists(path)) {
                MelonLogger.Error($"{path} does not exist!");
                return null;
            }

            byte[] raw = File.ReadAllBytes(path);
            //MelonLogger.Msg($"Raw length = {raw.Length}, expected = {width * height * 4}");
            if (raw.Length != width * height * 4) {
                MelonLogger.Warning($"Image dimensions do not match expected file size!");
                MelonLogger.Warning($"Expected {width * height * 4} bytes, got {raw.Length} bytes");
                MelonLogger.Warning($"This may cause the loaded image to be distorted or deformed");
            }


            int i = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    float r = raw[i++] / 255f;
                    float g = raw[i++] / 255f;
                    float b = raw[i++] / 255f;
                    float a = raw[i++] / 255f;

                    tex.SetPixel(x, height - 1 - y, new Color(r, g, b, a));
                }
            }

            tex.Apply(false, false);

            return Sprite.Create(
                tex,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f),
                8f
            );
        }
    }
}