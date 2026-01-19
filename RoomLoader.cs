using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace LevelInjector {
    public static class RoomLoader {
        public static GameObject CreateRoom(string path, Transform parent, Vector2 offset) {
            GameObject room = LoadRoomData(path);
            if (room == null) return null;
            room.transform.SetParent(parent);
            room.transform.localPosition = offset;
            return room;
        }

        public static GameObject LoadRoomData(string path) {
            path = Path.Combine(MelonEnvironment.ModsDirectory, "CustomRooms", path);

            if (!File.Exists(path)) {
                MelonLogger.Error($"[RoomLoader] File not found: {path}");
                return null;
            }

            string json = File.ReadAllText(path);

            RoomData roomData;
            try {
                roomData = JsonConvert.DeserializeObject<RoomData>(json);
            } catch (JsonException e) {
                MelonLogger.Error($"[RoomLoader] JSON parse error:\n{e}");
                return null;
            }

            if (roomData == null) {
                MelonLogger.Error("[RoomLoader] RoomData is null");
                return null;
            }

            GameObject roomParent = new GameObject(
                string.IsNullOrEmpty(roomData.RoomName)
                    ? "Room"
                    : roomData.RoomName
            );

            if (roomData.Tiles == null)
                return roomParent;

            foreach (TileData tile in roomData.Tiles) {
                SpawnTile(tile, roomParent.transform);
            }

            return roomParent;
        }

        private static void SpawnTile(TileData data, Transform parent) {
            GameObject tile = Object.Instantiate(new GameObject("Tile"), parent);

            BoxCollider2D col = tile.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1f, 1f);
            col.isTrigger = false;

            SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
            if (!squareSprite)
                sr.sprite = CreateSquareSprite();
            else
                sr.sprite = squareSprite;

            tile.transform.localPosition = new Vector3(
                data.Position.X,
                data.Position.Y,
                0f
            );

            tile.transform.localScale = new Vector3(
                data.Scale.X,
                data.Scale.Y,
                1f
            );

            if (data.Color != null) {
                sr.color = new Color32(
                    data.Color.R,
                    data.Color.G,
                    data.Color.B,
                    data.Color.A
                );
            }
        }

        private static Sprite CreateSquareSprite() {
            if (squareSprite) return squareSprite;

            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            squareSprite = Sprite.Create(
                tex,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f),
                1f
            );
            squareSprite.name = "Square";
            return squareSprite;
        }

        private static Sprite squareSprite;
    }
}