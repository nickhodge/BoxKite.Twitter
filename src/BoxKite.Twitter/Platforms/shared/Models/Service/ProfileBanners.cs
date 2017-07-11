// (c) 2012-2016 Nick Hodge mailto:nhodge@mungr.com & Brendan Forster
// License: MS-PL

using System.Collections.Generic;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{

    public class ProfileBanners : TwitterControlBase
    {
        public BannerSizes sizes { get; set; }
    }

    public class BannerSizes
    {
        [JsonProperty("1500x500")]
        public BannerImageDetails Standard_1500x500 { get; set; }

        [JsonProperty("web_retina")]
        public BannerImageDetails WebRetina { get; set; }

        [JsonProperty("ipad_retina")]
        public BannerImageDetails IpadRetina { get; set; }

        [JsonProperty("600x200")]
        public BannerImageDetails Standard_600x200 { get; set; }

        [JsonProperty("300x100")]
        public BannerImageDetails Standard_300x100 { get; set; }

        [JsonProperty("mobile")]
        public BannerImageDetails Mobile { get; set; }

        [JsonProperty("ipad")]
        public BannerImageDetails Ipad { get; set; }

        [JsonProperty("mobile_retina")]
        public BannerImageDetails MobileRetina { get; set; }

        [JsonProperty("web")]
        public BannerImageDetails Web { get; set; }
    }

    public class BannerImageDetails
    {
        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
