using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary.OpeningBook
{
    public class OpeningBookMovePicker
    {

        private static Dictionary<ulong, List<Move>> ZobristMoves = new Dictionary<ulong, List<Move>>();
        private static bool _isInitialized = false;
        private static Random _random = new Random(DateTime.Now.Millisecond);
        public static void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            _isInitialized = true;

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
                    var hash = ZobristTable.CalculateZobristHash(g.Board);
                    var m = parser.GetMoveFromChessNotation(g, move);
                    g.AddMove(m);
                    if (!ZobristMoves.ContainsKey(hash))
                    {
                        ZobristMoves.Add(hash, new List<Move>());
                    }
                    ZobristMoves[hash].Add(m);
                }
            }
        }

        public static Move? GetMoveForZobrist(ulong hash)
        {
            if (!ZobristMoves.ContainsKey(hash))
            {
                return null;
            }
            var moves = ZobristMoves[hash];
            return ZobristMoves[hash][_random.Next(moves.Count)];
        }
    }
}
