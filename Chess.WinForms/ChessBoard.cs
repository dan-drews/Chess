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
    public partial class ChessBoard : UserControl
    {
        public Action<(Engine.NodeInfo? node, int depth)>? OnMoveCalculated;

        const int SQUARE_SIZE = 100;
        private Color _whiteColor = Color.FromArgb(250, 244, 212);
        private Color _blackColor = Color.FromArgb(66, 22, 0);
        public Game Game { get; set; }
        public Engine? WhiteEngine;
        public Engine? BlackEngine;

        bool _isRendered = false;
        bool _areMovesRendered = false;
        private (Files File, int Rank)? _selectedSquare = null;
        private List<Move>? _moves = null;

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

        private void OnClick(object? sender, EventArgs e)
        {
            MouseEventArgs mouseEvent = (MouseEventArgs)e;
            var squareTarget = GetSquareRankFileFromEventArgs(mouseEvent);
            if (_selectedSquare == null)
            {
                var moves = Game.GetAllLegalMoves().Where(x => x.StartingSquare.Rank == squareTarget.Rank && x.StartingSquare.File == squareTarget.File).ToList();
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
                    var matchingMoveQuery = _moves.Where(x => x.DestinationSquare.Rank == squareTarget.Rank && x.DestinationSquare.File == squareTarget.File);
                    if (matchingMoveQuery.Any())
                    {
                        Game.AddMove(matchingMoveQuery.First());
                        _isRendered = false;
                        this.Refresh();
                        if (Game.PlayerToMove == Colors.Black && !Game.IsCheckmate)
                        {
                            var evaluationResult = BlackEngine.GetBestMove(Game, Colors.Black);
                            OnMoveCalculated(evaluationResult);
                            var move = evaluationResult.node.Move;
                            Game.AddMove(move);
                            _isRendered = false;
                            this.Refresh();
                        }
                    }
                }
                _isRendered = false;
                this.Refresh();
                _moves = null;
                _selectedSquare = null;
            }

        }

        private void RenderMoves(List<Move> moves)
        {
            _areMovesRendered = true;
            foreach (var move in moves)
            {
                DrawCircle(Color.Gray, move.DestinationSquare.Rank, (int)move.DestinationSquare.File);
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

        private (Files File, int Rank) GetSquareRankFileFromEventArgs(MouseEventArgs e)
        {
            var position = e.Location;
            var rank = 8 - (position.Y / SQUARE_SIZE);
            var file = position.X / SQUARE_SIZE + 1;
            return ((Files)file, rank);
        }

    }
}
