//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ChessLibrary
//{
//    public class NaiveBoard : ICloneable, IBoard
//    {
//        private SquareState[][] _squares;

//        public NaiveBoard(bool setup = true)
//        {
//            _squares = new SquareState[8][];

//            if (setup)
//            {
//                for (Files i = Files.A; i <= Files.H; i++)
//                {
//                    _squares[(int)i - 1] = new SquareState[8];
//                    // Counting from 1 to 8 rather than 0 to 7 here to match the board labels
//                    for (int j = 1; j <= 8; j++)
//                    {
//                        _squares[(int)i - 1][j - 1] = new SquareState(new Square()
//                        {
//                            File = i,
//                            Rank = j
//                        });
//                    }
//                }
//            }
//        }

//        public SquareState GetSquare(Files file, int rank)
//        {
//            return _squares[(int)file - 1][rank - 1];
//        }

//        public void SetupBoard()
//        {
//            // White Pawns
//            for (Files f = Files.A; f <= Files.H; f++)
//            {
//                GetSquare(f, 2).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Pawn };
//            }

//            // Black Pawns
//            for (Files f = Files.A; f <= Files.H; f++)
//            {
//                GetSquare(f, 7).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Pawn };
//            }

//            GetSquare(Files.A, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Rook };
//            GetSquare(Files.H, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Rook };

//            GetSquare(Files.B, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Knight };
//            GetSquare(Files.G, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Knight };

//            GetSquare(Files.C, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Bishop };
//            GetSquare(Files.F, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Bishop };

//            GetSquare(Files.E, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.King };
//            GetSquare(Files.D, 1).Piece = new Piece() { Color = Colors.White, Type = PieceTypes.Queen };

//            GetSquare(Files.A, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Rook };
//            GetSquare(Files.H, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Rook };

//            GetSquare(Files.B, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Knight };
//            GetSquare(Files.G, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Knight };

//            GetSquare(Files.C, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Bishop };
//            GetSquare(Files.F, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Bishop };

//            GetSquare(Files.E, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.King };
//            GetSquare(Files.D, 8).Piece = new Piece() { Color = Colors.Black, Type = PieceTypes.Queen };
//        }

//        public object Clone()
//        {
//            var newBoard = new NaiveBoard(true);
//            for (Files file = Files.A; file <= Files.H; file++)
//            {
//                // Counting from 1 to 8 rather than 0 to 7 here to match the board labels
//                for (int rank = 1; rank <= 8; rank++)
//                {
//                    newBoard._squares[(int)file - 1][rank - 1].Piece = GetSquare(file, rank).Piece;
//                }
//            }
//            return newBoard;
//        }

//        public void MovePiece(Move move)
//        {
//            var initialPiece = GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece;
//            GetSquare(move.StartingSquare.File, move.StartingSquare.Rank).Piece = null;

//            if (initialPiece != null && initialPiece.Type == PieceTypes.Pawn)
//            {
//                var startingRank = initialPiece.Color == Colors.Black ? 4 : 5;
//                if (move.StartingSquare.Rank == startingRank && move.DestinationSquare.File != move.StartingSquare.File)
//                {
//                    // Pawn capture... but is it en passant?
//                    var destination = GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank);
//                    if (destination.Piece == null)
//                    {
//                        // Yup. It's an en passant.
//                        destination.Piece = initialPiece;
//                        var capturedPawnSquare = GetSquare(move.DestinationSquare.File, move.StartingSquare.Rank);
//                        capturedPawnSquare.Piece = null;
//                        return;
//                    }
//                }
//            }

//            if (move.PromotedPiece == null)
//            {
//                GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = initialPiece;
//            }
//            else
//            {
//                GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece = move.PromotedPiece;
//            }

//            if (initialPiece != null && initialPiece.Type == PieceTypes.King)
//            {
//                var rank = initialPiece.Color == Colors.Black ? 8 : 1;
//                if (move.StartingSquare.Rank == rank && move.StartingSquare.File == Files.E && (move.DestinationSquare.File == Files.G || move.DestinationSquare.File == Files.C))
//                {
//                    // Castling.
//                    if (move.DestinationSquare.File == Files.G)
//                    {
//                        var rookCurrentSquare = GetSquare(Files.H, rank);
//                        var targetRookSquare = GetSquare(Files.F, rank);
//                        targetRookSquare.Piece = rookCurrentSquare.Piece;
//                        rookCurrentSquare.Piece = null;
//                    }

//                    if (move.DestinationSquare.File == Files.C)
//                    {
//                        var rookCurrentSquare = GetSquare(Files.A, rank);
//                        var targetRookSquare = GetSquare(Files.D, rank);
//                        targetRookSquare.Piece = rookCurrentSquare.Piece;
//                        rookCurrentSquare.Piece = null;
//                    }
//                }
//            }
//        }

//        public void SetPiece(Files f, int rank, PieceTypes type, Colors color)
//        {
//            GetSquare(f, rank).Piece = new Piece() { Type = type, Color = color };
//        }

//        public void ClearPiece(Files f, int rank)
//        {
//            GetSquare(f, rank).Piece = null;
//        }
//    }
//}
