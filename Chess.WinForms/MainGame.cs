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
                MaxTimeMilliseconds = 1000,
                QueenValue = 900,
                RookValue = 300,
                StartingDepth = 1
            };

            var blackConfig = new ScorerConfiguration()
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
            };
            _whiteEngine = new Engine(whiteConfig);
            _blackEngine = new Engine(blackConfig);
            _game.ResetGame();
            InitializeComponent();
            chessBoard.Game = _game;
            chessBoard.BlackEngine = _blackEngine;
            chessBoard.WhiteEngine = _whiteEngine;
            chessBoard.OnMoveCalculated = (evaluationResult) =>
            {
                label1.Text = $"Depth: {evaluationResult.depth}, Score: {evaluationResult.node?.Score}, Nodes Evaluated: {Engine.nodesEvaluated}, NonQuiet Nodes Evaulated: {Engine.nonQuietDepthNodesEvaluated}, Zobrist Hash Matches : {Engine.zobristMatches}";
            };
        }

        private void btnLoadFen_Click(object sender, EventArgs e)
        {
            var fen = txtFen.Text;
            _game.LoadFen(fen);
            chessBoard.ForceRender();
        }
    }
}
