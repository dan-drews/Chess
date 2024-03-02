using ChessLibrary.MoveGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public static class Perft
    {

        private static readonly ParallelOptions ParallelOptions = new()
        {
            MaxDegreeOfParallelism = 10
        };

        public static ulong ExecutePerft(Game game, int depth, bool isFirstMove)
        {
            if (depth == 0)
            {
                return 1;
            }
            ulong count = 0;
            if (isFirstMove)
            {
                var moves = game.GetAllLegalMoves().OrderBy(x => x.StartingSquare);
                Parallel.ForEach(
                    moves,
                    x =>
                    {
                        Game g2 = (Game)game.Clone();
                        g2.AddMove(x, false);
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
                Span<Move> moves = stackalloc Move[256];
                var container = new MoveListContainer(moves);
                game.GetAllLegalMoves(container);//.OrderBy(x => x.StartingSquare);
                foreach (var move in moves)
                {
                    if (move != Move.NullMove)
                    {
                        game.AddMove(move, false);
                        var childrenMoves = ExecutePerft(game, depth - 1, false);
                        count += childrenMoves;
                        game.UndoLastMove();
                    }
                }
            }

            return count;
        }
    }
}