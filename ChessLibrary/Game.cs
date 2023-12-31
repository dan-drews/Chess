using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ChessLibrary.Enums;
using ChessLibrary.MoveGeneration;
using ChessLibrary.OpeningBook;
using ChessLibrary.TableStructures;

namespace ChessLibrary
{
    public class Game : ICloneable
    {
        internal ZobristTable ZobristTable { get; set; } = new ZobristTable();
        internal RepetititionTracker RepetititionTracker { get; set; } = new RepetititionTracker();
        public IMoveGenerator MoveGenerator { get; private set; }
        public BitBoard Board { get; private set; }

        public bool WhiteCanLongCastle { get; set; } = true;
        public bool BlackCanLongCastle { get; set; } = true;
        public bool WhiteCanShortCastle { get; set; } = true;
        public bool BlackCanShortCastle { get; set; } = true;

        private Files? _enPassantFile = null;
        public Files? EnPassantFile
        {
            get { return _enPassantFile; }
            set { _enPassantFile = value; }
        }

        public Colors StartingColor { get; set; } = Colors.White;

        private Game(IMoveGenerator evaluator, BitBoard board)
        {
            MoveGenerator = evaluator;
            Board = board;
            MagicSlidingImplementation.InitializeMagicSliders();
        }

        public Game(BoardType boardType, bool useOpeningBook = true)
        {
            MagicSlidingImplementation.InitializeMagicSliders();
            if (useOpeningBook)
            {
                OpeningBookMovePicker.Initialize();
            }
            switch (boardType)
            {
                case BoardType.BitBoard:
                    Board = new BitBoard();
                    MoveGenerator = new MoveGenerator(); //BitBoardMoveGenerator();
                    break;
                default:
                    throw new Exception("Board Type Not Supported");
            }
        }

        public object Clone()
        {
            return new Game(MoveGenerator, (BitBoard)Board.Clone())
            {
                Moves = new List<Move>(Moves),
                WhiteCanLongCastle = WhiteCanLongCastle,
                WhiteCanShortCastle = WhiteCanShortCastle,
                BlackCanLongCastle = BlackCanLongCastle,
                BlackCanShortCastle = BlackCanShortCastle,
                StartingColor = StartingColor,
                EnPassantFile = EnPassantFile,
                RepetititionTracker = (RepetititionTracker)RepetititionTracker.Clone()
            };
        }

        public Colors PlayerToMove
        {
            get
            {
                if (StartingColor == Colors.White)
                {
                    return Moves.Count % 2 == 0 ? Colors.White : Colors.Black;
                }
                return Moves.Count % 2 == 1 ? Colors.White : Colors.Black;
            }
        }

        public List<Move> Moves { get; private set; } = new List<Move>();

        public bool IsGameOver
        {
            get { return IsCheckmate || IsStalemate; }
        }

        private Move[]? _legalMoves;

        public bool IsStalemate
        {
            get
            {
                GetAllLegalMoves();
                //var hash = ZobristTable.CalculateZobristHash(Board);
                //if(Moves.Count(x=> x.Hash == hash) >= 3)
                //{
                //    return true;
                //}

                var hash = ZobristTable.CalculateZobristHash(Board);
                if (RepetititionTracker.Count(hash) >= 3)
                {
                    return true;
                }

                if (Moves.Count > 50)
                {
                    bool isFiftyMoveRule = true;
                    for (int i = 1; i <= 50; i++)
                    {
                        var move = Moves.ElementAt(Moves.Count - i);
                        if (move.Piece == PieceTypes.Pawn || move.CapturedPiece != null)
                        {
                            isFiftyMoveRule = false;
                            break;
                        }
                    }
                    if (isFiftyMoveRule)
                    {
                        return true;
                    }
                }
                return !GetAllLegalMoves().Any() && !IsKingInCheck(PlayerToMove);
            }
        }

        public bool IsCheckmate
        {
            get
            {
                if (!IsKingInCheck(PlayerToMove))
                {
                    return false;
                }
                return !GetAllLegalMoves().Any();
            }
        }

