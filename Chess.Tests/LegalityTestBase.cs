using ChessLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess.Tests
{
    public abstract class LegalityTestBase
    {
        public abstract Game Game { get; }

        [TestMethod]
        public void GetValidMoves_ForAllWhiteStartingPieces_Provides20Moves()
        {
            Game.ResetGame();
            Game.Board.ClearPiece(Files.A, 8);
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, null, true, true, true, true).Count;
            Assert.AreEqual(20, count);
        }

        [TestMethod]
        public void CanCalculateHash()
        {
            ulong hash = 0;
            for (int i = 0; i < 1_000_000; i++)
            {
                hash = Game.ZobristTable.CalculateZobristHash(Game.Board);
            }
            Assert.AreNotEqual(0, hash);
        }

        [TestMethod]
        public void GetValidMoves_ForAllWhiteStartingPieces_InLoop_Provides20Moves()
        {
            int count = 0;
            for (int i = 0; i < 1_000_000; i++)
            {
                Game.ResetGame();
                var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, null, true, true, true, true);
                count = moves.Count;
            }
            Assert.AreEqual(20, count);
        }

        [Ignore]
        [TestMethod]
        public void Get5LayersOfDepth()
        {
            int count = 0;
            for (int i = 0; i < 1; i++)
            {
                Game.ResetGame();
                var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, null, true, true, true, true);
                count = moves.Count;
                foreach(var move in moves)
                {
                    RecurseMoves(Game, move, 5);
                }
            }
        }

        void RecurseMoves(Game g, Move move, int depth)
        {
            if(depth == 0)
            {
                return;
            }
            if(move.StartingSquare.Rank == 8 && move.StartingSquare.File == Files.F && move.DestinationSquare.Rank == 6 && move.DestinationSquare.File == Files.H)
            {
                int i = 0;
            }
            Game.AddMove(move);
            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.PlayerToMove, Game.EnPassantFile, Game.BlackCanLongCastle, Game.BlackCanShortCastle, Game.WhiteCanLongCastle, Game.WhiteCanShortCastle);
            foreach(var move2 in moves)
            {
                RecurseMoves(g, move2, depth - 1);
            }
            Game.UndoLastMove();
        }

        [TestMethod]
        public void GetValidMoves_Allows_Castling()
        {
            Game.ResetGame();
            Game.Board.ClearPiece(Files.F, 1);
            Game.Board.ClearPiece(Files.G, 1);
            Game.Board.ClearPiece(Files.B, 1);
            Game.Board.ClearPiece(Files.C, 1);
            Game.Board.ClearPiece(Files.D, 1);

            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, null, true, true, true, true);
            Assert.IsTrue(moves.Any(x => x.Piece.Type == PieceTypes.King && x.DestinationSquare.File == Files.G && x.DestinationSquare.Rank == 1));
            Assert.IsTrue(moves.Any(x => x.Piece.Type == PieceTypes.King && x.DestinationSquare.File == Files.C && x.DestinationSquare.Rank == 1));
            Game.AddMove(moves.First(x => x.Piece.Type == PieceTypes.King && x.DestinationSquare.File == Files.G && x.DestinationSquare.Rank == 1));
            Assert.IsTrue(Game.Board.GetSquare(Files.G, 1).Piece.Type == PieceTypes.King);
            Assert.IsTrue(Game.Board.GetSquare(Files.F, 1).Piece.Type == PieceTypes.Rook);

        }

        [TestMethod]
        public void GetValidMoves_WithNoPawnInFrontOfRook()
        {
            Game.ResetGame();
            Game.Board.ClearPiece(Files.H, 2);
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, null, true, true, true, true).Count;
            Assert.AreEqual(24, count);
        }

        [TestMethod]
        public void GetValidMoves_InitialSetup_AddedRook()
        {
            Game.ResetGame();
            Game.Board.SetPiece(Files.D, 4, PieceTypes.Rook, Colors.White);
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, null, true, true, true, true).Count;
            Assert.AreEqual(30, count);
        }

        [TestMethod]
        public void GetValidMoves_InitialSetup_AddedQueen()
        {
            Game.ResetGame();
            Game.Board.SetPiece(Files.D, 4, PieceTypes.Queen, Colors.White);
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, null, true, true, true, true).Count;
            Assert.AreEqual(38, count);
        }

        [TestMethod]
        public void GetValidMoves_WithNoPawnInFrontOfBishop()
        {
            Game.ResetGame();
            Game.Board.ClearPiece(Files.E, 2);
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, null, true, true, true, true).Count;
            Assert.AreEqual(29, count);
        }

        [TestMethod]
        public void GetValidMoves_AfterE4E5()
        {
            Game.ResetGame();
            Game.AddMove(new Move(new Piece() { Color = Colors.White, Type = PieceTypes.Pawn}, Colors.White, new Square() { File = Files.E, Rank = 2 }, new Square() { File = Files.E, Rank = 4 }));
            Game.AddMove(new Move(new Piece() { Color = Colors.Black, Type = PieceTypes.Pawn }, Colors.Black, new Square() { File = Files.E, Rank = 7 }, new Square() { File = Files.E, Rank = 5 }));
            int count = 0;
            for (Files file = Files.A; file <= Files.H; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var square = Game.Board.GetSquare(file, rank);
                    if (square?.Piece?.Color == Colors.White)
                    {
                        var legalMoves = Game.Evaluator.GetAllLegalMoves(Game.Board, square, null, true, true, true, true);
                        count += legalMoves?.Count ?? 0;
                    }
                }
            }

            Assert.AreEqual(29, count);
        }

        [TestMethod]
        public void GetValidMoves_ForRookWithEmptyBoardAndRandomKing_Provides14Moves()
        {
            Game.Board.SetPiece(Files.B, 5, PieceTypes.Rook, Colors.White);
            Game.Board.SetPiece(Files.H, 1, PieceTypes.King, Colors.White);
            Game.Board.SetPiece(Files.H, 8, PieceTypes.King, Colors.Black);


            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.Board.GetSquare(Files.B, 5), null, true, true, true, true);

            Assert.AreEqual(14, moves.Count);
        }

        [TestMethod]
        public void GetValidMoves_ForRookWithBlockedSpace_Provides10Moves()
        {

            Game.Board.SetPiece(Files.B, 5, PieceTypes.Rook, Colors.White);
            Game.Board.SetPiece(Files.B, 4, PieceTypes.Pawn, Colors.White);
            Game.Board.SetPiece(Files.H, 1, PieceTypes.King, Colors.White);
            Game.Board.SetPiece(Files.H, 8, PieceTypes.King, Colors.Black);


            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.Board.GetSquare(Files.B, 5), null, true, true, true, true);

            Assert.AreEqual(10, moves.Count);
        }

        [TestMethod]
        public void GetValidMoves_TwoRooks_DisallowsCheck()
        {
            Game.Board.SetPiece(Files.H, 3, PieceTypes.Rook, Colors.White);
            Game.Board.SetPiece(Files.H, 6, PieceTypes.Rook, Colors.Black);
            Game.Board.SetPiece(Files.H, 1, PieceTypes.King, Colors.White);
            Game.Board.SetPiece(Files.H, 8, PieceTypes.King, Colors.Black);


            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.Board.GetSquare(Files.H, 3), null, true, true, true, true);

            Assert.AreEqual(4, moves.Count);
        }

        [TestMethod]
        public void GetValidMoves_PawnAttacking_DisallowsCheck()
        {
            Game.ResetGame();

            Game.AddMove(new Move(Game.Board.GetSquare(Files.B, 1).Piece, Colors.White, Game.Board.GetSquare(Files.B, 1).Square, Game.Board.GetSquare(Files.C, 3).Square));
            Game.AddMove(new Move(Game.Board.GetSquare(Files.D, 7).Piece, Colors.Black, Game.Board.GetSquare(Files.D, 7).Square, Game.Board.GetSquare(Files.D, 5).Square));

            Game.AddMove(new Move(Game.Board.GetSquare(Files.G, 1).Piece, Colors.White, Game.Board.GetSquare(Files.G, 1).Square, Game.Board.GetSquare(Files.F, 3).Square));
            Game.AddMove(new Move(Game.Board.GetSquare(Files.G, 8).Piece, Colors.Black, Game.Board.GetSquare(Files.G, 8).Square, Game.Board.GetSquare(Files.F, 6).Square));

            Game.AddMove(new Move(Game.Board.GetSquare(Files.H, 1).Piece, Colors.White, Game.Board.GetSquare(Files.H, 1).Square, Game.Board.GetSquare(Files.G, 1).Square));
            Game.AddMove(new Move(Game.Board.GetSquare(Files.F, 6).Piece, Colors.Black, Game.Board.GetSquare(Files.F, 6).Square, Game.Board.GetSquare(Files.G, 4).Square));

            Game.AddMove(new Move(Game.Board.GetSquare(Files.C, 3).Piece, Colors.White, Game.Board.GetSquare(Files.C, 3).Square, Game.Board.GetSquare(Files.D, 5).Square) { CapturedPiece = Game.Board.GetSquare(Files.D, 5).Piece });
            Game.AddMove(new Move(Game.Board.GetSquare(Files.F, 7).Piece, Colors.Black, Game.Board.GetSquare(Files.F, 7).Square, Game.Board.GetSquare(Files.F, 6).Square));

            Game.AddMove(new Move(Game.Board.GetSquare(Files.D, 5).Piece, Colors.White, Game.Board.GetSquare(Files.D, 5).Square, Game.Board.GetSquare(Files.C, 7).Square) { CapturedPiece = Game.Board.GetSquare(Files.C, 7).Piece });
            Game.AddMove(new Move(Game.Board.GetSquare(Files.E, 8).Piece, Colors.Black, Game.Board.GetSquare(Files.E, 8).Square, Game.Board.GetSquare(Files.D, 7).Square));

            Game.AddMove(new Move(Game.Board.GetSquare(Files.E, 2).Piece, Colors.White, Game.Board.GetSquare(Files.E, 2).Square, Game.Board.GetSquare(Files.E, 4).Square));
            Game.AddMove(new Move(Game.Board.GetSquare(Files.D, 8).Piece, Colors.Black, Game.Board.GetSquare(Files.D, 8).Square, Game.Board.GetSquare(Files.C, 7).Square) { CapturedPiece = Game.Board.GetSquare(Files.C, 7).Piece });

            Game.AddMove(new Move(Game.Board.GetSquare(Files.E, 4).Piece, Colors.White, Game.Board.GetSquare(Files.E, 4).Square, Game.Board.GetSquare(Files.E, 5).Square));

            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.Board.GetSquare(Files.D, 7), null, true, true, true, true);

            Assert.AreEqual(4, moves.Count);
        }

        [TestMethod]
        public void SampleFailure()
        {
            var engine = new Engine(new ScorerConfiguration()
            {
                SelfInCheckScore = -100,
                BishopValue = 40,
                KnightValue = 40,
                OpponentInCheckScore = 100,
                CenterSquareValue = 6,
                CenterBorderValue = 3,
                PawnValue = 18,
                KingValue = 99999,
                MaxTimeMilliseconds = 5000,
                QueenValue = 120,
                RookValue = 80,
                StartingDepth = 1
            });
            Game.ResetGame();
            ExecuteMove("E2 E4");
            ExecuteMove("C7 C6");
            ExecuteMove("G1 F3");
            ExecuteMove("D8 B6");
            ExecuteMove("F1 C4");
            ExecuteMove("B6 B4");
            ExecuteMove("C2 C3");
            ExecuteMove("B4 C4");
            ExecuteMove("D2 D3");
            ExecuteMove("B7 B6");
            ExecuteMove("D3 C4");
            ExecuteMove("C8 A6");
            ExecuteMove("F3 E5");
            ExecuteMove("D7 D6");
            ExecuteMove("C1 G5");
            ExecuteMove("D6 E5");
            ExecuteMove("B1 D2");
            ExecuteMove("B8 D7");
            ExecuteMove("D2 F3");
            ExecuteMove("E7 E6");
            ExecuteMove("F3 E5");
            ExecuteMove("D7 E5");
            ExecuteMove("E1 G1");
            ExecuteMove("A6 C4");
            ExecuteMove("F1 E1");
            ExecuteMove("F8 C5");
            ExecuteMove("D1 D2");
            ExecuteMove("A8 B8");
            ExecuteMove("E1 D1");
            ExecuteMove("A7 A6");
            ExecuteMove("G5 F4");
            ExecuteMove("F7 F6");
            ExecuteMove("F4 E5");
            ExecuteMove("F6 E5");
            ExecuteMove("D2 D7");

            Game.GetAllLegalMoves();
            engine.GetBestMove(Game, Game.PlayerToMove);
        }

        private void ExecuteMove(string move)
        {
            var startingSquare = move.Split(' ')[0];
            var destinationSquare = move.Split(' ')[1];

            var startingFile = Enum.Parse<Files>(startingSquare[0].ToString());
            var destinationFile = Enum.Parse<Files>(destinationSquare[0].ToString());

            var startingRank = int.Parse(startingSquare[1].ToString());
            var destinationRank = int.Parse(destinationSquare[1].ToString());

            var squareObject = Game.Board.GetSquare(startingFile, startingRank);
            var piece = squareObject.Piece;
            var color = piece.Color;
            var destinationSq = Game.Board.GetSquare(destinationFile, destinationRank).Square;
            Game.AddMove(new Move(squareObject.Piece, color, squareObject.Square, destinationSq));
        }

    }
}
