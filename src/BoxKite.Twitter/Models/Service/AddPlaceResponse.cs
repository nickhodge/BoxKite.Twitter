namespace BoxKite.Twitter.Models.Service
{

    public class AddPlaceResponse : TwitterControlBase
    {
        public string name { get; set; }
        public object[] polylines { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public object attributes { get; set; }
        public string url { get; set; }
        public string id { get; set; }
        public BoundingBox bounding_box { get; set; }
        public Contained_Within[] contained_within { get; set; }
        public Geometry geometry { get; set; }
        public string full_name { get; set; }
        public string place_type { get; set; }
    }

    public class Geometry
    {
        public float[] coordinates { get; set; }
        public string type { get; set; }
    }

    public class Contained_Within
    {
        public string name { get; set; }
        public string country { get; set; }
        public string country_code { get; set; }
        public object attributes { get; set; }
        public string url { get; set; }
        public string id { get; set; }
        public BoundingBox bounding_box { get; set; }
        public string full_name { get; set; }
        public string place_type { get; set; }
    }
}
