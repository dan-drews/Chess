using ChessLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class BitBoardEvaluatorTests : LegalityTestBase
    {
        private Game _game;
        public override Game Game
        {
            get { return _game; }
        }

        [TestInitialize()]
        public void Initialize()
        {
            _game = new Game(ChessLibrary.Enums.BoardType.BitBoard);
        }
    }
}
