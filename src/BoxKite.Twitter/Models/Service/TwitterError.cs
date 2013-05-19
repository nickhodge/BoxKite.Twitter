namespace BoxKite.Twitter.Models.Service
{
    public class TwitterError
    {
        public Error[] errors { get; set; }
    }

    public class Error
    {
        public string message { get; set; }
        public int code { get; set; }
    }
}
