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
using static ChessLibrary.Engine;

namespace Chess.WinForms
{
    public partial class ChessBoard : UserControl
    {
        public Action<(Engine.NodeInfo? node, int depth)>? OnMoveCalculated;
        private bool _hasSimulationStarted = false;
        const int SQUARE_SIZE = 100;
        private Color _whiteColor = Color.FromArgb(250, 244, 212);
        private Color _blackColor = Color.FromArgb(66, 22, 0);
        private Color _lastMoveColor = Color.FromArgb(99, 237, 255);
        public Game Game { get; set; }
        public Engine? WhiteEngine;
        public Engine? BlackEngine;

        bool _isRendered = false;
        bool _areMovesRendered = false;
        private Square? _selectedSquare = null;
        private List<Move>? _moves = null;

        public bool IsWhiteAi { get; set; } = false;
        public bool IsBlackAi { get; set; } = false;

        public MainGame? MainGame { get; set; }

        public ChessBoard()
        {
            Game = new Game(ChessLibrary.Enums.BoardType.BitBoard);
            this.Click += OnClick;
            InitializeComponent();
        }

        public void ForceRender()
        {
            _isRendered = false;
            this.Refresh();
        }

        public ChessBoard(Game game, Engine? whiteEngine, Engine? blackEngine)
        {
            this.Click += OnClick;
            Game = game;
            InitializeComponent();
            WhiteEngine = whiteEngine;
            BlackEngine = blackEngine;
        }

        private (NodeInfo? node, int Depth) ExecuteNextMoveForComputer()
        {
            (NodeInfo? node, int Depth) result;
            if (Game.PlayerToMove == Colors.White)
            {
                result = WhiteEngine.GetBestMove(Game, Colors.White);
            }
            else
            {
                result = BlackEngine.GetBestMove(Game, Colors.Black);
            }
            return result;
        }

        public void SimulateGame()
        {
            if (_hasSimulationStarted)
            {
                return;
            }
            _hasSimulationStarted = true;
            if (IsCurrentPlayerAnAi())
            {
                backgroundWorker1.RunWorkerAsync();
            }
            //while (!Game.IsGameOver)
            //{
            //    //ExecuteNextMoveForComputer();
            //}
        }

