using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ChessLibrary;
using System;
using System.Collections.Generic;

namespace Chess.Benchmarks
{

    public class NaiveVsBitBoard
    {
        private Game Naive = new Game(ChessLibrary.Enums.BoardType.Naive);
        private Game BitBoard = new Game(ChessLibrary.Enums.BoardType.BitBoard);

        [GlobalSetup]
        public void Setup()
        {
            Naive.ResetGame();
            BitBoard.ResetGame();
        }

        [Benchmark]
        public List<NewMove> GetAllMovesNaive() => Naive.GetAllLegalMoves();

        [Benchmark]
        public List<NewMove> GetAllMovesBitBoard() => BitBoard.GetAllLegalMoves();
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<NaiveVsBitBoard>();
            Console.ReadLine();
        }
    }
}
