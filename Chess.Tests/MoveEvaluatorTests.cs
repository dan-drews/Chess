using ChessLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class MoveEvaluatorTests
    {
        [TestMethod]
        public void GetValidMoves_ForAllWhiteStartingPieces_Provides20Moves()
        {
            var g = new Game();
            g.ResetGame();
            int count = 0;
            for (Files file = Files.A; file <= Files.H; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    var square = g.Board.GetSquare(file, rank);
                    if (square?.Piece?.Color == Colors.White)
                    {
                        var legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(g.Board, square);
                        count += legalMoves?.Count ?? 0;
                    }
                }
            }

            Assert.AreEqual(20, count);
        }

        [TestMethod]
        public void GetValidMoves_ForRookWithEmptyBoardAndRandomKing_Provides14Moves()
        {
            var g = new Game();
            g.Board.GetSquare(Files.B, 5).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Rook };
            g.Board.GetSquare(Files.H, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.King };
            g.Board.GetSquare(Files.H, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.King };


            var moves = MoveLegalityEvaluator.GetAllLegalMoves(g.Board, g.Board.GetSquare(Files.B, 5));

            Assert.AreEqual(14, moves.Count);
        }

        [TestMethod]
        public void GetValidMoves_ForRookWithBlockedSpace_Provides20Moves()
        {
            var g = new Game();
            g.Board.GetSquare(Files.B, 5).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Rook };
            g.Board.GetSquare(Files.B, 4).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Pawn };
            g.Board.GetSquare(Files.H, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.King };
            g.Board.GetSquare(Files.H, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.King };


            var moves = MoveLegalityEvaluator.GetAllLegalMoves(g.Board, g.Board.GetSquare(Files.B, 5));

            Assert.AreEqual(10, moves.Count);
        }

        [TestMethod]
        public void GetValidMoves_TwoRooks_DisallowsCheck()
        {
            var g = new Game();
            g.Board.GetSquare(Files.H, 3).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Rook };
            g.Board.GetSquare(Files.H, 6).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Rook };
            g.Board.GetSquare(Files.H, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.King };
            g.Board.GetSquare(Files.H, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.King };


            var moves = MoveLegalityEvaluator.GetAllLegalMoves(g.Board, g.Board.GetSquare(Files.H, 3));

            Assert.AreEqual(4, moves.Count);
        }
    }
}
