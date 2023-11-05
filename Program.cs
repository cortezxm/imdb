using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace imdb
{
    public class WebRequestGetExample
    {
        public static ArrayList films = new ArrayList();

        public static void Main()
        {
            string indexHTML = requestIndex();
            extractOptions(indexHTML);
            printOptions();

            Console.WriteLine("Selecciona una opcion: ");
            string opt = Console.ReadLine();
            int option = int.Parse(opt) - 1;
            string filmHTML = requestFilm(opt);
            extractMetadata(option, filmHTML);
            printFilm(option);
            Console.ReadKey();
        }

        public static string requestIndex()
        {
            Console.WriteLine("Ingresa una título: ");
            string film = Console.ReadLine();
            Console.WriteLine("Buscando...");
            Console.WriteLine("");

            string sRequest = "https://www.imdb.com/find/?s=tt&q=" + film + "&ref_=nv_sr_sm";
            WebRequest request = WebRequest.Create(sRequest);
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        public static void extractOptions(string html)
        {
            MatchCollection matches = Regex.Matches(html, "<a class=\"ipc-metadata-list-summary-item__t\".*?href=\"(.*?)\".*?>(.*?)</a>.*?<li role=\"presentation\" class=\"ipc-inline-list__item\"><span class=\"ipc-metadata-list-summary-item__li\".*?>(\\d{4})</span></li>.*?(<li role=\"presentation\" class=\"ipc-inline-list__item\"><span class=\"ipc-metadata-list-summary-item__li\".*?>(.*?)</span></li>)?.*?<li role=\"presentation\" class=\"ipc-inline-list__item\"><span class=\"ipc-metadata-list-summary-item__li\".*?>(.*?)</span></li>", RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                string url = match.Groups[1].Value;
                string name = match.Groups[2].Value;
                string year = match.Groups[3].Value;
                string type = match.Groups[5].Value;
                string starrings = match.Groups[6].Value;

                if (String.IsNullOrEmpty(type))
                    type = "Film";

                Film filmObj = new Film(type, url, name, year, starrings);
                films.Add(filmObj);

                if (films.Count == 5)
                    break;

            }
        }

        public static void printOptions()
        {
            Console.WriteLine("Resultados: ");
            for (int i = 0; i < films.Count; i++)
            {
                if (films[i] is Film)
                {
                    Film film = (Film)films[i];
                    int index = i + 1;

                    Console.WriteLine(index + ".");
                    if (film.type == "TV Series")
                        Console.WriteLine("\t" + film.name + " (" + film.type + ")");
                    else
                        Console.WriteLine("\t" + film.name);
                    Console.WriteLine("\t" + film.year);
                    Console.WriteLine("\t" + film.starrings);
                    Console.WriteLine("");
                }
            }
        }

        public static string requestFilm(string opt)
        {
            int option;
            string url = " ";

            if (int.TryParse(opt, out option))
            {
                if (films[option] is Film)
                {
                    Film film = (Film)films[option - 1];
                    url = film.url;
                }
            }

            string sRequest = "https://www.imdb.com" + url;
            WebRequest request = WebRequest.Create(sRequest);
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        public static void extractMetadata(int index, string html)
        {
            Film film = (Film)films[index];

            Match scoreMatch = Regex.Match(html, "<span class=\"sc-bde20123-1 iZlgcd\">(.*?)</span>");
            if (scoreMatch.Success)
            {
                string score = scoreMatch.Groups[1].Value;
                film.score = score;
            }

            Match runtimeAndRatingMatch = Regex.Match(html, "<ul class=\"ipc-inline-list ipc-inline-list--show-dividers sc-afe43def-4 kdXikI baseAlt\" role=\"presentation\">(.*?)</ul>");
            if (runtimeAndRatingMatch.Success)
            {
                string listItems = runtimeAndRatingMatch.Groups[1].Value;
                MatchCollection listItemMatches = Regex.Matches(listItems, "<li role=\"presentation\" class=\"ipc-inline-list__item\">(.*?)</li>");

                if (listItemMatches.Count >= 3)
                {
                    string runtime = listItemMatches[2].Groups[1].Value;
                    film.runtime = runtime;
                }
            }

            Match descriptionMatch = Regex.Match(html, "<span role=\"presentation\" data-testid=\"plot-xl\" class=\"sc-466bb6c-2 eVLpWt\">(.*?)</span>");
            if (descriptionMatch.Success)
            {
                string description = descriptionMatch.Groups[1].Value;
                film.description = description;
            }

            Match directorMatch = Regex.Match(html, "<a class=\"ipc-metadata-list-item__list-content-item ipc-metadata-list-item__list-content-item--link\" role=\"button\" tabindex=\"0\" aria-disabled=\"false\" href=\"(.*?)\">(.*?)</a>");
            if (directorMatch.Success)
            {
                string director = directorMatch.Groups[2].Value;
                film.direction = director;
            }
        }

        public static void printFilm(int index)
        {
            Film film = (Film)films[index];

            Console.WriteLine("Type: " + film.type);
            Console.WriteLine("Name: " + film.name);
            Console.WriteLine("Year: " + film.year);
            Console.WriteLine("Direction: " + film.direction);
            Console.WriteLine("Starrings: " + film.starrings);
            Console.WriteLine("Description: " + film.description);
            Console.WriteLine("Score: " + film.score + "/10");
            Console.WriteLine("Runtime: " + film.runtime);
        }

    }
}
