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
        public void GetValidMoves_AfterE4E5()
        {
            var g = new Game();
            g.ResetGame();
            g.AddMove(new Move(null, Colors.White, new Square() { File = Files.E, Rank = 2 }, new Square() { File = Files.E, Rank = 4 }));
            g.AddMove(new Move(null, Colors.Black, new Square() { File = Files.E, Rank = 7 }, new Square() { File = Files.E, Rank = 5 }));
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

            Assert.AreEqual(29, count);
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
        public void GetValidMoves_ForRookWithBlockedSpace_Provides10Moves()
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

        [TestMethod]
        public void GetValidMoves_PawnAttacking_DisallowsCheck()
        {
            var g = new Game();
            g.ResetGame();

            g.AddMove(new Move(g.Board.GetSquare(Files.B, 1).Piece, Colors.White, g.Board.GetSquare(Files.B, 1).Square, g.Board.GetSquare(Files.C, 3).Square));
            g.AddMove(new Move(g.Board.GetSquare(Files.D, 7).Piece, Colors.Black, g.Board.GetSquare(Files.D, 7).Square, g.Board.GetSquare(Files.D, 5).Square));

            g.AddMove(new Move(g.Board.GetSquare(Files.G, 1).Piece, Colors.White, g.Board.GetSquare(Files.G, 1).Square, g.Board.GetSquare(Files.F, 3).Square));
            g.AddMove(new Move(g.Board.GetSquare(Files.G, 8).Piece, Colors.Black, g.Board.GetSquare(Files.G, 8).Square, g.Board.GetSquare(Files.F, 6).Square));

            g.AddMove(new Move(g.Board.GetSquare(Files.H, 1).Piece, Colors.White, g.Board.GetSquare(Files.H, 1).Square, g.Board.GetSquare(Files.G, 1).Square));
            g.AddMove(new Move(g.Board.GetSquare(Files.F, 6).Piece, Colors.Black, g.Board.GetSquare(Files.F, 6).Square, g.Board.GetSquare(Files.G, 4).Square));

            g.AddMove(new Move(g.Board.GetSquare(Files.C, 3).Piece, Colors.White, g.Board.GetSquare(Files.C, 3).Square, g.Board.GetSquare(Files.D, 5).Square) { CapturedPiece = g.Board.GetSquare(Files.D, 5).Piece });
            g.AddMove(new Move(g.Board.GetSquare(Files.F, 7).Piece, Colors.Black, g.Board.GetSquare(Files.F, 7).Square, g.Board.GetSquare(Files.F, 6).Square));

            g.AddMove(new Move(g.Board.GetSquare(Files.D, 5).Piece, Colors.White, g.Board.GetSquare(Files.D, 5).Square, g.Board.GetSquare(Files.C, 7).Square) { CapturedPiece = g.Board.GetSquare(Files.C, 7).Piece });
            g.AddMove(new Move(g.Board.GetSquare(Files.E, 8).Piece, Colors.Black, g.Board.GetSquare(Files.E, 8).Square, g.Board.GetSquare(Files.D, 7).Square));

            g.AddMove(new Move(g.Board.GetSquare(Files.E, 2).Piece, Colors.White, g.Board.GetSquare(Files.E, 2).Square, g.Board.GetSquare(Files.E, 4).Square));
            g.AddMove(new Move(g.Board.GetSquare(Files.D, 8).Piece, Colors.Black, g.Board.GetSquare(Files.D, 8).Square, g.Board.GetSquare(Files.C, 7).Square) { CapturedPiece = g.Board.GetSquare(Files.C, 7).Piece });

            g.AddMove(new Move(g.Board.GetSquare(Files.E, 4).Piece, Colors.White, g.Board.GetSquare(Files.E, 4).Square, g.Board.GetSquare(Files.E, 5).Square));

            var moves = MoveLegalityEvaluator.GetAllLegalMoves(g.Board, g.Board.GetSquare(Files.D, 7));

            Assert.AreEqual(4, moves.Count);
        }
    }
}
