// (c) 2012-2013 Nick Hodge mailto:hodgenick@gmail.com & Brendan Forster
// License: MS-PL
using System;
using BoxKite.Twitter.Helpers;
using BoxKite.Twitter.Models.Service;
using Newtonsoft.Json;

namespace BoxKite.Twitter.Models
{
    public class DirectMessage : TwitterControlBase
    {
        private DateTimeOffset _time;
        [JsonProperty("created_at")]
        [JsonConverter(typeof(StringToDateTimeConverter))]
        public DateTimeOffset Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }

        private Entities _entities;
        [JsonProperty("entities")]
        public Entities Entities
        {
            get { return _entities; }
            set { SetProperty(ref _entities, value); }
        }

        private long _id;
        [JsonProperty("id")]
        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private User _recipient;
        [JsonProperty("recipient")]
        public User Recipient
        {
            get { return _recipient; }
            set { SetProperty(ref _recipient, value); }
        }

        private int _recipientid;
        [JsonProperty("recipient_id")]
        public int RecipientId
        {
            get { return _recipientid; }
            set { SetProperty(ref _recipientid, value); }
        }

        private string _recipientscreenname;
        [JsonProperty("recipient_screen_name")]
        public string RecipientScreenName
        {
            get { return _recipientscreenname; }
            set { SetProperty(ref _recipientscreenname, value); }
        }

        private User _sender;
        [JsonProperty("sender")]
        public User Sender
        {
            get { return _sender; }
            set { SetProperty(ref _sender, value); }
        }

        private int _senderid;
        [JsonProperty("sender_id")]
        public int SenderId
        {
            get { return _senderid; }
            set { SetProperty(ref _senderid, value); }
        }

        private string _senderscreenname;
        [JsonProperty("sender_screen_name")]
        public string SenderScreenName
        {
            get { return _senderscreenname; }
            set { SetProperty(ref _senderscreenname, value); }
        }

        private string _text;
        [JsonProperty("text")]
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
    }
}