using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ChessLibrary.Evaluation;
using ChessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net70)]
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
            StartingDepth = 1,
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
    }
}
