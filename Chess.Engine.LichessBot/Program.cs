// See https://aka.ms/new-console-template for more information
using Chess.Engine.LichessBot;
using Microsoft.Extensions.Configuration;
using System;

Console.WriteLine("Hello, World!");

var builder = new ConfigurationBuilder()
                    .AddUserSecrets<Program>()
                    .AddEnvironmentVariables();
var configurationRoot = builder.Build();

var apiKey = configurationRoot["lichess_api_key"];
var client = new HttpClient();
var request = new HttpRequestMessage(HttpMethod.Get, "https://lichess.org/api/stream/event");
request.Headers.Add("Authorization", $"Bearer {apiKey}");
var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
response.EnsureSuccessStatusCode();
var str = await response.Content.ReadAsStreamAsync();
var reader = new StreamReader(str);
var line = await reader.ReadLineAsync();
while (line != null)
{
    Console.WriteLine(line);
    if (line != "")
    {
        var lcEvent = System.Text.Json.JsonSerializer.Deserialize<LichessEvent>(line);
        if(lcEvent.type == "challenge")
        {
            ProcessChallenge.ProcessChallengeAsync(lcEvent, apiKey);
        }
        if(lcEvent.type == "gameStart")
        {
            ProcessGameStart.ProcessGameStartAsync(lcEvent, apiKey);
        }
        Console.WriteLine(lcEvent.type);
    }
    line = await reader.ReadLineAsync();
}
