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
                jsonFut();
            }
            else if (args[0] == "jsonLoop")
            {
                jsonLoop();
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

        public static void jsonFut()
        {
            // Configure fbclient and send the request
            var client = new FacebookClient();
            client.AccessToken = ConfigurationManager.AppSettings["sdk"];
            var groupid = ConfigurationManager.AppSettings["groupid"];
            client.AppId = "v3.3";
            var get = client.Get(groupid + "\\feed");
            string text = Convert.ToString(get);

            // Serialize fb api request
            JavaScriptSerializer oJS = new JavaScriptSerializer();
            Rootobject oRootObject = new Rootobject();
            oRootObject = oJS.Deserialize<Rootobject>(text);

            // Order by date 
            Datum query = oRootObject.data
                            .OrderByDescending(dat => dat.updated_time)
                            .ElementAt(0);

            // Split into list of players
            List<string> players = query.message.Split(new[] { "\n" }, StringSplitOptions.None)
                            .ToList();

            // Print the result
            foreach (var data in players)
            {
                if (data == null || data.Contains('|') || data == "")
                {
                    Console.WriteLine("Invalid Player:" + data);
                }
                else
                {
                    Console.WriteLine("Player:" + data);
                }
            }
            Console.WriteLine("Verification made at: " + DateTime.Now.ToString());
        }

        public static void jsonLoop()
        {
            // Configure fbclient and send the request
            var client = new FacebookClient();
            client.AccessToken = ConfigurationManager.AppSettings["sdk"];
            var groupid = ConfigurationManager.AppSettings["groupid"];
            client.AppId = "v3.3";
            var get = client.Get(groupid + "\\feed?limit=100");
            string text = Convert.ToString(get);

            // Serialize fb api request
            JavaScriptSerializer oJS = new JavaScriptSerializer();
            Rootobject oRootObject = new Rootobject();
            oRootObject = oJS.Deserialize<Rootobject>(text);

            // Order by date 
            List<Datum> queryAll = oRootObject.data
                            .OrderByDescending(dat => dat.updated_time)
                            .ToList();

            // Filter words
            List<string> wordsToFilter = ConfigurationManager.AppSettings["wordsToFilter"]
                                            .Split(';')
                                            .ToList();

            List<string> playersCount = new List<string>();
            // Create output file
            using (StreamWriter streamWriter = new StreamWriter(@"C:\Users\hmatos\Downloads\futLoop.csv", false, Encoding.Unicode))
            {
                int index = 0;
                foreach (Datum query in queryAll)
                {

                    // Split into list of players
                    List<string> players = query.message.Split(new[] { "\n" }, StringSplitOptions.None)
                                    .ToList();

                    if (query.message.Length > 100)
                    {

                        streamWriter.Write(++index + ", " + query.id + ", " + query.updated_time + ", ");


                        // Print the result
                        foreach (var data in players)
                        {

                            if (data == null || data.Contains('|') || data == "" || data.ToLower().Split(' ').ToList().Select(x => x.Trim())
                                                                                            .Intersect(wordsToFilter)
                                                                                            .Any())
                            {
                                //Console.WriteLine("Invalid Player:" + data);
                            }
                            else
                            {
                                streamWriter.Write(data + ", ");
                                playersCount.Add(data.Trim());
                                //Console.WriteLine("Player:" + data);
                            }
                        }

                        streamWriter.WriteLine();
                    }
                }
            }

            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            List<string> listDistinct = playersCount.Distinct().ToList();
            foreach (var player in listDistinct) {
                dictionary.Add(player, playersCount.Count(x => x.Equals(player)));
            }

            using (StreamWriter streamWriter = new StreamWriter(@"C:\Users\hmatos\Downloads\futLoopCount.csv", false, Encoding.Unicode))
            {
                streamWriter.WriteLine("Player, Count");
                foreach(var dict in dictionary)
                {
                    streamWriter.WriteLine(dict.Key + ", " + dict.Value);
                }
                
            }


                Console.WriteLine("Verification made at: " + DateTime.Now.ToString());
        }
    }
}
