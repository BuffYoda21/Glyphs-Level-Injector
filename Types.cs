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
}