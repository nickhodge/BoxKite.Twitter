using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models           
{
    public class Configuration : TwitterControlBase
    {
        [JsonProperty("characters_reserved_per_media")]
        public int CharactersReservedPerMedia { get; set; }

        [JsonProperty("max_media_per_upload")]
        public int MaxMediaPerUpload { get; set; }

        [JsonProperty("non_username_paths")]
        public List<string> NonUsernamePaths { get; set; }

        [JsonProperty("photo_size_limit")]
        public int PhotoSizeLimit { get; set; }

        [JsonProperty("photo_sizes")]
        public PhotoSizes PhotoSizes { get; set; }

        [JsonProperty("short_url_length")]
        public int ShortUrlLength { get; set; }

        [JsonProperty("short_url_length_https")]
        public int ShortUrlLengthHttps { get; set; }


        [JsonProperty("dm_text_character_limit")]
        public int DirectMessageCharacterLimit { get; set; }
    }
}