        private Move[]? _legalNonQuietMoves = null;

        public Move[] GetAllLegalMoves(bool includeQuietMoves = true)
        {
            if (includeQuietMoves)
            {
                if (_legalMoves == null)
                {
                    _legalMoves = MoveGenerator.GetAllLegalMoves(
                        Board,
                        PlayerToMove,
                        EnPassantFile,
                        BlackCanLongCastle,
                        BlackCanShortCastle,
                        WhiteCanLongCastle,
                        WhiteCanShortCastle,
                        true
                    );
                }
                return _legalMoves;
            }

            if (_legalNonQuietMoves == null)
            {
                _legalNonQuietMoves = MoveGenerator.GetAllLegalMoves(
                    Board,
                    PlayerToMove,
                    EnPassantFile,
                    BlackCanLongCastle,
                    BlackCanShortCastle,
                    WhiteCanLongCastle,
                    WhiteCanShortCastle,
                    false
                );
            }

            return _legalNonQuietMoves;
        }

        private bool? _isBlackKingInCheck = null;
        private bool? _isWhiteKingInCheck = null;

        public bool IsKingInCheck(Colors color)
        {
            if (color == Colors.Black)
            {
                if (_isBlackKingInCheck == null)
                {
                    _isBlackKingInCheck = MoveGenerator.IsKingInCheck(Board, color);
                }
                return _isBlackKingInCheck.Value;
            }
            else
            {
                if (_isWhiteKingInCheck == null)
                {
                    _isWhiteKingInCheck = MoveGenerator.IsKingInCheck(Board, color);
                }
                return _isWhiteKingInCheck.Value;
            }
        }

        public void LoadFen(string fen)
        {
            Moves.Clear();
            var fenSections = fen.Split(' ');
            var board = fenSections[0];
            var sideToMove = fenSections[1];
            var castlingAbility = fenSections[2];
            var enPassantTargetSquare = fenSections[3];
            var halfMoveClock = fenSections[4];
            var fullMoveCounter = fenSections[5];

            var ranks = board.Split('/');
            RenderBoardFen(ranks);
            StartingColor = sideToMove == "w" ? Colors.White : Colors.Black;
            SetCastlingFen(castlingAbility);
            EnPassantFile = enPassantTargetSquare[0] switch
            {
                'a' => Files.A,
                'b' => Files.B,
                'c' => Files.C,
                'd' => Files.D,
                'e' => Files.E,
                'f' => Files.F,
                'g' => Files.G,
                'h' => Files.H,
                '-' => null,
                _ => throw new NotImplementedException()
            };
        }

        private void SetCastlingFen(string castlingAbility)
        {
            WhiteCanShortCastle = false;
            WhiteCanLongCastle = false;
            BlackCanShortCastle = false;
            BlackCanLongCastle = false;
            if (castlingAbility.Contains("K"))
            {
                WhiteCanShortCastle = true;
            }

            if (castlingAbility.Contains("Q"))
            {
                WhiteCanLongCastle = true;
            }

            if (castlingAbility.Contains("k"))
            {
                BlackCanShortCastle = true;
            }

            if (castlingAbility.Contains("q"))
            {
                BlackCanLongCastle = true;
            }
        }

