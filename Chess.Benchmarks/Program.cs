﻿using BenchmarkDotNet.Attributes;
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

        [IterationSetup]
        public void Iteration()
        {
            BitBoard.ResetGame();
        }

        [GlobalSetup]
        public void Setup()
        {
            BitBoard.ResetGame();            
        }

        [Benchmark]
        public void GetBestMove()
        {
            e.GetBestMove(BitBoard, BitBoard.PlayerToMove);
        }

        [Benchmark]
        public void Perft1()
        {
            BitBoard.Evaluator.GetAllLegalMoves(BitBoard.Board, Colors.White, null, true, true, true, true);
        }

        [Benchmark]
        public void Perft2()
        {
            var moves = BitBoard.Evaluator.GetAllLegalMoves(BitBoard.Board, Colors.White, null, true, true, true, true);
            foreach (var move in moves)
            {
                RecurseMoves(BitBoard, move, 1);
            }
        }

        [Benchmark]
        public void Perft3()
        {
            var moves = BitBoard.Evaluator.GetAllLegalMoves(BitBoard.Board, Colors.White, null, true, true, true, true);
            foreach (var move in moves)
            {
                RecurseMoves(BitBoard, move, 2);
            }
        }

        [Benchmark]
        public void Perft4()
        {
            var moves = BitBoard.Evaluator.GetAllLegalMoves(BitBoard.Board, Colors.White, null, true, true, true, true);
            foreach (var move in moves)
            {
                RecurseMoves(BitBoard, move, 3);
            }
        }

        

        [Benchmark]
        public void ZobristHash()
        {
            ZobristTable.CalculateZobristHash(BitBoard.Board);
        }

        [Benchmark]
        public void LoadFen()
        {
            BitBoard.LoadFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
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
