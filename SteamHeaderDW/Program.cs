using System;
using System.Collections.Generic;
using System.Net;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace SteamHeaderDW
{
    class Program
    {
        static void Main(string[] args) {
            var wc = new WebClient();
            var SteamUsernameLink = "http://steamcommunity.com/id/{USERNAME}/games?tab=all&xml=1";
            var SteamID64Link = "http://steamcommunity.com/profiles/{USERNAME}/games?tab=all&xml=1";
            var DownloadLink = "";
            var DownloadCount = 0;
            var ImageLink = "http://cdn.akamai.steamstatic.com/steam/apps/{GAMEID}/header.jpg";

            while (DownloadLink == "") {
                Console.WriteLine("Steam CustomID or SteamID64? [customid] [steamid64]");
                var result = Console.ReadLine();
                switch (result.Trim().ToLower()) {
                    case "customid":
                        DownloadLink = SteamUsernameLink;
                        break;
                    case "steamid64":
                        DownloadLink = SteamID64Link;
                        break;
                }
            }
            Console.WriteLine("The id?");
            var username = Console.ReadLine().Trim().ToLower();
            DownloadLink = DownloadLink.Replace("{USERNAME}", username);
            var GamesText = wc.DownloadString(DownloadLink);
            var GamesList = FindAllBetween(GamesText, "<appID>", "</appID>", true);
            foreach (var game in GamesList) {
                DownloadCount++;
                Console.Write("Downloading header of {0}. [{1}/{2}]", game, DownloadCount, GamesList.Count);
                var ImageFilename = game + ".jpg";
                try {
                    wc.DownloadFile(ImageLink.Replace("{GAMEID}", game), ImageFilename);
                    Console.WriteLine(" - downloaded");
                } catch {
                    Console.WriteLine(" - ERROR");
                }
            }

            int width = 460;
            int height = 215;
            int columns = 0;

            while (columns <= 2) {
                try {
                    Console.WriteLine("\nNumber of Columns (higher than 2):");
                    columns = Int32.Parse(Console.ReadLine());
                    if (columns <= 2) {
                        Console.WriteLine("Columns must be a number higher than 2");
                    }
                } catch {
                    Console.WriteLine("Columns must be a number");
                }
            }

            DrawImage(columns, width, height, username);

            Console.WriteLine("================DONE================");
            Console.WriteLine("=======PRESS A BUTTON TO EXIT=======");
            Console.ReadKey(true);
        }

        static public List<string> FindAllBetween(string text, string from, string to, bool after) {
            List<string> found = new List<string>();
            var isafter = (after) ? from.Length : 0;
            var workline = text;

            while (workline.Contains(from)) {
                workline = workline.Substring(workline.IndexOf(from) + isafter);
                var link = workline.Substring(0, workline.IndexOf(to));
                workline = workline.Substring(workline.IndexOf(to));
                found.Add(link);
            }

            return found;
        }

        private static void DrawImage(int columns, int width, int height, string username) {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] files = d.GetFiles("*.jpg");
            Bitmap bmp = new Bitmap(width * columns, height * (int)Math.Ceiling((files.Length + 0.0) / columns));
            Graphics g = Graphics.FromImage(bmp);

            int x = 0;
            int y = 0;
            foreach (FileInfo file in files) {
                if (x == columns) {
                    x = 0;
                    y++;
                }
                g.DrawImage(Image.FromFile(file.Name), x * width, y * height, width, height);
                x++;
            }
            var num = 0;
            while (File.Exists(username + "_" + num + ".jpg")) {
                num++;
            }

            bmp.Save(username + "_" + num + ".jpg");
        }

    }
}
