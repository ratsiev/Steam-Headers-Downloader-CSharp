using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SteamHeaderDW
{
    class Program
    {
        static void Main(string[] args)
        {
            var wc = new WebClient();
            var SteamUsernameLink = "http://steamcommunity.com/id/{USERNAME}/games?tab=all&xml=1";
            var SteamID64Link = "http://steamcommunity.com/profiles/{USERNAME}/games?tab=all&xml=1";
            var DownloadLink = "";
            var DownloadCount = 0;
            var ImageLink = "http://cdn.akamai.steamstatic.com/steam/apps/{GAMEID}/header.jpg";
            while (DownloadLink == "")
            {
                Console.WriteLine("Steam CustomID or SteamID64? [customid] [steamid64]");
                var result = Console.ReadLine();
                switch (result.Trim().ToLower())
                {
                    case "customid":
                        DownloadLink = SteamUsernameLink;
                        break;
                    case "steamid64":
                        DownloadLink = SteamID64Link;
                        break;
                }
            }
            Console.WriteLine("The id?");
            DownloadLink = DownloadLink.Replace("{USERNAME}", Console.ReadLine().Trim().ToLower());
            var GamesText = wc.DownloadString(DownloadLink);
            var GamesList = FindAllBetween(GamesText, "<appID>", "</appID>", true);
            foreach (var game in GamesList)
            {
                DownloadCount++;
                Console.Write("Downloading header of {0}. [{1}/{2}]", game, DownloadCount, GamesList.Count);
                var ImageFilename = game + ".jpg";
                try
                {
                    wc.DownloadFile(ImageLink.Replace("{GAMEID}", game), ImageFilename);
                    Console.WriteLine(" - downloaded");
                }
                catch
                {
                    Console.WriteLine(" - ERROR");
                }
            }
            Console.WriteLine("================DONE================");
            Console.WriteLine("=======PRESS A BUTTON TO EXIT=======");
            Console.ReadKey(true);
        }

        static public List<string> FindAllBetween(string text, string from, string to, bool after)
        {
            List<string> found = new List<string>();
            var isafter = (after) ? from.Length : 0;
            var workline = text;

            while (workline.Contains(from))
            {
                workline = workline.Substring(workline.IndexOf(from) + isafter);
                var link = workline.Substring(0, workline.IndexOf(to));
                workline = workline.Substring(workline.IndexOf(to));
                found.Add(link);
            }

            return found;
        }
    }
}
