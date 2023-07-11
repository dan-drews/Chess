using ChessLibrary.Enums;
using ChessLibrary.MoveLegaility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChessLibrary
{
    public class Game : ICloneable
    {
        internal ZobristTable ZobristTable { get; set; } = new ZobristTable();
        public IMoveLegality Evaluator { get; private set; }
        public IBoard Board { get; private set; }

        public bool WhiteCanLongCastle { get; set; } = true;
        public bool BlackCanLongCastle { get; set; } = true;
        public bool WhiteCanShortCastle { get; set; } = true;
        public bool BlackCanShortCastle { get; set; } = true;
        public MagicBitboards MagicBitboards { get; set; }

        private Files? _enPassantFile = null;
        public Files? EnPassantFile
        {
            get
            {
                return _enPassantFile;
            }
            set
            {
                _enPassantFile = value;
            }
        }

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
                    Evaluator = new BitBoardLegality();
                    MagicBitboards = new MagicBitboards();
                    MagicBitboards.Initialize();
                    Board = new BitBoard(MagicBitboards);
                    break;
                //case BoardType.Naive:
                //    Board = new NaiveBoard();
                //    Evaluator = new NaiveMoveLegality();
                //    break;
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
                EnPassantFile = EnPassantFile,
                MagicBitboards = MagicBitboards
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
                GetAllLegalMoves();
                var hashLookup = Moves.ToLookup(x => x.Hash);
                if (hashLookup.Any(x => x.Count() >= 3))
                {
                    return true;
                }
                if (Moves.Count > 50)
                {
                    bool isFiftyMoveRule = true;
                    for (int i = 1; i <= 50; i++)
                    {
                        var move = Moves.ElementAt(Moves.Count - i);
                        if (move.Piece.Type == PieceTypes.Pawn || move.CapturedPiece != null)
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
                return !GetAllLegalMoves().Any() ;
            }
        }

        private List<Move>? _legalNonQuietMoves = null;
        public List<Move> GetAllLegalMoves(bool includeQuietMoves = true)
        {
            if (includeQuietMoves)
            {
                if (_legalMoves == null)
                {
                    _legalMoves = Evaluator.GetAllLegalMoves(Board, PlayerToMove, EnPassantFile, BlackCanLongCastle, BlackCanShortCastle, WhiteCanLongCastle, WhiteCanShortCastle, true);
                }
                return _legalMoves;
            }

            if(_legalNonQuietMoves == null)
            {
                _legalNonQuietMoves = Evaluator.GetAllLegalMoves(Board, PlayerToMove, EnPassantFile, BlackCanLongCastle, BlackCanShortCastle, WhiteCanLongCastle, WhiteCanShortCastle, false);
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
        }

        public IBoard AddMove(Move move, bool validate = true)
        {
            var startingSquare = Board.GetSquare(move.StartingSquare.File, move.StartingSquare.Rank);
            if (startingSquare.Piece == null)
            {
                Debugger.Break();
                throw new Exception("No piece to move");
            }
            if (startingSquare.Piece.Color != PlayerToMove || startingSquare.Piece.Color != move.Player)
            {
                Debugger.Break();
                throw new Exception("Wrong Color Moving");
            }
            List<Move>? legalMoves = null;
            if (validate)
            {
                legalMoves = Evaluator.GetAllLegalMoves(Board, startingSquare, EnPassantFile, BlackCanLongCastle, BlackCanShortCastle, WhiteCanLongCastle, WhiteCanShortCastle);
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
                if (move.Piece.Type == PieceTypes.Pawn && Math.Abs(move.StartingSquare.Rank - move.DestinationSquare.Rank) == 2)
                {
                    EnPassantFile = move.StartingSquare.File;
                }
                else
                {
                    EnPassantFile = null;
                }

                if (move.Piece.Type == PieceTypes.King)
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

                if (move.Piece.Type == PieceTypes.Rook)
                {
                    if (PlayerToMove == Colors.White)
                    {
                        if (move.StartingSquare.Rank == 1)
                        {
                            if (move.StartingSquare.File == Files.A)
                            {
                                WhiteCanLongCastle = false;
                            }
                            if (move.StartingSquare.File == Files.H)
                            {
                                WhiteCanShortCastle = false;
                            }
                        }
                    }
                    else
                    {
                        if (move.StartingSquare.Rank == 8)
                        {
                            if (move.StartingSquare.File == Files.A)
                            {
                                BlackCanLongCastle = false;
                            }
                            if (move.StartingSquare.File == Files.H)
                            {
                                BlackCanShortCastle = false;
                            }
                        }
                    }
                }

                var hash = ZobristTable.CalculateZobristHash(Board);
                if (validate)
                {
                    var m = legalMoves![legalMoves.IndexOf(move)];
                    m.Hash = hash;
                    Moves.Add(m);
                }
                else
                {
                    move.Hash = hash;
                    Moves.Add(move);
                }
                Board.MovePiece(move);                
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
                    if (move.DestinationSquare.Rank == startingRank + moveDirection && Math.Abs(move.DestinationSquare.File - move.StartingSquare.File) == 1)
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
            _legalNonQuietMoves = null;
            _legalMoves = null;
            _isWhiteKingInCheck = null;
            _isBlackKingInCheck = null;
            if (Moves.Count >= 2)
            {
                var moveBeforeLast = Moves.ElementAt(Moves.Count - 2);
                if (moveBeforeLast.Piece.Type == PieceTypes.Pawn &&
                    Math.Abs(moveBeforeLast.StartingSquare.Rank - moveBeforeLast.DestinationSquare.Rank) == 2)
                {
                    EnPassantFile = moveBeforeLast.StartingSquare.File;
                }
                else
                {
                    EnPassantFile = null;
                }
            }

            WhiteCanShortCastle = true;
            WhiteCanLongCastle = true;
            BlackCanShortCastle = true;
            BlackCanLongCastle = true;
            foreach (var previousMove in Moves.Where(x => x.Piece.Type == PieceTypes.Rook || x.Piece.Type == PieceTypes.King))
            {
                if (previousMove.Piece.Type == PieceTypes.King)
                {
                    if (previousMove.Piece.Color == Colors.White)
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
                    if (previousMove.Piece.Color == Colors.White && previousMove.StartingSquare.Rank == 1)
                    {
                        if (previousMove.StartingSquare.File == Files.A)
                        {
                            WhiteCanLongCastle = false;
                        }
                        else if (previousMove.StartingSquare.File == Files.H)
                        {
                            WhiteCanShortCastle = false;
                        }
                    }
                    else if (previousMove.Piece.Color == Colors.Black && previousMove.StartingSquare.Rank == 8)
                    {
                        if (previousMove.StartingSquare.File == Files.A)
                        {
                            BlackCanLongCastle = false;
                        }
                        else if (previousMove.StartingSquare.File == Files.H)
                        {
                            BlackCanShortCastle = false;
                        }
                    }
                }
            }

            Moves.RemoveAt(Moves.Count - 1); // can't just remove "Move" because the move equality kicks in.
            return Board;
        }

        private void SetupBoard()
        {
            Board.SetupBoard();
        }
    }
}
