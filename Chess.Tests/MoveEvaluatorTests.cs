using ChessLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class MoveEvaluatorTests
    {
        [TestMethod]
        public void GetValidMoves_ForAllSquares_Provides20Moves()
        {
            var g = new Game();
            g.ResetGame();
            int count = 0;
            for(Files file = Files.A; file <= Files.H; file++)
            {
                for(int rank = 1; rank <= 8; rank++)
                {
                    var square = g.Board.GetSquare(file, rank);
                    if(square?.Piece?.Color == Colors.White)
                    {
                        var legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(g.Board, square);
                        count += legalMoves?.Count ?? 0;
                    }
                }
            }

            Assert.AreEqual(20, count);
        }
    }
}
