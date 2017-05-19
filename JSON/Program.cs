using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;

namespace JSON
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "json")
            {
                json();
            }
            else
            {
                photos();
            }

            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }


        public static void photos()
        {
            string outputPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),"Files");

            var client = new FacebookClient();
            client.AccessToken = ConfigurationManager.AppSettings["sdk"];
            client.AppId = "v2.9";

            // my user id / albums
            var get = client.Get("376705120039/albums");
            string text = Convert.ToString(get);
            string photoText = null;
            string urlText = null;

            // Serialize fb api request
            JavaScriptSerializer oJS = new JavaScriptSerializer();
            Rootobject oRootObject = new Rootobject();
            oRootObject = oJS.Deserialize<Rootobject>(text);

            Rootobject photos = new Rootobject();
            Images urls = new Images();
            int i = 0;
            foreach (var item in oRootObject.data)
            {
                var photoGet = client.Get(item.id + "/photos");
                photoText = Convert.ToString(photoGet);
                photos = oJS.Deserialize<Rootobject>(photoText);
                foreach (var it in photos.data)
                {
                    var urlGet = client.Get(it.id + "?fields=images");
                    urlText = Convert.ToString(urlGet);
                    urls = oJS.Deserialize<Images>(urlText);

                    using (WebClient clientFB = new WebClient())
                    {
                        //clientFB.DownloadFile(new Uri(urls.images[0].source), outputPath + "\\" + i.ToString());

                        //OR 

                        clientFB.DownloadFileAsync(new Uri(urls.images[0].source), outputPath + "\\" + i.ToString() + ".png");
                        //Console.WriteLine(urls.images[0].source + outputPath + "\\" + i.ToString());
                    }
                    i++;
                }
            }

        }
        public static void json()
        {
            // Configure fbclient and send the request
            var client = new FacebookClient();
            client.AccessToken = ConfigurationManager.AppSettings["sdk"];
            client.AppId = "v2.3";
            var get = client.Get("10155212799055040/comments?limit=500");
            string text = Convert.ToString(get);

            // Serialize fb api request
            JavaScriptSerializer oJS = new JavaScriptSerializer();
            Rootobject oRootObject = new Rootobject();
            oRootObject = oJS.Deserialize<Rootobject>(text);

            // Order by likes 
            IEnumerable<Datum> query = oRootObject.data.OrderBy(dat => dat.like_count);

            // Set limit date for posting
            string dateTime = "05/03/2017 23:59:59.42";
            DateTime dt = Convert.ToDateTime(dateTime, CultureInfo.InvariantCulture);

            // Print the result
            foreach (var data in query)
            {
                if (data.created_time < dt)
                {
                    Console.Write("User:" + data.from.name);
                    Console.WriteLine("\tCreated:" + data.created_time.ToString());
                    Console.WriteLine(data.like_count + " likes");
                    Console.WriteLine("-------------------------------------------------------------------");
                }
                else
                {
                    Console.Write("Invalid User:" + data.from.name);
                    Console.WriteLine("\tCreated:" + data.created_time.ToString());
                    Console.WriteLine(data.like_count + " likes");
                    Console.WriteLine("-------------------------------------------------------------------");
                }
            }
            Console.WriteLine(oRootObject.data.Length + " Posts");
            Console.WriteLine("Verification made at: " + DateTime.Now.ToString());
        }
    }
}
