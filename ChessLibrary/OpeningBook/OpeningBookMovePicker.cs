using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ChessLibrary.OpeningBook
{
    public class OpeningBookMovePicker
    {
        private static Dictionary<ulong, List<Move>> _zobristMoves =
            new Dictionary<ulong, List<Move>>();
        private static bool _isInitialized = false;
        private static Random _random = new Random(DateTime.Now.Millisecond);

        public static void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;

            var filePath = Path.Combine("book", "book.json");
            if (!Directory.Exists("book"))
            {
                Directory.CreateDirectory("book");
            }
            if (File.Exists(filePath))
            {
                _zobristMoves = JsonConvert.DeserializeObject<Dictionary<ulong, List<Move>>>(
                    File.ReadAllText(filePath)
                )!;
                return;
            }

            var games = System.IO.File.ReadAllText(System.IO.Path.Combine("GameList", "Games.txt"));
            var parser = new MatchParser();
            foreach (var game in games.Split("\n").Where(x => x.Length > 0))
            {
                Game g = new Game(ChessLibrary.Enums.BoardType.BitBoard);
                g.ResetGame();
                var moves = game.Split(' ')[0..^2];
                int movesToPerform = Math.Min(moves.Length, 8);
                foreach (var move in moves)
                {
                    var hash = ZobristTable.CalculateZobristHash(g);
                    var m = parser.GetMoveFromChessNotation(g, move);
                    g.AddMove(m, false);
                    if (!_zobristMoves.ContainsKey(hash))
                    {
                        _zobristMoves.Add(hash, new List<Move>());
                    }
                    _zobristMoves[hash].Add(m);
                }
            }
            File.WriteAllText(filePath, JsonConvert.SerializeObject(_zobristMoves));
        }

        public static Move? GetMoveForZobrist(ulong hash)
        {
            if (!_zobristMoves.ContainsKey(hash))
            {
                return null;
            }
            var moves = _zobristMoves[hash];
            if (moves.Count == 1)
            {
                return null; // Don't pick it if it's the only variation
            }
            return _zobristMoves[hash][_random.Next(moves.Count)];
        }
    }
}
