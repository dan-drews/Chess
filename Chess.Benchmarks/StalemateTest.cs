using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using ChessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess.Benchmarks
{

    [SimpleJob(RuntimeMoniker.Net70)]
    [RPlotExporter]
    [MemoryDiagnoser(true)]
    public class StalemateTest
    {
        private Game BitBoard = new Game(ChessLibrary.Enums.BoardType.BitBoard);
        private Engine e = new Engine(new ChessLibrary.Evaluation.ScorerConfiguration()
        {
            SelfInCheckScore = -100,
            BishopValue = 320,
            KnightValue = 325,
            OpponentInCheckScore = 40,
            CenterSquareValue = 60,
            CenterBorderValue = 30,
            PawnValue = 120,
            KingValue = 99999,
            //MaxTimeMilliseconds = 300_000, //300_000,// Int32.MaxValue, //10000,
            QueenValue = 900,
            RookValue = 600,
            StartingDepth = 6,
            MaxDepth = 6
        });

        [GlobalSetup]
        public void Iteration()
        {
            BitBoard.ResetGame();
            for (int i = 0; i < 10; i++)
            {
                var moves = BitBoard.Evaluator.GetAllLegalMoves(BitBoard.Board, BitBoard.PlayerToMove, null, true, true, true, true);
                BitBoard.AddMove(moves.First(), false);
            }
        }

        [Benchmark]
        [MinIterationTime(500)]
        public void Baseline()
        {
            var lastMove = BitBoard.Moves.Last();
            BitBoard.UndoLastMove();
            BitBoard.AddMove(lastMove, false);
        }

        [Benchmark]
        [MinIterationTime(500)]
        public void MoveListAllocations()
        {

            var ml = new List<Move>();
            for (int j = 0; j < 64; j++)
            {
                ml.Add(new Move(4561352));
            }
        }

        [Benchmark]
        [MinIterationTime(500)]
        public void StalemateDetection()
        {
            var lastMove = BitBoard.Moves.Last();
            BitBoard.UndoLastMove();
            BitBoard.AddMove(lastMove, false);
            var a = BitBoard.IsStalemate;
        }

        [Benchmark]
        [MinIterationTime(500)]
        public void CheckmateDetection()
        {
            var lastMove = BitBoard.Moves.Last();
            BitBoard.UndoLastMove();
            BitBoard.AddMove(lastMove, false);
            var a = BitBoard.IsCheckmate;
        }
    }
}
