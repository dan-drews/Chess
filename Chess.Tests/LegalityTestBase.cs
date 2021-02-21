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
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, new System.Collections.Generic.List<Move>()).Count;
            Assert.AreEqual(20, count);
        }

        [TestMethod]
        public void GetValidMoves_ForAllWhiteStartingPieces_InLoop_Provides20Moves()
        {
            int count = 0;
            for (int i = 0; i < 1_000_000; i++)
            {
                Game.ResetGame();
                count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, new System.Collections.Generic.List<Move>()).Count;
            }
            Assert.AreEqual(20, count);
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

            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, new List<Move>());
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
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, new System.Collections.Generic.List<Move>()).Count;
            Assert.AreEqual(24, count);
        }

        [TestMethod]
        public void GetValidMoves_InitialSetup_AddedRook()
        {
            Game.ResetGame();
            Game.Board.SetPiece(Files.D, 4, PieceTypes.Rook, Colors.White);
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, new System.Collections.Generic.List<Move>()).Count;
            Assert.AreEqual(30, count);
        }

        [TestMethod]
        public void GetValidMoves_InitialSetup_AddedQueen()
        {
            Game.ResetGame();
            Game.Board.SetPiece(Files.D, 4, PieceTypes.Queen, Colors.White);
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, new System.Collections.Generic.List<Move>()).Count;
            Assert.AreEqual(38, count);
        }

        [TestMethod]
        public void GetValidMoves_WithNoPawnInFrontOfBishop()
        {
            Game.ResetGame();
            Game.Board.ClearPiece(Files.E, 2);
            int count = Game.Evaluator.GetAllLegalMoves(Game.Board, Colors.White, new System.Collections.Generic.List<Move>()).Count;
            Assert.AreEqual(29, count);
        }

        [TestMethod]
        public void GetValidMoves_AfterE4E5()
        {
            Game.ResetGame();
            Game.AddMove(new Move(null, Colors.White, new Square() { File = Files.E, Rank = 2 }, new Square() { File = Files.E, Rank = 4 }));
            Game.AddMove(new Move(null, Colors.Black, new Square() { File = Files.E, Rank = 7 }, new Square() { File = Files.E, Rank = 5 }));
            int count = 0;
            for (Files file = Files.A; file <= Files.H; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var square = Game.Board.GetSquare(file, rank);
                    if (square?.Piece?.Color == Colors.White)
                    {
                        var legalMoves = Game.Evaluator.GetAllLegalMoves(Game.Board, square, Game.Moves);
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


            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.Board.GetSquare(Files.B, 5), Game.Moves);

            Assert.AreEqual(14, moves.Count);
        }

        [TestMethod]
        public void GetValidMoves_ForRookWithBlockedSpace_Provides10Moves()
        {

            Game.Board.SetPiece(Files.B, 5, PieceTypes.Rook, Colors.White);
            Game.Board.SetPiece(Files.B, 4, PieceTypes.Pawn, Colors.White);
            Game.Board.SetPiece(Files.H, 1, PieceTypes.King, Colors.White);
            Game.Board.SetPiece(Files.H, 8, PieceTypes.King, Colors.Black);


            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.Board.GetSquare(Files.B, 5), Game.Moves);

            Assert.AreEqual(10, moves.Count);
        }

        [TestMethod]
        public void GetValidMoves_TwoRooks_DisallowsCheck()
        {
            Game.Board.SetPiece(Files.H, 3, PieceTypes.Rook, Colors.White);
            Game.Board.SetPiece(Files.H, 6, PieceTypes.Rook, Colors.Black);
            Game.Board.SetPiece(Files.H, 1, PieceTypes.King, Colors.White);
            Game.Board.SetPiece(Files.H, 8, PieceTypes.King, Colors.Black);


            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.Board.GetSquare(Files.H, 3), Game.Moves);

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

            var moves = Game.Evaluator.GetAllLegalMoves(Game.Board, Game.Board.GetSquare(Files.D, 7), Game.Moves);

            Assert.AreEqual(4, moves.Count);
        }

    }
}
