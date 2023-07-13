﻿using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChessLibrary
{
    public class Engine
    {
        public static int nodesEvaluated = 0;
        public static int nonQuietDepthNodesEvaluated = 0;
        public static long miliseconds = 0;
        public static int skips = 0;
        public static int zobristMatches = 0;

        private ConcurrentDictionary<bool, ConcurrentDictionary<int, ConcurrentDictionary<ulong, int?>>> ZobristScore = new ConcurrentDictionary<bool, ConcurrentDictionary<int, ConcurrentDictionary<ulong, int?>>>();

        public int MaxTime { get; set; }
        private int _startingDepth;
        private bool _wasEvaluationCancelled = false;
        public Scorer Scorer { get; set; }
         
        private Stopwatch _stopwatch = new Stopwatch();

        public Engine(ScorerConfiguration scoreConfig)
        {
            Scorer = new Scorer(scoreConfig);
            MaxTime = scoreConfig.MaxTimeMilliseconds;
            _startingDepth = scoreConfig.StartingDepth;
            ZobristScore.TryAdd(true, new ConcurrentDictionary<int, ConcurrentDictionary<ulong, int?>>());
            ZobristScore.TryAdd(false, new ConcurrentDictionary<int, ConcurrentDictionary<ulong, int?>>());
        }

        public (NodeInfo? node, int depth) GetBestMove(Game game, Colors playerColor)
        {
            _stopwatch.Restart();
            nodesEvaluated = 0;
            nonQuietDepthNodesEvaluated = 0;
            zobristMatches = 0;
            var opponentColor = playerColor == Colors.White ? Colors.Black : Colors.White;
            var isCheckmate = game.IsCheckmate;
            if (!isCheckmate)
            {
                NodeInfo? result = null;
                int depthToSearch = _startingDepth - 1;
                bool checkmate = false;
                while (_stopwatch.ElapsedMilliseconds < MaxTime && !checkmate)
                {
                    depthToSearch++;
                    nodesEvaluated = 0;
                    nonQuietDepthNodesEvaluated = 0;
                    zobristMatches = 0;
                    skips = 0;
                    var previousResult = result;
                    result = GetMoveScores(game, playerColor, opponentColor, depthToSearch - 1);
                    checkmate = result.Score == Int32.MaxValue;
                    if (result.Move == null && previousResult.Move != null)
                    {
                        result = previousResult;
                    }
                    else if (_wasEvaluationCancelled)
                    {
                        result = previousResult;
                    }
                    _wasEvaluationCancelled = false;

                }
                return (result, _wasEvaluationCancelled ? depthToSearch - 1 : depthToSearch);
            }


            return (null, 3);
        }

        private int GetLoudMoveScores(Game game, Colors playerColor, Colors opponentColor, NewMove? move, int alpha, int beta)
        {
            var scores = Scorer.GetScore(game.Board, game.IsKingInCheck(Colors.White), game.IsKingInCheck(Colors.Black), game.IsStalemate);
            var playerScore = playerColor == Colors.Black ? scores.blackScore : scores.whiteScore;
            var opponentScore = playerColor == Colors.Black ? scores.whiteScore : scores.blackScore;
            Engine.nodesEvaluated++;
            Engine.nonQuietDepthNodesEvaluated++;
            if (playerScore - opponentScore >= beta)
            {
                return beta;
            }

            if (playerScore - opponentScore > alpha)
            {
                alpha = playerScore - opponentScore;
            }

            var legalNonQuietMoves = game.GetAllLegalMoves(false);
            legalNonQuietMoves = legalNonQuietMoves.OrderMoves(this).ToList();
            foreach (var nqm in legalNonQuietMoves)
            {
                game.AddMove(nqm, false);
                playerScore = GetLoudMoveScores(game, playerColor, opponentColor, nqm, alpha, beta);
                game.UndoLastMove();
                if (playerColor == game.PlayerToMove)
                {
                    if (playerScore >= beta)
                    {
                        return beta;
                    }
                    if (playerScore > alpha)
                    {
                        alpha = playerScore;
                    }
                }
                else
                {
                    beta = Math.Min(beta, playerScore);
                    if (alpha >= beta)
                    {
                        skips++;
                        break;
                    }
                }



            }
            return playerScore;
        }

        private NodeInfo GetMoveScores(Game game, Colors playerColor, Colors opponentColor, int currentDepth)
        {
            int? currentBestScore = null;
            NewMove? currentBestMove = null;
            //Parallel.ForEach(game.GetAllLegalMoves(), m =>
            //{
            //    Game clonedGame = (Game)game.Clone();
            //    clonedGame.AddMove(m, false);
            //    var score = GetRawMoveScores(clonedGame, playerColor, opponentColor, currentDepth, m, Int32.MinValue, Int32.MaxValue);
            //    clonedGame.UndoLastMove();
            //    if (score != null && (currentBestScore == null || score > currentBestScore))
            //    {
            //        currentBestScore = score;
            //        currentBestMove = m;
            //    }
            //});
            foreach(var move in game.GetAllLegalMoves())
            {
                game.AddMove(move, false);
                var score = GetRawMoveScores(game, playerColor, opponentColor, currentDepth, move, Int32.MinValue, Int32.MaxValue);
                game.UndoLastMove();
                if (score != null && (currentBestScore == null || score > currentBestScore))
                {
                    currentBestScore = score;
                    currentBestMove = move;
                }

            }
            return new NodeInfo(currentBestMove, currentBestScore ?? 0, 0, 0);
        }

        private int? GetRawMoveScores(Game game, Colors playerColor, Colors opponentColor, int currentDepth, NewMove? move, int alpha, int beta)
        {

            if (_stopwatch.ElapsedMilliseconds >= MaxTime)
            {
                _wasEvaluationCancelled = true;
                return null;
            }

            var zobristTable = ZobristScore[game.PlayerToMove == playerColor];

            if (!zobristTable.ContainsKey(currentDepth))
            {
                zobristTable.TryAdd(currentDepth, new ConcurrentDictionary<ulong, int?>());
            }

            var hash = game.ZobristTable.CalculateZobristHash(game.Board);
            if (zobristTable[currentDepth].ContainsKey(hash))
            {
                zobristMatches++;
                return zobristTable[currentDepth][hash];
            }

            var isCheckmate = game.IsCheckmate;
            var isStalemate = game.IsStalemate;


            if (currentDepth == 0 || isCheckmate || isStalemate)
            {
                nodesEvaluated++;
                if (isStalemate)
                {
                    zobristTable[currentDepth].TryAdd(hash, 0);
                    return 0;
                }
                if (isCheckmate)
                {
                    if(playerColor == game.PlayerToMove) // move was added by opponent, so technically, player to move is who moves next
                    {
                        zobristTable[currentDepth].TryAdd(hash, -Int32.MaxValue);
                        return -Int32.MaxValue;
                    }
                    zobristTable[currentDepth].TryAdd(hash, Int32.MaxValue);
                    return Int32.MaxValue;
                }

                //var loudScore = GetLoudMoveScores(game, playerColor, opponentColor, move, alpha, beta);

                var scores = Scorer.GetScore(game.Board, game.IsKingInCheck(Colors.White), game.IsKingInCheck(Colors.Black), isStalemate);
                var playerScore = playerColor == Colors.Black ? scores.blackScore : scores.whiteScore;
                var opponentScore = playerColor == Colors.Black ? scores.whiteScore : scores.blackScore;
                //var result = Math.Max(loudScore, playerScore - opponentScore);
                var result = playerScore - opponentScore;
                zobristTable[currentDepth].TryAdd(hash, result);
                return result;
            }
            var moves = game.GetAllLegalMoves().OrderMoves(this).ToList();

            int? bestScoreThisIteration = null;
            foreach (var newMove in moves)
            {
                game.AddMove(newMove, false);
                var scoreForThisMove = GetRawMoveScores(game, playerColor, opponentColor, currentDepth - 1, newMove, alpha, beta);
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
                    bestScoreThisIteration = bestScoreThisIteration >= scoreForThisMove ? bestScoreThisIteration : scoreForThisMove;
                    alpha = Math.Max(alpha, scoreForThisMove.Value);
                    if (alpha >= beta)
                    {
                        skips++;
                        break;
                    }
                }
                else
                {
                    bestScoreThisIteration = bestScoreThisIteration <= scoreForThisMove ? bestScoreThisIteration : scoreForThisMove;
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

            //var isCheckmate = game.IsCheckmate;
            //var isStalemate = game.IsStalemate;

            //if(currentDepth == 0)
            //{
            //    var loudMoveScore = GetLoudMoveScores(game, playerColor, opponentColor, move, alpha, beta); 
            //    //if(loudMoveScore)
            //}

            //if (currentDepth == 0 || isCheckmate ||isStalemate)
            //{
            //    if (game.IsStalemate)
            //    {
            //        return new NodeInfo(move, 0, 0, 0);
            //    }
            //    nodesEvaluated++;
            //    if(move == null)
            //    {
            //        throw new Exception("Error! Move is null");
            //    }
            //    if (isCheckmate)
            //    {
            //        return move.Piece.Color == playerColor ? new NodeInfo(move, 1000000000 + currentDepth, 0, 0) : new NodeInfo(move, -1000000000 + currentDepth, 0, 0);
            //    }

            //    var scores = Scorer.GetScore(game.Board, game.IsKingInCheck(Colors.White), game.IsKingInCheck(Colors.Black), game.IsStalemate);
            //    var playerScore = playerColor == Colors.Black ? scores.blackScore : scores.whiteScore;
            //    var opponentScore = playerColor == Colors.Black ? scores.whiteScore : scores.blackScore;
            //    return new NodeInfo(move, playerScore - opponentScore, 0, 0);
            //}

            //var legalMoves = game.GetAllLegalMoves();
            //legalMoves = legalMoves.OrderMoves(this).ToList();
            //if (game.PlayerToMove == playerColor)
            //{
            //    var value = new NodeInfo(move, int.MinValue, alpha, beta);
            //    foreach(var m in legalMoves)
            //    {
            //        if(_stopwatch.ElapsedMilliseconds >= _maxTime)
            //        {
            //            return value;
            //        }
            //        game.AddMove(m, false);
            //        var node = GetMoveScores(game, playerColor, opponentColor, currentDepth - 1, m, alpha, beta);
            //        game.UndoLastMove();
            //        value = value.Score >= node.Score ? value : new NodeInfo(m, node.Score, node.Alpha, node.Beta);
            //        alpha = Math.Max(alpha, value.Score);                    
            //        if (alpha >= beta)
            //        {
            //            skips++;
            //            break;
            //        }
            //    }
            //    return value;
            //}
            //else
            //{
            //    var value = new NodeInfo(move, int.MaxValue, alpha, beta);
            //    foreach (var m in legalMoves)
            //    {
            //        if (_stopwatch.ElapsedMilliseconds >= _maxTime)
            //        {
            //            return value;
            //        }
            //        game.AddMove(m, false);
            //        var node = GetMoveScores(game, playerColor, opponentColor, currentDepth - 1, m, alpha, beta);
            //        game.UndoLastMove();
            //        value = value.Score <= node.Score ? value : new NodeInfo(m, node.Score, node.Alpha, node.Beta);
            //        beta = Math.Min(beta, value.Score);
            //        if (alpha >= beta)
            //        {
            //            skips++;
            //            break;
            //        }
            //    }
            //    return value;
            //}
        }

        public class NodeInfo
        {
            public NewMove? Move { get; set; }
            public int Score { get; set; }
            public int Alpha { get; set; }
            public int Beta { get; set; }

            public NodeInfo(NewMove? move, int score, int alpha, int beta)
            {
                Move = move;
                Score = score;
                Alpha = alpha;
                Beta = beta;
            }
        }

    }
}
