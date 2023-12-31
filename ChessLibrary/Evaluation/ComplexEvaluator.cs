using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary.Evaluation;

public class ComplexEvaluator : IEvaluator
{
    public ScorerConfiguration Config { get; set; }

    public ComplexEvaluator(ScorerConfiguration config)
    {
        Config = config;
    }

    public (int blackScore, int whiteScore) GetScore(
        BitBoard board,
        bool isWhiteKingInCheck,
        bool isBlackKingInCheck,
        bool isStalemate,
        int totalMoveCount
    )
    {
        return (
            GetScoreInternal(
                board,
                isWhiteKingInCheck,
                isBlackKingInCheck,
                Colors.Black,
                isStalemate,
                totalMoveCount
            ),
            GetScoreInternal(
                board,
                isWhiteKingInCheck,
                isBlackKingInCheck,
                Colors.White,
                isStalemate,
                totalMoveCount
            )
        );
    }

    public int GetPieceValue(PieceTypes piece)
    {
        return piece switch
        {
            PieceTypes.Pawn => Config.PawnValue,
            PieceTypes.Bishop => Config.BishopValue,
            PieceTypes.Rook => Config.RookValue,
            PieceTypes.Knight => Config.KnightValue,
            PieceTypes.Queen => Config.QueenValue,
            PieceTypes.King => Config.KingValue,
            _ => 0
        };
    }

    private int GetScoreInternal(
        BitBoard board,
        bool isWhiteKingInCheck,
        bool isBlackKingInCheck,
        Colors color,
        bool isStalemate,
        int totalMoveCount
    )
    {
        if (isStalemate)
        {
            return Config.StalemateScore;
        }
        int score = 0;
        for (Files f = Files.A; f <= Files.H; f++)
        {
            for (int rank = 1; rank <= 8; rank++)
            {
                var square = board.GetSquare(f, rank);
                if (square.Piece?.Type != null && square.Piece.Color == color)
                {
                    score += GetPieceValue(square.Piece.Type);
                    switch (square.Piece.Type)
                    {
                        case PieceTypes.Knight:
                            score += _knightSquareTable[
                                GetTableLocationForPosition(
                                    square.Square.SquareNumber,
                                    square.Piece.Color
                                )
                            ];
                            break;
                        case PieceTypes.King:
                            score += _kingSquareTable[
                                GetTableLocationForPosition(
                                    square.Square.SquareNumber,
                                    square.Piece.Color
                                )
                            ];
                            break;
                        case PieceTypes.Bishop:
                            score += _bishopSquareTable[
                                GetTableLocationForPosition(
                                    square.Square.SquareNumber,
                                    square.Piece.Color
                                )
                            ];
                            break;
                        case PieceTypes.Queen:
                            if (totalMoveCount < 12)
                            {
                                score += _queenEarlyGameSquareTable[
                                    GetTableLocationForPosition(
                                        square.Square.SquareNumber,
                                        square.Piece.Color
                                    )
                                ];
                            }
                            else
                            {
                                score += _queenSquareTable[
                                    GetTableLocationForPosition(
                                        square.Square.SquareNumber,
                                        square.Piece.Color
                                    )
                                ];
                            }
                            break;
                        case PieceTypes.Rook:
                            score += _rookSquareTable[
                                GetTableLocationForPosition(
                                    square.Square.SquareNumber,
                                    square.Piece.Color
                                )
                            ];
                            break;
                        case PieceTypes.Pawn:
                            score += _pawnSquareTable[
                                GetTableLocationForPosition(
                                    square.Square.SquareNumber,
                                    square.Piece.Color
                                )
                            ];
                            break;
                    }
                }
            }
        }
        bool isOpponentInCheck = color == Colors.White ? isBlackKingInCheck : isWhiteKingInCheck;
        bool isSelfInCheck = color == Colors.White ? isWhiteKingInCheck : isBlackKingInCheck;

        var stackedPawns = MultipledPawnsInFiles(board, color);
        score += stackedPawns * Config.StackedPawnPenalty; // the penalty is negative, so add it

        if (isOpponentInCheck)
        {
            score += Config.OpponentInCheckScore;
        }
        if (isSelfInCheck)
        {
            score += Config.SelfInCheckScore;
        }
        return score;
    }

    private static int MultipledPawnsInFiles(BitBoard board, Colors color)
    {
        var result = 0;
        for (Files f = Files.A; f <= Files.H; f++)
        {
            var pawns = board.PawnsInFile(f, color);
            if (pawns > 1)
            {
                result += pawns;
            }
        }
        return result;
    }

    private static int GetTableLocationForPosition(int position, Colors color)
    {
        switch (color)
        {
            case Colors.White:
                return 63 - position;
            case Colors.Black:
                return _blackPieceMapping[position];
        }
        throw new NotImplementedException();
    }

    private static int[] _blackPieceMapping =
    {
        07,
        06,
        05,
        04,
        03,
        02,
        01,
        00,
        15,
        14,
        13,
        12,
        11,
        10,
        09,
        08,
        23,
        22,
        21,
        20,
        19,
        18,
        17,
        16,
        31,
        30,
        29,
        28,
        27,
        26,
        25,
        24,
        39,
        38,
        37,
        36,
        35,
        34,
        33,
        32,
        47,
        46,
        45,
        44,
        43,
        42,
        41,
        40,
        55,
        54,
        53,
        52,
        51,
        50,
        49,
        48,
        63,
        62,
        61,
        60,
        59,
        58,
        57,
        56,
    };

