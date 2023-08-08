using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using ChessLibrary;
using System;
using System.Collections.Generic;

namespace Chess.Benchmarks
{

    [SimpleJob(RuntimeMoniker.Net70)]
    [RPlotExporter]
    public class NaiveVsBitBoard
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
        public void Setup()
        {
            BitBoard.ResetGame();            
        }

        [Benchmark]
        public void GetBestMove()
        {
            BitBoard.ResetGame();
            e.GetBestMove(BitBoard, BitBoard.PlayerToMove);
        }

        [Benchmark]
        public void GetAllLegalMovesRecursively()
        {
            BitBoard.ResetGame();
            var moves = BitBoard.Evaluator.GetAllLegalMoves(BitBoard.Board, Colors.White, null, true, true, true, true);
            foreach (var move in moves)
            {
                RecurseMoves(BitBoard, move, 4);
            }
        }

        [Benchmark]
        public void ZobristHash()
        {
            ZobristTable.CalculateZobristHash(BitBoard.Board);
        }

        void RecurseMoves(Game g, Move move, int depth)
        {
            if (depth == 0)
            {
                return;
            }
            BitBoard.AddMove(move);
            var moves = BitBoard.GetAllLegalMoves();
            foreach (var move2 in moves)
            {
                RecurseMoves(g, move2, depth - 1);
            }
            BitBoard.UndoLastMove();
        }
        //public List<Move> GetAllMovesBitBoard() => BitBoard.GetAllLegalMoves();

    }

    public class Program
    {
        public static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
