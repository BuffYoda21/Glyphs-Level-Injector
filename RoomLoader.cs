using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace LevelInjector {
    [HarmonyPatch]
    public static class RoomLoader {
        public static void LoadRooms(string sceneName) {
            string rootPath = Path.Combine(MelonEnvironment.ModsDirectory, "CustomRooms", sceneName);
            if (!Directory.Exists(rootPath)) return;

            foreach (string jsonPath in Directory.GetFiles(rootPath, "*.json", SearchOption.AllDirectories)
                        .Concat(Directory.GetFiles(rootPath, "*.jsonc", SearchOption.AllDirectories))) {
                LoadRoomFromFile(jsonPath, rootPath);
            }
        }

        [HarmonyPatch(typeof(BetweenManager), "LoadRoomsFromResources")]
        [HarmonyPrefix]
        public static void OnBetweenLoad(BetweenManager __instance) {
            injectedRooms.Clear();

            string rootPath = Path.Combine(MelonEnvironment.ModsDirectory, "CustomRooms", "BetweenRooms");
            if (!Directory.Exists(rootPath)) return;

            foreach (string jsonPath in Directory.GetFiles(rootPath, "*.json", SearchOption.AllDirectories)
                        .Concat(Directory.GetFiles(rootPath, "*.jsonc", SearchOption.AllDirectories))) {
                GameObject room = LoadRoomFromFile(jsonPath, rootPath);
                if (!room || room.name.Length < 2) continue;
                char[] prefix = room.name.ToCharArray();
                if (
                    (prefix[0] != '_' && prefix[0] != '-' && prefix[0] != '^') ||
                    (prefix[1] != '_' && prefix[1] != '-' && prefix[1] != '^')
                ) continue;

                BetweenManager.EntranceType roomEntrance = BetweenManager.EntranceType.Top;
                BetweenManager.ExitType roomExit = BetweenManager.ExitType.Top;
                switch (prefix[0]) {
                    case '_': roomEntrance = BetweenManager.EntranceType.Bottom; break;
                    case '-': roomEntrance = BetweenManager.EntranceType.Middle; break;
                    case '^': roomEntrance = BetweenManager.EntranceType.Top; break;
                }
                switch (prefix[1]) {
                    case '_': roomExit = BetweenManager.ExitType.Bottom; break;
                    case '-': roomExit = BetweenManager.ExitType.Middle; break;
                    case '^': roomExit = BetweenManager.ExitType.Top; break;
                }
                BetweenManager.Room betweenRoom = new BetweenManager.Room() {
                    roomObject = room,
                    entrance = roomEntrance,
                    exit = roomExit,
                };
                __instance.rooms.Add(betweenRoom);
                injectedRooms.Add(room);
            }

            GameObject roomParent = new GameObject("CustomBetweenRooms");
            foreach (GameObject room in injectedRooms) {
                room.SetActive(false);
                room.transform.SetParent(roomParent.transform);
                room.transform.position = new Vector3(-200f, 0f, 0f);
            }
        }

        [HarmonyPatch(typeof(BetweenManager), "InstantiateRoom")]
        [HarmonyPrefix]
        public static void OnInstantiateRoom(BetweenManager.Room room, Vector3 position) {
            bool isCustom = false;
            foreach (GameObject obj in injectedRooms) {
                if (obj.name == room.roomObject.name)
                    isCustom = true;
            }
            if (isCustom) {
                foreach (GameObject obj in injectedRooms) {
                    obj.SetActive(false);
                }
                room.roomObject.SetActive(true);
            }
        }

        private static GameObject LoadRoomFromFile(string jsonPath, string rootPath) {
            string relativePath = Path.GetRelativePath(rootPath, jsonPath);
            string[] pathParts = relativePath.Split(Path.DirectorySeparatorChar);

            GameObject currentParent = null;
            for (int i = 0; i < pathParts.Length - 1; i++) {
                string folderName = pathParts[i];
                GameObject existing = currentParent != null ? currentParent.transform.Find(folderName)?.gameObject : GameObject.Find(folderName);

                if (existing == null) {
                    existing = new GameObject(folderName);
                    if (currentParent != null) existing.transform.SetParent(currentParent.transform);
                }

                currentParent = existing;
            }

            string fileName = Path.GetFileNameWithoutExtension(jsonPath);
            GameObject roomObj = new GameObject(fileName);
            roomObj.transform.SetParent(currentParent?.transform);

            string json = File.ReadAllText(jsonPath);
            RoomData roomData = JsonConvert.DeserializeObject<RoomData>(json);

            if (roomData == null) return null;

            if (roomData.LocalPosition != null) {
                roomObj.transform.localPosition = new Vector3(
                    roomData.LocalPosition.X,
                    roomData.LocalPosition.Y,
                    0f
                );
            }

            CreateSquareSprite();

            if (roomData.Tiles != null) {
                foreach (TileData tile in roomData.Tiles) {
                    SpawnTile(tile, roomObj.transform);
                }
            }

            if (roomData.Bg != null) {
                GameObject bg = new GameObject("bg");
                bg.transform.SetParent(roomObj.transform);

                SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
                sr.color = new Color32(
                    roomData.Bg.Color.R,
                    roomData.Bg.Color.G,
                    roomData.Bg.Color.B,
                    roomData.Bg.Color.A
                );
                sr.sortingOrder = -100;

                if (roomData.Bg.Path == null) {
                    sr.sprite = squareSprite;
                    bg.transform.localScale = new Vector2(28.5f, 16f);
                } else {
                    string dataPath = Path.Combine(MelonEnvironment.ModsDirectory, "CustomRooms", "SpriteData");
                    sr.sprite = SpriteLoader.LoadSpriteFromFile(Path.Combine(dataPath, roomData.Bg.Path), roomData.Bg.ImgSize.Width, roomData.Bg.ImgSize.Height);
                    bg.transform.localScale = new Vector2(roomData.Bg.Scale.X, roomData.Bg.Scale.Y);
                }
            }

            if (roomData.Elements != null) {
                foreach (PrefabData prefab in roomData.Elements) {
                    SpawnPrefab(prefab, roomObj.transform);
                }
            }

            return roomObj;
        }

        private static void SpawnTile(TileData data, Transform parent) {
            GameObject tile = new GameObject("Tile");
            tile.transform.SetParent(parent);
            tile.layer = 3; // tile layer

            BoxCollider2D col = tile.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1f, 1f);
            col.isTrigger = false;

            SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
            if (!squareSprite)
                sr.sprite = CreateSquareSprite();
            else
                sr.sprite = squareSprite;

            tile.transform.localPosition = new Vector2(data.Position.X, data.Position.Y);
            tile.transform.localScale = new Vector3(data.Scale.X, data.Scale.Y, 1f);

            if (data.Color != null) {
                sr.color = new Color32(
                    data.Color.R,
                    data.Color.G,
                    data.Color.B,
                    data.Color.A
                );
            }
        }

        private static void SpawnPrefab(PrefabData data, Transform parent) {
            GameObject reference = Resources.Load<GameObject>(data.PrefabPath);
            if (reference == null) {
                MelonLogger.Warning($"Prefab {data.PrefabPath} not found!");
                return;
            }
            GameObject prefab = Object.Instantiate(reference);

            prefab.name = data.Name;
            if (parent) prefab.transform.SetParent(parent);
            prefab.transform.localPosition = new Vector2(data.Position.X, data.Position.Y);
            prefab.transform.localRotation = Quaternion.Euler(0f, 0f, data.Rotation);
            prefab.transform.localScale = new Vector3(data.Scale.X, data.Scale.Y, 1f);

            if (data.Color != null && prefab.GetComponent<SpriteRenderer>()) {
                prefab.GetComponent<SpriteRenderer>().color = new Color32(
                    data.Color.R,
                    data.Color.G,
                    data.Color.B,
                    data.Color.A
                );
            }

            if (data.SlidingPlatform != null && prefab.GetComponent<SlidingPlatform>()) {
                SlidingPlatform slidingPlatform = prefab.GetComponent<SlidingPlatform>();
                //if (!slidingPlatform) prefab.AddComponent<SlidingPlatform>(); // need to check for depedency components
                slidingPlatform.xv = data.SlidingPlatform.Xvelocity;
                slidingPlatform.yv = data.SlidingPlatform.Yvelocity;
                if (data.SlidingPlatform.IsIce) {
                    GameObject ice = Object.Instantiate(Resources.Load<GameObject>("prefabs/platforming/Ice"));
                    ice.transform.SetParent(prefab.transform);
                    ice.transform.localPosition = new Vector3(0f, 0f, 0f);
                    ice.transform.localScale = new Vector3(10f, 2f, 1f);
                }
            }

            if (data.BouncePlatform != null && prefab.GetComponent<BouncePlatform>()) {
                BouncePlatform bouncePlatform = prefab.GetComponent<BouncePlatform>();
                bouncePlatform.xstrength = data.BouncePlatform.Xstrength;
                bouncePlatform.ystrength = data.BouncePlatform.Ystrength;
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

        private static List<GameObject> injectedRooms = new List<GameObject>();
        private static Sprite squareSprite;
    }
}