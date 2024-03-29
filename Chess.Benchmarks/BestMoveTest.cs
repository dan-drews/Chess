﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ChessLibrary;
using ChessLibrary.Evaluation;

namespace Chess.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net80)]
    [RPlotExporter]
    [MemoryDiagnoser(true)]
    public class BestMoveTest
    {
        private Game BitBoard = new Game(ChessLibrary.Enums.BoardType.BitBoard);
        private ScorerConfiguration config = new ScorerConfiguration()
        {
            SelfInCheckScore = -100,
            BishopValue = 320,
            KnightValue = 325,
            OpponentInCheckScore = 40,
            CenterSquareValue = 60,
            CenterBorderValue = 30,
            PawnValue = 120,
            KingValue = 99999,
            MaxTimeMilliseconds = 300_000, //300_000,// Int32.MaxValue, //10000,
            QueenValue = 900,
            RookValue = 600,
            StartingDepth = 6,
            MaxDepth = 6,
            UseOpeningBook = false
        };
        private Engine e;

        public BestMoveTest()
        {
            e = new Engine(config);
        }

        [IterationSetup]
        public void Iteration()
        {
            e = new Engine(config);
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
        public void GetBestMoveFromFen1()
        {
            BitBoard.LoadFen("r4rk1/ppp2p2/7p/2PqPR1P/3Pb3/2P1Q3/P3B3/5RK1 b - - 0 31");
            e.GetBestMove(BitBoard, BitBoard.PlayerToMove);
        }
    }
}
