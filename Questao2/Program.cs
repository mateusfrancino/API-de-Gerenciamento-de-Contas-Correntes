using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint-Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        string url = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                dynamic data = JsonConvert.DeserializeObject(responseBody) ?? throw new ArgumentNullException(nameof(data));

                int totalGoals = 0;

                foreach (var match in data.data)
                {
                    if (match.team1.ToString() == team)
                    {
                        totalGoals += Convert.ToInt32(match.team1goals.ToString());
                    }
                    else if (match.team2.ToString() == team)
                    {
                        totalGoals += Convert.ToInt32(match.team2goals.ToString());
                    }
                }

                return totalGoals;
            }
            else
            {
                Console.WriteLine($"Falha na requisição: {response.StatusCode}");
                return 0;
            }
        }
    }
}
