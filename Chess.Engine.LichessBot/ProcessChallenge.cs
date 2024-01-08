using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine.LichessBot
{
    internal static class ProcessChallenge
    {

        public static void ProcessChallengeAsync(LichessEvent lcEvent, string apiKey)
        {

            var thr = new Thread(() => ProcessThread(lcEvent, apiKey));
            thr.Start();


        }

        public static async Task ProcessThread(LichessEvent lcEvent, string apiKey)
        {
            Console.WriteLine("Thread started");
            Console.WriteLine(lcEvent.challenge.id);
            Console.WriteLine(lcEvent.challenge.challenger.name);
            Console.WriteLine(lcEvent.challenge.destUser.name);
            Console.WriteLine(lcEvent.challenge.variant.name);
            Console.WriteLine(lcEvent.challenge.speed);
            Console.WriteLine(lcEvent.challenge.timeControl.type);
            Console.WriteLine(lcEvent.challenge.color);
            Console.WriteLine(lcEvent.challenge.finalColor);
            Console.WriteLine(lcEvent.challenge.perf.name);



            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://lichess.org/api/challenge/{lcEvent.challenge.id}/accept");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
        }
    }
}
