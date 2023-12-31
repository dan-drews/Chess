namespace ChessLibrary
{
    public class Piece
    {
        public static Piece[][] Pieces;

        static Piece()
        {
            Pieces = new Piece[][]
            {
                new Piece[]
                {
                    new Piece() { Color = Colors.White, Type = PieceTypes.Pawn },
                    new Piece() { Color = Colors.White, Type = PieceTypes.Rook },
                    new Piece() { Color = Colors.White, Type = PieceTypes.Knight },
                    new Piece() { Color = Colors.White, Type = PieceTypes.Bishop },
                    new Piece() { Color = Colors.White, Type = PieceTypes.Queen },
                    new Piece() { Color = Colors.White, Type = PieceTypes.King }
                },
                new Piece[]
                {
                    new Piece() { Color = Colors.Black, Type = PieceTypes.Pawn },
                    new Piece() { Color = Colors.Black, Type = PieceTypes.Rook },
                    new Piece() { Color = Colors.Black, Type = PieceTypes.Knight },
                    new Piece() { Color = Colors.Black, Type = PieceTypes.Bishop },
                    new Piece() { Color = Colors.Black, Type = PieceTypes.Queen },
                    new Piece() { Color = Colors.Black, Type = PieceTypes.King }
                }
            };
        }

        private Piece() { }

        public PieceTypes Type { get; set; }
        public Colors Color { get; set; }

        private int? _score = null;

        public int Score
        {
            get
            {
                if (_score == null || _score == 0)
                {
                    switch (Type)
                    {
                        case PieceTypes.Pawn:
                            _score = 2;
                            break;
                        case PieceTypes.Knight:
                        case PieceTypes.Bishop:
                            _score = 8;
                            break;
                        case PieceTypes.Rook:
                            _score = 13;
                            break;
                        case PieceTypes.Queen:
                            _score = 21;
                            break;
                        case PieceTypes.King:
                            _score = 100;
                            break;
                        default:
                            _score = 0;
                            break;
                    }
                }
                return _score.Value;
            }
        }
    }
}
