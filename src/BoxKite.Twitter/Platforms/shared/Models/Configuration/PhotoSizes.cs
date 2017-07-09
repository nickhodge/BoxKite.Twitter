 using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class PhotoSizes
    {
        [JsonProperty("thumb")]
        public Size Thumb { get; set; }

        [JsonProperty("small")]
        public Size Small { get; set; }

        [JsonProperty("medium")]
        public Size Medium { get; set; }

        [JsonProperty("large")]
        public Size Large { get; set; }
    }

    public class Size
    {
        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("resize")]
        /// <summary>
        /// Crop or fit
        /// </summary>
        public string Resize { get; set; }
    }
}