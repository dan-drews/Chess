using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public class Engine
    {
        const decimal MAX_DEPTH = 1.5M;
        public static Move? GetBestMove(Game game, Colors playerColor, int currentDepth = 1)
        {
            var opponentColor = playerColor == Colors.White ? Colors.Black : Colors.White;
            if (currentDepth == 1)
            {
                var myCurrentScore = game.GetScore(playerColor);
                var opponentCurrentScore = game.GetScore(opponentColor);
            }
            var isCheckmate = game.IsCheckmate;
            var gameScore = new GameScore()
            {
                MyScore = game.GetScore(playerColor),
                OpponentScore = game.GetScore(opponentColor),
                OpponentWinsWithCheckmate = isCheckmate && game.PlayerToMove == playerColor,
                IWinWithCheckmate = isCheckmate && game.PlayerToMove == opponentColor
            };
            if (!isCheckmate)
            {
                gameScore.ChildrenGames = new List<GameScore>();
                gameScore.ChildrenGames.Add(GetMoveScores(game, playerColor, opponentColor, currentDepth, null, game));
                var bestGameAvg = gameScore.ChildrenGames[0].ChildrenGames!.Select(x => x.ChildrenCountWithLowerScore / (double)(x.ChildrenCountWithHigherScore + x.ChildrenCountWithLowerScore + x.ChildrenCountWithNeutralScore)).Min();
                var bestGames = gameScore.ChildrenGames[0].ChildrenGames!.Where(x => x.ChildrenCountWithLowerScore / (double)(x.ChildrenCountWithHigherScore + x.ChildrenCountWithLowerScore + x.ChildrenCountWithNeutralScore) == bestGameAvg);
                var gameAvg = bestGames.Max(x => x.ChildrenCountWithHigherScore);
                var g = bestGames.First(x => x.ChildrenCountWithHigherScore == gameAvg);
                return g.Move;
            }


            return null;
        }

        private static GameScore GetMoveScores(Game game, Colors playerColor, Colors opponentColor, int currentDepth, Move? move, Game currentGame)
        {
            var isCheckmate = game.IsCheckmate;
            var gameScore = new GameScore()
            {
                MyScore = game.GetScore(playerColor),
                OpponentScore = game.GetScore(opponentColor),
                OpponentWinsWithCheckmate = isCheckmate && game.PlayerToMove == playerColor,
                IWinWithCheckmate = isCheckmate && game.PlayerToMove == opponentColor,
                Move = move
            };
            if (gameScore.OpponentWinsWithCheckmate)
            {
                gameScore.OpponentScore += 1000;
            }

            if (gameScore.IWinWithCheckmate)
            {
                gameScore.MyScore += 500;
            }

            if (currentDepth <= MAX_DEPTH * 2 && !isCheckmate)
            {
                gameScore.ChildrenGames = new List<GameScore>();
                var legalMoves = MoveLegalityEvaluator.GetAllLegalMoves(game.Board, game.PlayerToMove, game.Moves);
                if (legalMoves.Any())
                {
                    var legalMoveSubset = legalMoves.ToList(); //.OrderBy(x => Guid.NewGuid()).Take((legalMoves.Count + 3) / 4).ToList();
                    foreach (var m in legalMoveSubset)
                    {
                        var clonedGame = (Game)game.Clone();
                        clonedGame.AddMove(m, false);
                        var score = GetMoveScores(clonedGame, playerColor, opponentColor, currentDepth + 1, m, currentGame);
                        //if(score.ScoreDiff >= gameScore.ScoreDiff - 8)
                        //{
                            gameScore.ChildrenGames.Add(score);
                        //}
                    }
                    if (gameScore.ChildrenGames.Any())
                    {
                        gameScore.ChildrenMinScoreDiff = gameScore.ChildrenGames.Select(x => x.ChildrenMinScoreDiff).Min();
                        gameScore.ChildrenMaxScoreDiff = gameScore.ChildrenGames.Select(x => x.ChildrenMaxScoreDiff).Max();
                        gameScore.TotalChildren = gameScore.ChildrenGames.Count();
                        gameScore.ChildrenAvgScoreDiff = gameScore.ChildrenGames.Select(x => x.ChildrenAvgScoreDiff * x.TotalChildren).Average();
                        int currentDiff = currentGame.GetScore(playerColor) - currentGame.GetScore(opponentColor);
                        gameScore.ChildrenCountWithHigherScore = gameScore.ChildrenGames.Count(x => x.ScoreDiff > currentDiff) + gameScore.ChildrenGames.Sum(x=> x.ChildrenCountWithHigherScore);
                        gameScore.ChildrenCountWithLowerScore = gameScore.ChildrenGames.Count(x => x.ScoreDiff < currentDiff) + gameScore.ChildrenGames.Sum(x => x.ChildrenCountWithLowerScore);
                        gameScore.ChildrenCountWithNeutralScore = gameScore.ChildrenGames.Count(x => x.ScoreDiff == currentDiff) + gameScore.ChildrenGames.Sum(x => x.ChildrenCountWithNeutralScore);
                    }
                    else
                    {
                        gameScore.ChildrenAvgScoreDiff = gameScore.ScoreDiff;
                        gameScore.ChildrenMaxScoreDiff = gameScore.ScoreDiff;
                        gameScore.ChildrenMinScoreDiff = gameScore.ScoreDiff;
                    }
                }
                else
                {
                    gameScore.ChildrenAvgScoreDiff = gameScore.ScoreDiff;
                    gameScore.ChildrenMaxScoreDiff = gameScore.ScoreDiff;
                    gameScore.ChildrenMinScoreDiff = gameScore.ScoreDiff;
                }
            }
            else
            {
                gameScore.ChildrenAvgScoreDiff = gameScore.ScoreDiff;
                gameScore.ChildrenMaxScoreDiff = gameScore.ScoreDiff;
                gameScore.ChildrenMinScoreDiff = gameScore.ScoreDiff;
            }

            return gameScore;
        }

        private class GameScore
        {
            public int MyScore { get; set; }
            public int OpponentScore { get; set; }
            public bool OpponentWinsWithCheckmate { get; set; }
            public bool IWinWithCheckmate { get; set; }
            public List<GameScore>? ChildrenGames { get; set; }
            public int TotalChildren { get; set; } = 1;

            public int ChildrenCountWithHigherScore { get; set; } = 0;
            public int ChildrenCountWithLowerScore { get; set; } = 0;
            public int ChildrenCountWithNeutralScore { get; set; } = 0;

            public int ChildrenMaxScoreDiff { get; set; }
            public int ChildrenMinScoreDiff { get; set; }
            public double ChildrenAvgScoreDiff { get; set; }
            public Move? Move { get; set; }

            public int ScoreDiff
            {
                get
                {
                    return MyScore - OpponentScore;
                }
            }

        }

    }
}
