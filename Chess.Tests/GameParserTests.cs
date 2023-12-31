using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chess.Tests
{
    [TestClass]
    public class GameParserTests
    {
        [TestMethod]
        public void Parse()
        {
            var games = System.IO.File.ReadAllText(System.IO.Path.Combine("GameList", "Games.txt"));
            var parser = new MatchParser();
            foreach (var game in games.Split("\n").Where(x => x.Length > 0))
            {
                Game g = new Game(ChessLibrary.Enums.BoardType.BitBoard);
                g.ResetGame();

                parser.LoadGameFromChessNotation(g, game);

                //var moves = game.Split(' ')[0..^2];

                //foreach (var move in moves)
                //{
                //    var m = parser.GetMoveFromChessNotation(g, move);
                //    g.AddMove(m);
                //}
            }
        }
    }
}
