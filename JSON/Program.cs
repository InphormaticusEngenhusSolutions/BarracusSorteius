using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;
using System.Configuration;
using System.Globalization;

namespace JSON
{
    class Program
    {
        static void Main(string[] args)
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

            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}
