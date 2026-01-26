using Newtonsoft.Json;
using System.Collections.Generic;

namespace LevelInjector {
    public class RoomData {
        [JsonProperty("localPosition")]
        public Vec2 LocalPosition;

        [JsonProperty("tiles")]
        public List<TileData> Tiles;

        [JsonProperty("bg")]
        public BgData Bg;

        [JsonProperty("elements")]
        public List<PrefabData> Elements;
    }

    public class TileData {
        [JsonProperty("position")]
        public Vec2 Position;

        [JsonProperty("scale")]
        public Vec2 Scale;

        [JsonProperty("color")]
        public Color32Data Color;
    }

    public class Vec2 {
        [JsonProperty("x")]
        public float X;

        [JsonProperty("y")]
        public float Y;
    }

    public class BgData {
        [JsonProperty("color")]
        public Color32Data Color;

        [JsonProperty("path")]
        public string Path;

        [JsonProperty("imgSize")]
        public Dimensions ImgSize;

        [JsonProperty("scale")]
        public Vec2 Scale;
    }

    public class Dimensions {
        [JsonProperty("width")]
        public int Width;

        [JsonProperty("height")]
        public int Height;
    }

    public class PrefabData {
        [JsonProperty("prefab")]
        public string PrefabPath;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("position")]
        public Vec2 Position;

        [JsonProperty("rotation")]
        public float Rotation;

        [JsonProperty("scale")]
        public Vec2 Scale;

        [JsonProperty("color")]
        public Color32Data Color;

        [JsonProperty("SlidingPlatform")]
        public SlidingPlatformData SlidingPlatform;

        [JsonProperty("BouncePlatform")]
        public BouncePlatformData BouncePlatform;

        [JsonProperty("Button")]
        public ButtonData Button;

        [JsonProperty("SwapData")]
        public SwapData SwapData;

        [JsonProperty("children")]
        public ChildData Children;
    }

    public class Color32Data {
        [JsonProperty("r")]
        public byte R;

        [JsonProperty("g")]
        public byte G;

        [JsonProperty("b")]
        public byte B;

        [JsonProperty("a")]
        public byte A;
    }

    public class SlidingPlatformData {
        [JsonProperty("xv")]
        public float Xvelocity;

        [JsonProperty("yv")]
        public float Yvelocity;

        [JsonProperty("isIce")]
        public bool IsIce;
    }

    public class BouncePlatformData {
        [JsonProperty("xstrength")]
        public float Xstrength;

        [JsonProperty("ystrength")]
        public float Ystrength;
    }

    public class ButtonData {
        [JsonProperty("pressTime")]
        public float PressTime;

        [JsonProperty("doors")]
        public List<DoorData> Doors;
    }

    public class DoorData {
        [JsonProperty("position")]
        public Vec2 Position;

        [JsonProperty("rotation")]
        public float Rotation;

        [JsonProperty("scale")]
        public Vec2 Scale;

        [JsonProperty("color")]
        public Color32Data Color;

        [JsonProperty("isTangible")]
        public bool IsTangible = true;

        [JsonProperty("children")]
        public ChildData Children;
    }

    public class SwapData {
        [JsonProperty("on")]
        public ChildData On;

        [JsonProperty("off")]
        public ChildData Off;
    }

    public class ChildData {
        [JsonProperty("tiles")]
        public List<TileData> Tiles;

        [JsonProperty("elements")]
        public List<PrefabData> Elements;
    }
}