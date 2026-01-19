using Newtonsoft.Json;
using System.Collections.Generic;

namespace LevelInjector {
    public class RoomData {
        [JsonProperty("localPosition")]
        public Vec2 LocalPosition;

        [JsonProperty("tiles")]
        public List<TileData> Tiles;
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