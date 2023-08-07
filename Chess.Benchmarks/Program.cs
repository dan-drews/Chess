using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ChessLibrary;
using System;
using System.Collections.Generic;

namespace Chess.Benchmarks
{

    public class NaiveVsBitBoard
    {
        private Game BitBoard = new Game(ChessLibrary.Enums.BoardType.BitBoard);

        [GlobalSetup]
        public void Setup()
        {
            BitBoard.ResetGame();
        }

        [Benchmark]
        public List<Move> GetAllMovesBitBoard() => BitBoard.GetAllLegalMoves();
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
