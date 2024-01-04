using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChessLibrary.TranspositionTables;

namespace ChessLibrary
{
    public static class Perft
    {
        public static TranspositionTable _perftTable = new TranspositionTable();

        public static void ClearPerftTable() => _perftTable = new TranspositionTable();

        public static int ExecutePerft(Game game, int depth, bool isFirstMove)
        {
            if (depth == 0)
            {
                return 1;
            }
            var hash = ZobristTable.CalculateZobristHash(game);
            if (_perftTable.IsHashPopulatedForDepth(hash, depth))
            {
                return _perftTable[hash, depth];
            }
            int count = 0;
            var moves = game.GetAllLegalMoves().OrderBy(x => x.StartingSquare);
            if (isFirstMove)
            {
                Parallel.ForEach(
                    moves,
                    x =>
                    {
                        Game g2 = (Game)game.Clone();
                        g2.AddMove(x);
                        var childrenMoves = ExecutePerft(g2, depth - 1, false);
                        Interlocked.Add(ref count, childrenMoves);
                        //count += childrenMoves;

                        var startingSquare = new Square(x.StartingSquare);
                        var targetSquare = new Square(x.TargetSquare);
                        Console.WriteLine(
                            $"{startingSquare.File.ToString().ToLower()}{startingSquare.Rank}{targetSquare.File.ToString().ToLower()}{targetSquare.Rank}: {childrenMoves}"
                        );
                        g2.UndoLastMove();
                    }
                );
            }
            else
            {
                foreach (var move in moves)
                {
                    game.AddMove(move);
                    var childrenMoves = ExecutePerft(game, depth - 1, false);
                    count += childrenMoves;
                    game.UndoLastMove();
                }
            }

            _perftTable[hash, depth] = count;
            return count;
        }
    }
}
