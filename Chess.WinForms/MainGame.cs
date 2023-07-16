using ChessLibrary;
using ChessLibrary.Evaluation;

namespace Chess.WinForms
{
    public partial class MainGame : Form
    {
        private Game _game;
        private Engine? _whiteEngine;
        private Engine? _blackEngine;
        public int Seconds { get; set; }

        public MainGame()
        {
            _game = new Game(ChessLibrary.Enums.BoardType.BitBoard);
            //var whiteConfig = new ScorerConfiguration()
            //{
            //    SelfInCheckScore = 0,
            //    BishopValue = 100,
            //    KnightValue = 101,
            //    OpponentInCheckScore = 0,
            //    CenterSquareValue = 30,
            //    CenterBorderValue = 10,
            //    PawnValue = 40,
            //    KingValue = 99999,
            //    MaxTimeMilliseconds = 10000, // Int32.MaxValue, //2500,
            //    QueenValue = 900,
            //    RookValue = 300,
            //    StartingDepth = 1
            //};

            var whiteConfig = new ScorerConfiguration()
            {
                SelfInCheckScore = -100,
                BishopValue = 320,
                KnightValue = 325,
                OpponentInCheckScore = 40,
                CenterSquareValue = 60,
                CenterBorderValue = 30,
                PawnValue = 120,
                KingValue = 99999,
                MaxTimeMilliseconds = 10_000, //300_000,// Int32.MaxValue, //10000,
                QueenValue = 900,
                RookValue = 600,
                StartingDepth = 1,
                MaxDepth = null
            };

            var blackConfig = new ScorerConfiguration()
            {
                SelfInCheckScore = -100,
                BishopValue = 320,
                KnightValue = 325,
                MaxDepth = null,
                OpponentInCheckScore = 40,
                CenterSquareValue = 60,
                CenterBorderValue = 30,
                PawnValue = 120,
                KingValue = 99999,
                MaxTimeMilliseconds = 40_000, //Int32.MaxValue, //10000,
                QueenValue = 900,
                RookValue = 600,
                StartingDepth = 1,
                //MaxDepth == null
            };
            _whiteEngine = new Engine(whiteConfig);
            _blackEngine = new Engine(blackConfig);
            _game.ResetGame();
            InitializeComponent();
            chessBoard.Game = _game;
            chessBoard.BlackEngine = _blackEngine;
            chessBoard.WhiteEngine = _whiteEngine;
            chessBoard.MainGame = this;
            chessBoard.OnMoveCalculated = (evaluationResult) =>
            {
                label1.Text = $"Depth: {evaluationResult.depth}, Score: {evaluationResult.node?.Score}, Nodes Evaluated: {Engine.nodesEvaluated}, NonQuiet Nodes Evaulated: {Engine.nonQuietDepthNodesEvaluated}, Zobrist Hash Matches : {Engine.zobristMatches}";
            };
            timer1.Start();
        }

        private void btnLoadFen_Click(object sender, EventArgs e)
        {
            var fen = txtFen.Text;
            _game.LoadFen(fen);
            chessBoard.ForceRender();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chessBoard.SimulateGame();
        }

        private void chkBlackCpu_CheckedChanged(object sender, EventArgs e)
        {
            chessBoard.IsBlackAi = chkBlackCpu.Checked;
        }

        private void chkWhiteCpu_CheckedChanged(object sender, EventArgs e)
        {
            chessBoard.IsWhiteAi = chkWhiteCpu.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Seconds++;
            lblTimer.Text = Seconds.ToString();

        }

        private void MainGame_ResizeEnd(object sender, EventArgs e)
        {
            chessBoard.ForceRender();
        }
    }
}
