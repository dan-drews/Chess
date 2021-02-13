using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Attributes
{
    public class PieceScoreAttribute : Attribute
    {
        public int Score { get; set; }
        public PieceScoreAttribute(int score)
        {
            Score = score;
        }
    }
}
