using ChessLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess.WinForms
{
    public partial class MainGame : Form
    {
        private Game _game;
        private Engine? _whiteEngine;
        private Engine? _blackEngine;

        public MainGame()
        {
            _game = new Game(ChessLibrary.Enums.BoardType.BitBoard);
            var whiteConfig = new ScorerConfiguration()
            {
                SelfInCheckScore = 0,
                BishopValue = 100,
                KnightValue = 101,
                OpponentInCheckScore = 0,
                CenterSquareValue = 30,
                CenterBorderValue = 10,
                PawnValue = 40,
                KingValue = 99999,
                MaxTimeMilliseconds = 10000,
                QueenValue = 900,
                RookValue = 300,
                StartingDepth = 1
            };

            var blackConfig = new ScorerConfiguration()
            {
                SelfInCheckScore = 0,
                BishopValue = 3,
                KnightValue = 3,
                OpponentInCheckScore = 0,
                CenterSquareValue = 0,
                CenterBorderValue = 0,
                PawnValue = 1,
                KingValue = 99999,
                MaxTimeMilliseconds = 10000,
                QueenValue = 9,
                RookValue = 5,
                StartingDepth = 1
            };
            _whiteEngine = new Engine(whiteConfig);
            _blackEngine = new Engine(blackConfig);
            _game.ResetGame();
            InitializeComponent();
        }


    }
}