        private void OnClick(object? sender, EventArgs e)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs)e;
            var squareTarget = GetSquareRankFileFromEventArgs(mouseEvent);
            if (_selectedSquare == null)
            {
                var moves = Game.GetAllLegalMoves().Where(x => x.StartingSquare == squareTarget.SquareNumber).ToList();
                if (moves.Any())
                {
                    RenderMoves(moves);
                    _selectedSquare = squareTarget;
                    _moves = moves;
                }
                else
                {
                    _isRendered = false;
                    if (_areMovesRendered)
                    {
                        this.Refresh();
                    }
                    _selectedSquare = null;
                    _moves = null;
                }
            }
            else
            {
                if (_moves != null)
                {
                    var matchingMoveQuery = _moves.Where(x => x.TargetSquare == squareTarget.SquareNumber);
                    if (matchingMoveQuery.Any())
                    {
                        Game.AddMove(matchingMoveQuery.First());
                        if (this.MainGame != null)
                        {
                            this.MainGame.Seconds = 0;
                        }
                        ForceRender();
                        if (IsCurrentPlayerAnAi() && !Game.IsCheckmate)
                        {
                            backgroundWorker1.RunWorkerAsync();
                            //var evaluationResult = BlackEngine.GetBestMove(Game, Colors.Black);
                            //OnMoveCalculated(evaluationResult);
                            //var move = evaluationResult.node.Move;
                            //if (!Game.IsGameOver)
                            //{
                            //    Game.AddMove(move);
                            //    _isRendered = false;
                            //    this.Refresh();
                            //}
                        }
                    }
                }
                else
                {
                    ForceRender();
                }
                    _moves = null;
                    _selectedSquare = null;
            }

        }

        private void RenderMoves(List<Move> moves)
        {
            _areMovesRendered = true;
            foreach (var move in moves)
            {
                var destinationSquare = new Square(move.TargetSquare);
                DrawCircle(Color.Gray, destinationSquare.Rank, (int)destinationSquare.File);
            }
        }

        void DrawCircle(Color color, int rank, int file)
        {
            using var brush = new System.Drawing.SolidBrush(color);
            using System.Drawing.Graphics formGraphics = this.CreateGraphics();

            var transposedFile = file - 1;
            var transposedRank = 8 - rank;

            var xStart = transposedFile * SQUARE_SIZE;
            var xEnd = xStart + SQUARE_SIZE;
            var x = xStart + ((xEnd - xStart) / 2);

            var yStart = transposedRank * SQUARE_SIZE;
            var yEnd = yStart + SQUARE_SIZE;
            var y = yStart + ((yEnd - yStart) / 2);

            FillCircle(formGraphics, brush, x, y, 10);
        }

        public void FillCircle(Graphics g, Brush brush,
                                  float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        private void ChessBoard_OnPaint(object sender, PaintEventArgs e)
        {
            //SimulateGame();
            if (!_isRendered)
            {
                DrawBoard();
                RenderPieces(e);
            }
            _isRendered = true;
        }

        void DrawBoard()
        {
            this.Size = new Size(SQUARE_SIZE * 8, SQUARE_SIZE * 8);
            this.ParentForm.Size = new Size(this.Size.Width, this.Size.Height + 200);
            for (int rank = 1; rank <= 8; rank++)
            {
                for (int file = 1; file <= 8; file++)
                {
                    bool isLightSquare = (rank + file) % 2 != 0;

                    var squareColor = isLightSquare ? _whiteColor : _blackColor;

                    var sq = new Square()
                    {
                        File = (Files)file,
                        Rank = rank,
                    };

                    if (Game.Moves.Any())
                    {
                        var lastMove = Game.Moves.Last();
                        bool isLastMoveStartSquare = lastMove.StartingSquare == sq.SquareNumber;
                        bool isLastMoveDestinationSquare = lastMove.TargetSquare == sq.SquareNumber;
                        if (isLastMoveStartSquare || isLastMoveDestinationSquare)
                        {
                            squareColor = _lastMoveColor;
                        }
                    }

                    DrawSquare(squareColor, rank, file);
                }
            }
        }

        void DrawSquare(Color color, int rank, int file)
        {
            using var brush = new System.Drawing.SolidBrush(color);
            using System.Drawing.Graphics formGraphics = this.CreateGraphics();

            var transposedFile = file - 1;
            var transposedRank = 8 - rank;
            formGraphics.FillRectangle(brush, new Rectangle(transposedFile * SQUARE_SIZE, transposedRank * SQUARE_SIZE, SQUARE_SIZE, SQUARE_SIZE));
        }

        private void RenderPieces(PaintEventArgs e)
        {
            for (int rank = 1; rank <= 8; rank++)
            {
                for (int file = 1; file <= 8; file++)
                {
                    var square = Game.Board.GetSquare((Files)file, rank);
                    if (square.Piece != null)
                    {
                        Image image = Image.FromFile(GetImagePathForPiece(square.Piece));
                        Size bitmapSize = new Size(SQUARE_SIZE / 2, SQUARE_SIZE / 2);
                        PictureBox pictureBox = new PictureBox();
                        pictureBox.Image = image;
                        pictureBox.ClientSize = bitmapSize;
                        pictureBox.BackColor = Color.Transparent;


                        var transposedFile = file - 1;
                        var transposedRank = 8 - rank;

                        var location = new Point((transposedFile * SQUARE_SIZE) + (SQUARE_SIZE / 4), (transposedRank * SQUARE_SIZE) + (SQUARE_SIZE / 4));

                        e.Graphics.DrawImage(image, location);

                        //this.Controls.Add(pictureBox);
                    }
                }
            }
        }

        private string GetImagePathForPiece(Piece piece)
        {
            string colorPath = piece.Color switch
            {
                Colors.White => "White",
                Colors.Black => "Black",
                _ => throw new NotImplementedException()
            };
            string typePath = piece.Type switch
            {
                PieceTypes.Pawn => "Pawn",
                PieceTypes.Rook => "Rook",
                PieceTypes.Knight => "Knight",
                PieceTypes.Bishop => "Bishop",
                PieceTypes.Queen => "Queen",
                PieceTypes.King => "King",
                _ => throw new NotImplementedException()
            };
            return $"assets/{colorPath}{typePath}.png";
        }

        private Square GetSquareRankFileFromEventArgs(MouseEventArgs e)
        {
            var position = e.Location;
            var rank = 8 - (position.Y / SQUARE_SIZE);
            var file = position.X / SQUARE_SIZE + 1;
            return new Square() { File = (Files)file, Rank = rank };// ((Files)file, rank);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var result = ExecuteNextMoveForComputer();
            e.Result = result;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (NodeInfo? node, int Depth) result = ((NodeInfo?, int))e.Result;
            if (result.node != null)
            {
                OnMoveCalculated(result);
                var move = result.node.Move;
                if (!Game.IsGameOver)
                {

                    Game.AddMove(move!.Value);
                    if (this.MainGame != null)
                    {
                        this.MainGame.Seconds = 0;
                    }
                    ForceRender();
                    if (IsCurrentPlayerAnAi())
                    {
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else
                    {
                        _hasSimulationStarted = false;
                    }
                }
            }
        }

        private bool IsCurrentPlayerAnAi()
        {
            return (Game.PlayerToMove == Colors.White && IsWhiteAi) || (Game.PlayerToMove == Colors.Black && IsBlackAi);
        }

    }
}