        private void RenderBoardFen(string[] ranks)
        {
            int currentRank = 8;
            foreach (var rank in ranks)
            {
                int file = 1;
                foreach (var c in rank)
                {
                    if (int.TryParse(c.ToString(), out int numberToSkip))
                    {
                        for (int i = 0; i < numberToSkip; i++)
                        {
                            Board.ClearPiece((Files)file, currentRank);
                            file++;
                        }
                    }
                    else
                    {
                        Colors color;
                        if (c.ToString().ToLower() == c.ToString()) // It's lower-case so black piece
                        {
                            color = Colors.Black;
                        }
                        else
                        {
                            color = Colors.White;
                        }
                        PieceTypes piece = c.ToString().ToUpper() switch
                        {
                            "R" => PieceTypes.Rook,
                            "B" => PieceTypes.Bishop,
                            "N" => PieceTypes.Knight,
                            "K" => PieceTypes.King,
                            "Q" => PieceTypes.Queen,
                            "P" => PieceTypes.Pawn,
                            _ => throw new NotImplementedException()
                        };
                        Board.SetPiece((Files)file, currentRank, piece, color);
                        file++;
                    }
                }
                currentRank--;
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
            _legalMoves = null;
            _legalNonQuietMoves = null;
        }

        public BitBoard AddMove(Move move, bool validate = true)
        {
            if (move == Move.NullMove)
            {
                Moves.Add(move);
                return Board;
            }
            var startingSquare = Board.GetSquare(move.StartingSquare);
            if (startingSquare.Piece == null)
            {
                Debugger.Break();
                throw new Exception("No piece to move");
            }
            if (
                startingSquare.Piece.Color != PlayerToMove
                || startingSquare.Piece.Color != move.Color
            )
            {
                Debugger.Break();
                throw new Exception("Wrong Color Moving");
            }
            Move[]? legalMoves = null;
            if (validate)
            {
                legalMoves = MoveGenerator.GetAllLegalMoves(
                    Board,
                    startingSquare,
                    EnPassantFile,
                    BlackCanLongCastle,
                    BlackCanShortCastle,
                    WhiteCanLongCastle,
                    WhiteCanShortCastle
                );
                if (legalMoves == null)
                {
                    Debugger.Break();
                    throw new Exception("Invalid move");
                }
            }
            if (!validate || legalMoves!.Any(x => x.Equals(move)))
            {
                _legalNonQuietMoves = null;
                _legalMoves = null;
                _isWhiteKingInCheck = null;
                _isBlackKingInCheck = null;
                var sq = Board.GetSquare(move.StartingSquare);
                if (move.Flags == Flag.PawnTwoForward)
                {
                    EnPassantFile = startingSquare.Square.File;
                }
                else
                {
                    EnPassantFile = null;
                }

                if (move.Piece == PieceTypes.King)
                {
                    if (PlayerToMove == Colors.White)
                    {
                        WhiteCanLongCastle = false;
                        WhiteCanShortCastle = false;
                    }
                    else
                    {
                        BlackCanLongCastle = false;
                        BlackCanShortCastle = false;
                    }
                }

                if (move.Piece == PieceTypes.Rook)
                {
                    if (PlayerToMove == Colors.White)
                    {
                        if (sq.Square.Rank == 1)
                        {
                            if (sq.Square.File == Files.A)
                            {
                                WhiteCanLongCastle = false;
                            }
                            if (sq.Square.File == Files.H)
                            {
                                WhiteCanShortCastle = false;
                            }
                        }
                    }
                    else
                    {
                        if (sq.Square.Rank == 8)
                        {
                            if (sq.Square.File == Files.A)
                            {
                                BlackCanLongCastle = false;
                            }
                            if (sq.Square.File == Files.H)
                            {
                                BlackCanShortCastle = false;
                            }
                        }
                    }
                }
                Board.MovePiece(move);
                var hash = ZobristTable.CalculateZobristHash(Board);
                RepetititionTracker.AddHash(hash);
                if (validate)
                {
                    var m = legalMoves![Array.IndexOf(legalMoves, move)];
                    Moves.Add(m);
                }
                else
                {
                    var m = move;
                    Moves.Add(m);
                }
                return Board;
            }
            else
            {
                throw new Exception("Invalid Move");
            }
        }

        public BitBoard UndoLastMove()
        {
            var move = Moves.Last();
            if (move == Move.NullMove)
            {
                Moves.RemoveAt(Moves.Count - 1); // can't just remove "Move" because the move equality kicks in.
                return Board;
            }
            var startingSquare = Board.GetSquare(move.TargetSquare);
            var moveStartingSquare = Board.GetSquare(move.StartingSquare);
            if (startingSquare.Piece == null)
            {
                throw new Exception("No piece to move");
            }
            var initialPiece = move.Piece;
            if (move.Flags == Flag.EnPassantCapture)
            {
                var moveBeforeLast = Moves.ElementAt(Moves.Count - 2);
                // Yup, it was en passant.
                Board.SetPiece(
                    moveBeforeLast.TargetSquare,
                    moveBeforeLast.Piece,
                    moveBeforeLast.Color
                );
                Board.SetPiece(move.StartingSquare, initialPiece, move.Color);
                Board.ClearPiece(move.TargetSquare);
            }
            else
            {
                if (move.CapturedPiece != null)
                {
                    Board.SetPiece(
                        startingSquare.Square.SquareNumber,
                        move.CapturedPiece.Value,
                        move.Color == Colors.White ? Colors.Black : Colors.White
                    );
                }
                else
                {
                    Board.ClearPiece(startingSquare.Square.File, startingSquare.Square.Rank);
                }
                Board.SetPiece(move.StartingSquare, initialPiece, move.Color);

                if (initialPiece == PieceTypes.King)
                {
                    var rank = move.Color == Colors.Black ? 8 : 1;
                    if (move.Flags == Flag.ShortCastle)
                    {
                        // Castling.
                        Board.SetPiece(Files.H, rank, PieceTypes.Rook, move.Color);
                        Board.ClearPiece(Files.F, rank);
                    }

                    if (move.Flags == Flag.LongCastle)
                    {
                        Board.SetPiece(Files.A, rank, PieceTypes.Rook, move.Color);
                        Board.ClearPiece(Files.D, rank);
                    }
                }
            }
            _legalNonQuietMoves = null;
            _legalMoves = null;
            _isWhiteKingInCheck = null;
            _isBlackKingInCheck = null;
            if (Moves.Count >= 2)
            {
                var moveBeforeLast = Moves.ElementAt(Moves.Count - 2);
                var previousStarting = Board.GetSquare(moveBeforeLast.StartingSquare);
                var previousDestination = Board.GetSquare(moveBeforeLast.TargetSquare);
                if (
                    moveBeforeLast.Piece == PieceTypes.Pawn
                    && Math.Abs(previousStarting.Square.Rank - previousDestination.Square.Rank) == 2
                )
                {
                    EnPassantFile = previousStarting.Square.File;
                }
                else
                {
                    EnPassantFile = null;
                }
            }
            else
            {
                EnPassantFile = null;
            }

            Moves.RemoveAt(Moves.Count - 1); // can't just remove "Move" because the move equality kicks in.
            RepetititionTracker.RemoveHash();
            WhiteCanShortCastle = true;
            WhiteCanLongCastle = true;
            BlackCanShortCastle = true;
            BlackCanLongCastle = true;
            foreach (
                var previousMove in Moves.Where(
                    x => x.Piece == PieceTypes.Rook || x.Piece == PieceTypes.King
                )
            )
            {
                var previousStarting = Board.GetSquare(previousMove.StartingSquare);
                if (previousMove.Piece == PieceTypes.King)
                {
                    if (previousMove.Color == Colors.White)
                    {
                        WhiteCanLongCastle = false;
                        WhiteCanShortCastle = false;
                    }
                    else
                    {
                        BlackCanLongCastle = false;
                        BlackCanShortCastle = false;
                    }
                }
                else
                {
                    if (previousMove.Color == Colors.White && previousStarting.Square.Rank == 1)
                    {
                        if (previousStarting.Square.File == Files.A)
                        {
                            WhiteCanLongCastle = false;
                        }
                        else if (previousStarting.Square.File == Files.H)
                        {
                            WhiteCanShortCastle = false;
                        }
                    }
                    else if (
                        previousMove.Color == Colors.Black
                        && previousStarting.Square.Rank == 8
                    )
                    {
                        if (previousStarting.Square.File == Files.A)
                        {
                            BlackCanLongCastle = false;
                        }
                        else if (previousStarting.Square.File == Files.H)
                        {
                            BlackCanShortCastle = false;
                        }
                    }
                }
            }
            return Board;
        }

        private void SetupBoard()
        {
            Board.SetupBoard();
        }
    }
}
