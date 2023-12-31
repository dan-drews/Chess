using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ChessLibrary.Evaluation;
using ChessLibrary.OpeningBook;
using MoreLinq;

namespace ChessLibrary
{
    public class Engine
    {
        public static int nodesEvaluated = 0;
        public static int nonQuietDepthNodesEvaluated = 0;
        public static long miliseconds = 0;
        public static int skips = 0;
        public static int zobristMatches = 0;

        private ConcurrentDictionary<
            bool,
            ConcurrentDictionary<int, ConcurrentDictionary<ulong, int?>>
        > ZobristScore =
            new ConcurrentDictionary<
                bool,
                ConcurrentDictionary<int, ConcurrentDictionary<ulong, int?>>
            >();

        private int _maxTime => Config.MaxTimeMilliseconds;
        private int _startingDepth => Config.StartingDepth;
        private int? _maxDepth => Config.MaxDepth;
        private bool _wasEvaluationCancelled = false;
        public bool UseNullMovePruning { get; set; }
        public IEvaluator Scorer { get; set; }

        private Stopwatch _stopwatch = new Stopwatch();
        public ScorerConfiguration Config { get; set; }

        public Engine(ScorerConfiguration scoreConfig)
        {
            Config = scoreConfig;
            Scorer = new ComplexEvaluator(scoreConfig);
            ZobristScore.TryAdd(
                true,
                new ConcurrentDictionary<int, ConcurrentDictionary<ulong, int?>>()
            );
            ZobristScore.TryAdd(
                false,
                new ConcurrentDictionary<int, ConcurrentDictionary<ulong, int?>>()
            );
        }

        public (NodeInfo? node, int depth) GetBestMove(Game game, Colors playerColor)
        {
            ZobristScore[true].Clear();
            ZobristScore[false].Clear();
            _stopwatch.Restart();
            nodesEvaluated = 0;
            nonQuietDepthNodesEvaluated = 0;
            zobristMatches = 0;
            var opponentColor = playerColor == Colors.White ? Colors.Black : Colors.White;
            var isCheckmate = game.IsCheckmate;
            if (!isCheckmate)
            {
                if (Config.UseOpeningBook && game.Moves.Count < 8)
                {
                    var hash = ZobristTable.CalculateZobristHash(game.Board);
                    var pickedMove = OpeningBookMovePicker.GetMoveForZobrist(hash);
                    if (pickedMove != null)
                    {
                        System.Threading.Thread.Sleep(1000);
                        return (new NodeInfo(pickedMove.Value, 0, 0, 0), 1);
                    }
                }

                NodeInfo? result = null;
                int depthToSearch = _startingDepth - 1;
                bool checkmate = false;
                while (
                    _stopwatch.ElapsedMilliseconds < _maxTime
                    && !checkmate
                    && depthToSearch < (_maxDepth ?? int.MaxValue)
                )
                {
                    _wasEvaluationCancelled = false;
                    depthToSearch++;
                    //nodesEvaluated = 0;
                    nonQuietDepthNodesEvaluated = 0;
                    //zobristMatches = 0;
                    skips = 0;
                    var previousResult = result;
                    if (game.GetAllLegalMoves().Length == 1)
                    {
                        return (new NodeInfo(game.GetAllLegalMoves().First(), 0, 0, 0), 1);
                    }
                    result = GetMoveScores(
                        game,
                        playerColor,
                        opponentColor,
                        depthToSearch - 1,
                        previousResult?.Move
                    );
                    checkmate = result.Score == Int32.MaxValue;
                    if (checkmate)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                    if (result.Move == null && previousResult?.Move != null)
                    {
                        result = previousResult;
                    }
                    else if (_wasEvaluationCancelled)
                    {
                        //Console.Write("Cancelled");
                        //result = previousResult;
                    }
                }
                return (result, _wasEvaluationCancelled ? depthToSearch - 1 : depthToSearch);
            }

            return (null, 3);
        }

        private NodeInfo GetMoveScores(
            Game game,
            Colors playerColor,
            Colors opponentColor,
            int currentDepth,
            Move? previousBest
        )
        {
            int? currentBestScore = null;
            Move? currentBestMove = null;
            object l = new object();
            IEnumerable<Move> moves = game.GetAllLegalMoves();
            moves = moves.OrderMoves(this, previousBest);
            if (previousBest != null)
            {
                game.AddMove(previousBest.Value, false);
                var score = GetRawMoveScores(
                    game,
                    playerColor,
                    opponentColor,
                    currentDepth,
                    Int32.MinValue,
                    Int32.MaxValue
                );
                game.UndoLastMove();
                moves = moves.Except(new Move[] { previousBest.Value });
                if (score != null)
                {
                    currentBestScore = score;
                    currentBestMove = previousBest.Value;
                }
            }
            Parallel.ForEach(
                moves,
                (move) =>
                {
                    Game tempGame = (Game)game.Clone();
                    tempGame.AddMove(move, false);
                    var score = GetRawMoveScores(
                        tempGame,
                        playerColor,
                        opponentColor,
                        currentDepth,
                        Int32.MinValue,
                        Int32.MaxValue
                    );
                    tempGame.UndoLastMove();
                    lock (l)
                    {
                        if (score != null && (currentBestScore == null || score > currentBestScore))
                        {
                            currentBestScore = score;
                            currentBestMove = move;
                        }
                    }
                }
            );
            return new NodeInfo(currentBestMove, currentBestScore ?? 0, 0, 0);
        }

