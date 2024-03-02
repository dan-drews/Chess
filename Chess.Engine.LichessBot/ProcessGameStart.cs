using ChessLibrary;
using ChessLibrary.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine.LichessBot
{
    internal static class ProcessGameStart
    {

        public static void ProcessGameStartAsync(LichessEvent lcEvent, string apiKey)
        {

            var thr = new Thread(() => ProcessThread(lcEvent, apiKey));
            thr.Start();
        }

        public static async Task ProcessThread(LichessEvent lcEvent, string apiKey)
        {
            Console.WriteLine("Thread started");
            Console.WriteLine(lcEvent.game.id);
            Console.WriteLine(lcEvent.game.variant.name);
            Console.WriteLine(lcEvent.game.speed);
            Console.WriteLine(lcEvent.game.color);

            var playerColor = lcEvent.game.color == "white" ? Colors.White : Colors.Black;

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://lichess.org/api/bot/game/stream/{lcEvent.game.id}");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(str);
            var line = await reader.ReadLineAsync();

            var matchParser = new MatchParser();
            var game = new ChessLibrary.Game(ChessLibrary.Enums.BoardType.BitBoard, false);
            var engine = new ChessLibrary.Engine(new ScorerConfiguration()
            {
                SelfInCheckScore = -100,
                BishopValue = 320,
                KnightValue = 325,
                OpponentInCheckScore = 40,
                CenterSquareValue = 60,
                CenterBorderValue = 30,
                PawnValue = 120,
                KingValue = 99999,
                MaxTimeMilliseconds = 10_000, //300_000,// Int32.MaxValue, //10000,
                QueenValue = 900,
                RookValue = 600,
                StartingDepth = 1,
                MaxDepth = null
            });
            while (line != null)
            {
                Console.WriteLine(line);
                if(line != "")
                {
                    var lcGameState = System.Text.Json.JsonSerializer.Deserialize<LichessGameState>(line);
                    if(lcGameState.type == "gameFull")
                    {
                        var lcGame = System.Text.Json.JsonSerializer.Deserialize<LichessGame>(line);
                        lcGameState = lcGame.state;
                    }
                    Console.WriteLine(lcGameState.type);
                    Console.WriteLine(lcGameState.moves);
                    Console.WriteLine(lcGameState.wtime);
                    Console.WriteLine(lcGameState.btime);
                    Console.WriteLine(lcGameState.winc);
                    Console.WriteLine(lcGameState.binc);
                    Console.WriteLine(lcGameState.status);

                    if(lcGameState.status == "aborted" || lcGameState.status == "mate" || lcGameState.status == "draw" || lcGameState.status == "stalemate" || lcGameState.status == "resign" || lcGameState.status == "outoftime")
                    {
                        return;
                    }

                    if(lcGameState.status == "started")
                    {
                        game.ResetGame();
                        if (lcGameState.moves != string.Empty)
                        {
                            matchParser.LoadGameFromPgn(game, lcGameState.moves);
                        }

                        if (game.PlayerToMove == playerColor)
                        {
                            var move = engine.GetBestMove(game,playerColor).node.Move.ToString();
                            Console.WriteLine(move);

                            var request2 = new HttpRequestMessage(HttpMethod.Post, $"https://lichess.org/api/bot/game/{lcEvent.game.id}/move/{move}");
                            request2.Headers.Add("Authorization", $"Bearer {apiKey}");
                            var response2 = await client.SendAsync(request2);
                            Console.WriteLine(await response2.Content.ReadAsStringAsync());
                        }
                    }

                }
                line = await reader.ReadLineAsync();
            }
        }
    }
}
