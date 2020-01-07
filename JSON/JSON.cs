using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON
{


    public class Images
    {
        public Image[] images { get; set; }
        public string id { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string source { get; set; }
        public int width { get; set; }
    }


    public class Rootobject
    {
        public Datum[] data { get; set; }
        public Paging paging { get; set; }
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
    }

    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class Datum
    {
        public DateTime created_time { get; set; }
        public From from { get; set; }
        public string message { get; set; }
        public bool can_remove { get; set; }
        public int like_count { get; set; }
        public Message_Tags[] message_tags { get; set; }
        public DateTime updated_time { get; set; }
        public bool user_likes { get; set; }
        public string id { get; set; }
    }

    public class From
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Message_Tags
    {
        public string id { get; set; }
        public int length { get; set; }
        public string name { get; set; }
        public int offset { get; set; }
        public string type { get; set; }
    }

}
