using ChessLibrary.Enums;
using ChessLibrary.MoveLegaility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLibrary
{
    public class Game : ICloneable
    {

        public IMoveLegality Evaluator { get; private set; }
        public IBoard Board { get; private set; }

        public bool WhiteCanLongCastle { get; set; } = true;
        public bool BlackCanLongCastle { get; set; } = true;
        public bool WhiteCanShortCastle { get; set; } = true;
        public bool BlackCanShortCastle { get; set; } = true;
        public Files? EnPassantFile { get; set; } = null;

        public Colors StartingColor { get; set; } = Colors.White;

        private Game(IMoveLegality evaluator, IBoard board)
        {
            Evaluator = evaluator;
            Board = board;
        }

        public Game(BoardType boardType)
        {
            switch (boardType)
            {
                case BoardType.BitBoard:
                    Board = new BitBoard();
                    Evaluator = new BitBoardLegality();
                    break;
                case BoardType.Naive:
                    Board = new NaiveBoard();
                    Evaluator = new NaiveMoveLegality();
                    break;
                default:
                    throw new Exception("Board Type Not Supported");
            }
        }

        public object Clone()
        {
            return new Game(Evaluator, (IBoard)Board.Clone())
            {
                Moves = Moves.Select(x => (Move)x.Clone()).ToList(),
                WhiteCanLongCastle = WhiteCanLongCastle,
                WhiteCanShortCastle = WhiteCanShortCastle,
                BlackCanLongCastle = BlackCanLongCastle,
                BlackCanShortCastle = BlackCanShortCastle,
                StartingColor = StartingColor,
                EnPassantFile = EnPassantFile                
            };
        }

        public Colors PlayerToMove
        {
            get
            {
                if(StartingColor == Colors.White)
                {
                    return Moves.Count % 2 == 0 ? Colors.White : Colors.Black;
                }
                return Moves.Count % 2 == 1 ? Colors.White : Colors.Black;
            }
        }

        public List<Move> Moves { get; private set; } = new List<Move>();


        public bool IsGameOver
        {
            get
            {
                return IsCheckmate || IsStalemate;
            }
        }

        private List<Move>? _legalMoves;

        public bool IsStalemate
        {
            get
            {
                if (_legalMoves == null)
                {
                    _legalMoves = Evaluator.GetAllLegalMoves(Board, PlayerToMove, Moves);
                }
                return !GetAllLegalMoves().Any() && !IsKingInCheck(PlayerToMove);
            }
        }

        public bool IsCheckmate
        {
            get
            {
                if (_legalMoves == null)
                {
                    _legalMoves = Evaluator.GetAllLegalMoves(Board, PlayerToMove, Moves);
                }
                return !GetAllLegalMoves().Any() && IsKingInCheck(PlayerToMove);
            }
        }

        public List<Move> GetAllLegalMoves()
        {
            if (_legalMoves == null)
            {
                _legalMoves = Evaluator.GetAllLegalMoves(Board, PlayerToMove, Moves);
            }
            return _legalMoves;
        }

        private bool? _isBlackKingInCheck = null;
        private bool? _isWhiteKingInCheck = null;

        public bool IsKingInCheck(Colors color)
        {
            if (color == Colors.Black)
            {
                if (_isBlackKingInCheck == null)
                {
                    _isBlackKingInCheck = Evaluator.IsKingInCheck(Board, color, Moves);
                }
                return _isBlackKingInCheck.Value;
            }
            else
            {
                if (_isWhiteKingInCheck == null)
                {
                    _isWhiteKingInCheck = Evaluator.IsKingInCheck(Board, color, Moves);
                }
                return _isWhiteKingInCheck.Value;
            }
        }

        public void ResetGame()
        {
            Moves = new List<Move>();

            // Clear Board
            for (Files f = Files.A; f <= Files.H; f++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    Board.ClearPiece(f, rank);
                }
            }
            SetupBoard();
        }

        public IBoard AddMove(Move move, bool validate = true)
        {
            var startingSquare = Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank);
            if (startingSquare.Piece == null)
            {
                throw new Exception("No piece to move");
            }
            if (startingSquare.Piece.Color != PlayerToMove || startingSquare.Piece.Color != move.Player)
            {
                throw new Exception("Wrong Color Moving");
            }
            List<Move>? legalMoves = null;
            if (validate)
            {
                legalMoves = Evaluator.GetAllLegalMoves(Board, startingSquare, Moves);
                if (legalMoves == null)
                {
                    throw new Exception("Invalid move");
                }
            }
            if (!validate || legalMoves!.Any(x => x.Equals(move)))
            {
                if (validate)
                {
                    Moves.Add(legalMoves![legalMoves.IndexOf(move)]);
                }
                else
                {
                    Moves.Add(move);
                }
                Board.MovePiece(move);

                _legalMoves = null;
                _isWhiteKingInCheck = null;
                _isBlackKingInCheck = null;
                return Board;
            }
            else
            {
                throw new Exception("Invalid Move");
            }
        }

        public IBoard UndoLastMove()
        {
            var move = Moves.Last();
            var startingSquare = Board.GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank);
            if (startingSquare.Piece == null)
            {
                throw new Exception("No piece to move");
            }
            var initialPiece = move.Piece;
            bool hasPerformedMove = false;
            if (initialPiece != null && initialPiece.Type == PieceTypes.Pawn)
            {
                var startingRank = initialPiece.Color == Colors.Black ? 4 : 5;
                var moveDirection = initialPiece.Color == Colors.Black ? -1 : 1;
                if (move.StartingSquare.Rank == startingRank && move.DestinationSquare.File != move.StartingSquare.File)
                {
                    // Pawn capture... but is it en passant?
                    if(move.DestinationSquare.Rank == startingRank + moveDirection && Math.Abs(move.DestinationSquare.File - move.StartingSquare.File) == 1)
                    {
                        // Moved diagonally, but still, was it an en passant?
                        var moveBeforeLast = Moves.ElementAt(Moves.Count - 2);
                        if (moveBeforeLast.Piece.Type == PieceTypes.Pawn 
                            && moveBeforeLast.StartingSquare.Rank == startingRank + (moveDirection * 2) 
                            && moveBeforeLast.DestinationSquare.Rank == startingRank
                            && moveBeforeLast.DestinationSquare.File == move.DestinationSquare.File)
                        {
                            // Yup, it was en passant.
                            Board.SetPiece(moveBeforeLast.DestinationSquare.File, moveBeforeLast.DestinationSquare.Rank, moveBeforeLast.Piece.Type, moveBeforeLast.Piece.Color);
                            Board.SetPiece(move.StartingSquare.File, move.StartingSquare.Rank, initialPiece.Type, initialPiece.Color);
                            Board.ClearPiece(move.DestinationSquare.File, move.DestinationSquare.Rank);
                            hasPerformedMove = true;
                        }
                    }
                }
            }
            if (!hasPerformedMove)
            {
                startingSquare.Piece = move.CapturedPiece;
                if (move.CapturedPiece != null)
                {
                    Board.SetPiece(startingSquare.Square.File, startingSquare.Square.Rank, move.CapturedPiece.Type, move.CapturedPiece.Color);
                }
                else
                {
                    Board.ClearPiece(startingSquare.Square.File, startingSquare.Square.Rank);
                }
                Board.SetPiece(move.StartingSquare.File, move.StartingSquare.Rank, initialPiece!.Type, initialPiece.Color);

                if (initialPiece != null && initialPiece.Type == PieceTypes.King)
                {
                    var rank = initialPiece.Color == Colors.Black ? 8 : 1;
                    if (move.StartingSquare.Rank == rank && move.StartingSquare.File == Files.E && (move.DestinationSquare.File == Files.G || move.DestinationSquare.File == Files.C))
                    {
                        // Castling.
                        if (move.DestinationSquare.File == Files.G)
                        {
                            Board.SetPiece(Files.H, rank, PieceTypes.Rook, initialPiece.Color);
                            Board.ClearPiece(Files.F, rank);
                        }

                        if (move.DestinationSquare.File == Files.C)
                        {
                            Board.SetPiece(Files.A, rank, PieceTypes.Rook, initialPiece.Color);
                            Board.ClearPiece(Files.D, rank);
                        }
                    }
                }
            }

            _legalMoves = null;
            _isWhiteKingInCheck = null;
            _isBlackKingInCheck = null;
            Moves.RemoveAt(Moves.Count - 1); // can't just remove "Move" because the move equality kicks in.
            return Board;
        }

        private void SetupBoard()
        {
            Board.SetupBoard();
        }
    }
}