        private int? GetRawMoveScores(
            Game game,
            Colors playerColor,
            Colors opponentColor,
            int currentDepth,
            int alpha,
            int beta
        )
        {
            if (_stopwatch.ElapsedMilliseconds >= _maxTime)
            {
                _wasEvaluationCancelled = true;
                return null;
            }

            var zobristTable = ZobristScore[game.PlayerToMove == playerColor];

            if (!zobristTable.ContainsKey(currentDepth))
            {
                zobristTable.TryAdd(currentDepth, new ConcurrentDictionary<ulong, int?>());
            }
            ulong hash = ZobristTable.CalculateZobristHash(game.Board);
            if (zobristTable[currentDepth].ContainsKey(hash))
            {
                zobristMatches++;
                return zobristTable[currentDepth][hash];
            }

            var isCheckmate = game.IsCheckmate;
            var isStalemate = game.IsStalemate;
            var isRepetition = game.RepetititionTracker.IsRepetition(hash, true);

            if (currentDepth == 0 || isCheckmate || isStalemate || isRepetition)
            {
                if (game.Moves.Last() != Move.NullMove)
                {
                    nodesEvaluated++;
                }
                if (isRepetition)
                {
                    zobristTable[currentDepth].TryAdd(hash, 0);
                    return 0;
                }
                if (isStalemate)
                {
                    zobristTable[currentDepth].TryAdd(hash, 0);
                    return 0;
                }
                if (isCheckmate)
                {
                    if (playerColor == game.PlayerToMove) // move was added by opponent, so technically, player to move is who moves next
                    {
                        zobristTable[currentDepth].TryAdd(hash, -Int32.MaxValue);
                        return -Int32.MaxValue;
                    }
                    zobristTable[currentDepth].TryAdd(hash, Int32.MaxValue);
                    return Int32.MaxValue;
                }

                if (game.IsKingInCheck(game.PlayerToMove))
                {
                    // Extend search when in check
                    currentDepth++;
                    if (!zobristTable.ContainsKey(currentDepth))
                    {
                        zobristTable.TryAdd(currentDepth, new ConcurrentDictionary<ulong, int?>());
                    }
                }
                else
                {
                    //var loudScore = GetLoudMoveScores(game, playerColor, opponentColor, move, alpha, beta);

                    var scores = Scorer.GetScore(
                        game.Board,
                        game.IsKingInCheck(Colors.White),
                        game.IsKingInCheck(Colors.Black),
                        isStalemate,
                        game.Moves.Count
                    );
                    var playerScore =
                        playerColor == Colors.Black ? scores.blackScore : scores.whiteScore;
                    var opponentScore =
                        playerColor == Colors.Black ? scores.whiteScore : scores.blackScore;
                    //var result = Math.Max(loudScore, playerScore - opponentScore);
                    var result = playerScore - opponentScore;
                    zobristTable[currentDepth].TryAdd(hash, result);
                    return result;
                }
            }

            var moves = game.GetAllLegalMoves().OrderMoves(this, null).ToList();
            if (UseNullMovePruning && currentDepth >= 3 && !game.IsKingInCheck(playerColor))
            {
                moves.Insert(0, Move.NullMove);
            }
            int? bestScoreThisIteration = null;
            foreach (var move in moves)
            {
                game.AddMove(move, false);
                var newDepth = currentDepth - 1;
                if (move == Move.NullMove)
                {
                    newDepth = currentDepth - 1 - 2;
                }
                var scoreForThisMove = GetRawMoveScores(
                    game,
                    playerColor,
                    opponentColor,
                    newDepth,
                    alpha,
                    beta
                );
                game.UndoLastMove();
                if (scoreForThisMove == null)
                {
                    zobristTable[currentDepth].TryAdd(hash, null);
                    return null;
                }
                if (bestScoreThisIteration == null)
                {
                    bestScoreThisIteration = scoreForThisMove;
                }
                if (playerColor == game.PlayerToMove) //we undid the move, so its reporting the player color inaccurately
                {
                    bestScoreThisIteration =
                        bestScoreThisIteration >= scoreForThisMove
                            ? bestScoreThisIteration
                            : scoreForThisMove;
                    alpha = Math.Max(alpha, scoreForThisMove.Value);
                    if (alpha >= beta)
                    {
                        skips++;
                        break;
                    }
                }
                else
                {
                    bestScoreThisIteration =
                        bestScoreThisIteration <= scoreForThisMove
                            ? bestScoreThisIteration
                            : scoreForThisMove;
                    beta = Math.Min(beta, scoreForThisMove.Value);
                    if (alpha >= beta)
                    {
                        skips++;
                        break;
                    }
                }
            }
            zobristTable[currentDepth].TryAdd(hash, bestScoreThisIteration);
            return bestScoreThisIteration;
        }

        public class NodeInfo
        {
            public Move? Move { get; set; }
            public int Score { get; set; }
            public int Alpha { get; set; }
            public int Beta { get; set; }

            public NodeInfo(Move? move, int score, int alpha, int beta)
            {
                Move = move;
                Score = score;
                Alpha = alpha;
                Beta = beta;
            }
        }
    }
}
