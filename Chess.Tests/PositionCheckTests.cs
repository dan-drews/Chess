using ChessLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Tests
{
    [TestClass]
    public class PositionCheckTests
    {
        private Game _game { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            _game = new Game(ChessLibrary.Enums.BoardType.BitBoard);
        }

        public class SampleTest
        {
            public int depth { get; set; }
            public int nodes { get; set; }
            public string fen { get; set; }
        }

        public static IEnumerable<object[]> MovesList
        {
            get
            {
                var positionRawText = System.IO.File.ReadAllText("positions.json");
                var positions = System.Text.Json.JsonSerializer.Deserialize<List<SampleTest>>(positionRawText);

                foreach ( var position in positions )
                {
                    yield return new object[] { position.depth, position.nodes, position.fen };
                }
            }
    }

        [DynamicData(nameof(MovesList), DynamicDataSourceType.Property)]
        [DataTestMethod]
        public void ComputeMoves(int depth, int nodes, string fen)
        {
            
            _game.ResetGame();
            _game.LoadFen(fen);

            var moves = RecurseMoves(depth, false);
            Assert.AreEqual(nodes, moves.moves);

        }

        [DataRow(1, 20)]
        [DataRow(2, 400)]
        [DataRow(3, 8902)]
        [DataRow(4, 197281)]
        [DataRow(5, 4865609)]
        //[DataRow(6, 119060324)]
        [DataTestMethod]
        public void ComputeMovesStartingPosition(int depth, int nodes)
        {

            _game.ResetGame();
            _game.LoadFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            var moves = RecurseMoves(depth, false);
            Assert.AreEqual(nodes, moves.moves);

        }

        [DataRow(1, 48, 8, 0, 2)]
        [DataRow(2, 2039, 351, 1, 91)]
        [DataRow(3, 97862, 17102, 45, 3162)]
        [DataRow(4, 4085603, 757163, 1929, 128013)]
        //[DataRow(5, 193690690, 35043416, 73365, 4993637)]
        [DataTestMethod]
        public void ComputeMovesPosition2(int depth, int nodes, int captures, int enPassants, int castles)
        {

            _game.ResetGame();
            _game.LoadFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");

            var moves = RecurseMoves(depth, false);
            Assert.AreEqual(nodes, moves.moves);
            Assert.AreEqual(captures, moves.caputres);
            Assert.AreEqual(enPassants, moves.enPassants);
            Assert.AreEqual(castles, moves.castles);

        }

        [DataRow(1, 14, 1, 0, 0)]
        [DataRow(2, 191, 14, 0, 0)]
        [DataRow(3, 2812, 209, 2, 0)]
        [DataRow(4, 43238, 3348, 123, 0)]
        [DataRow(5, 674624, 52051, 1165, 0)]
        //[DataRow(6, 11030083, 940350, 33325, 0)]
        //[DataRow(7, 178633661, 14519036, 294874, 0)]
        [DataTestMethod]
        public void ComputeMovesPosition3(int depth, int nodes, int captures, int enPassants, int castles)
        {

            _game.ResetGame();
            _game.LoadFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");

            var moves = RecurseMoves(depth, false);
            Assert.AreEqual(nodes, moves.moves);
            Assert.AreEqual(captures, moves.caputres);
            Assert.AreEqual(enPassants, moves.enPassants);
            Assert.AreEqual(castles, moves.castles);

        }

        private (int moves, int caputres, int enPassants, int castles) RecurseMoves(int depth, bool isDebugging)
        {
            if(depth == 1)
            {
                var moves = _game.GetAllLegalMoves();
                var ep = moves.Where(x => x.Flags == Flag.EnPassantCapture).Count();
                var castles = moves.Where(x => x.Flags == Flag.LongCastle || x.Flags == Flag.ShortCastle).Count();
                return (moves.Length, moves.Where(x => x.CapturedPiece != null).Count(), ep, castles);
            }

            (int moves, int caputres, int enPassants, int castles) total = (0, 0, 0, 0);
            foreach(var move in _game.GetAllLegalMoves())
            {
                _game.AddMove(move, false);
                var result = RecurseMoves(depth - 1, false);//, depth == 3 && move.StartingSquare == 7 && move.TargetSquare == 4);
                if (isDebugging)
                {
                    Console.WriteLine($"{_game.Board.GetSquare(move.StartingSquare).Square.File}{_game.Board.GetSquare(move.StartingSquare).Square.Rank}{_game.Board.GetSquare(move.TargetSquare).Square.File}{_game.Board.GetSquare(move.TargetSquare).Square.Rank} {result.moves}");
                }
                total.moves += result.moves;
                total.caputres += result.caputres;
                total.enPassants += result.enPassants;
                total.castles += result.castles;
                _game.UndoLastMove();
            }
            return total;
        }
    }
}