    private static int[] _knightSquareTable =
    {
        -50,
        -40,
        -30,
        -30,
        -30,
        -30,
        -40,
        -50,
        -40,
        -20,
        0,
        0,
        0,
        0,
        -20,
        -40,
        -30,
        0,
        10,
        15,
        15,
        10,
        0,
        -30,
        -30,
        5,
        15,
        20,
        20,
        15,
        5,
        -30,
        -30,
        0,
        15,
        20,
        20,
        15,
        0,
        -30,
        -30,
        5,
        10,
        15,
        15,
        10,
        5,
        -30,
        -40,
        -20,
        0,
        5,
        5,
        0,
        -20,
        -40,
        -50,
        -40,
        -5,
        -30,
        -30,
        -30,
        -5,
        -50
    };

    private static int[] _bishopSquareTable =
    {
        -20,
        -10,
        -10,
        -10,
        -10,
        -10,
        -10,
        -20,
        -10,
        0,
        0,
        0,
        0,
        0,
        0,
        -10,
        -10,
        0,
        5,
        10,
        10,
        5,
        0,
        -10,
        -10,
        5,
        5,
        10,
        10,
        5,
        5,
        -10,
        -10,
        0,
        10,
        10,
        10,
        10,
        0,
        -10,
        -10,
        10,
        10,
        10,
        10,
        10,
        10,
        -10,
        -10,
        5,
        0,
        0,
        0,
        0,
        5,
        -10,
        -20,
        -10,
        -10,
        -10,
        -10,
        -10,
        -10,
        -20,
    };

    private static int[] _queenSquareTable =
    {
        -20,
        -10,
        -10,
        -5,
        -5,
        -10,
        -10,
        -20,
        -10,
        0,
        0,
        0,
        0,
        0,
        0,
        -10,
        -10,
        0,
        5,
        5,
        5,
        5,
        0,
        -10,
        -5,
        0,
        5,
        5,
        5,
        5,
        0,
        -5,
        0,
        0,
        5,
        5,
        5,
        5,
        0,
        -5,
        -10,
        5,
        5,
        5,
        5,
        5,
        0,
        -10,
        -10,
        0,
        5,
        0,
        0,
        0,
        0,
        -10,
        -20,
        -10,
        -10,
        -5,
        -5,
        -10,
        -10,
        -20
    };

    private static int[] _queenEarlyGameSquareTable =
    {
        -20,
        -10,
        -10,
        -5,
        -5,
        -10,
        -10,
        -20,
        -10,
        0,
        0,
        0,
        0,
        0,
        0,
        -10,
        -10,
        0,
        5,
        5,
        5,
        5,
        0,
        -10,
        -5,
        0,
        5,
        5,
        5,
        5,
        0,
        -5,
        0,
        0,
        5,
        5,
        5,
        5,
        0,
        -5,
        -10,
        5,
        5,
        5,
        5,
        5,
        0,
        -10,
        -10,
        0,
        5,
        0,
        0,
        0,
        0,
        -10,
        -20,
        -10,
        -10,
        100,
        -5,
        -10,
        -10,
        -20
    };

    private static int[] _kingSquareTable =
    {
        -30,
        -40,
        -40,
        -50,
        -50,
        -40,
        -40,
        -30,
        -30,
        -40,
        -40,
        -50,
        -50,
        -40,
        -40,
        -30,
        -30,
        -40,
        -40,
        -50,
        -50,
        -40,
        -40,
        -30,
        -30,
        -40,
        -40,
        -50,
        -50,
        -40,
        -40,
        -30,
        -20,
        -30,
        -30,
        -40,
        -40,
        -30,
        -30,
        -20,
        -10,
        -20,
        -20,
        -20,
        -20,
        -20,
        -20,
        -10,
        20,
        20,
        0,
        0,
        0,
        0,
        20,
        20,
        20,
        30,
        40,
        0,
        0,
        10,
        40,
        20
    };

    private static int[] _pawnSquareTable =
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        50,
        50,
        50,
        50,
        50,
        50,
        50,
        50,
        10,
        10,
        20,
        30,
        30,
        20,
        10,
        10,
        5,
        5,
        10,
        25,
        25,
        10,
        5,
        5,
        0,
        0,
        0,
        20,
        20,
        0,
        0,
        0,
        5,
        -5,
        -10,
        0,
        0,
        -10,
        -5,
        5,
        5,
        10,
        10,
        -20,
        -20,
        10,
        10,
        5,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0
    };

    private static int[] _rookSquareTable =
    {
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        0,
        5,
        10,
        10,
        10,
        10,
        10,
        10,
        5,
        -5,
        0,
        0,
        0,
        0,
        0,
        0,
        -5,
        -5,
        0,
        0,
        0,
        0,
        0,
        0,
        -5,
        -5,
        0,
        0,
        0,
        0,
        0,
        0,
        -5,
        -5,
        0,
        0,
        0,
        0,
        0,
        0,
        -5,
        -5,
        0,
        0,
        0,
        0,
        0,
        0,
        -5,
        0,
        0,
        0,
        10,
        5,
        10,
        0,
        0
    };
}
