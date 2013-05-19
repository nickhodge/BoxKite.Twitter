namespace BoxKite.Twitter.Models
{
    public class Event
    {
        /*{
  "target": TARGET_USER,
  "source": SOURCE_USER, 
  "event":"EVENT_NAME",
  "target_object": TARGET_OBJECT,
  "created_at": "Sat Sep 4 16:10:54 +0000 2010"
}*/
        public User Target { get; set; }
        public User Source { get; set; }
        public string EventName { get; set; }

    }

    public class ListEvent : Event
    {
        //public TwitterList TargetObject { get; set; }
    }

    public class TweetEvent : Event
    {
        public Tweet TargetObject { get; set; }
    }
}
