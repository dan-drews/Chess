using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine.LichessBot
{
    public class LichessGameState
    {
        public string type { get; set; }
        public string moves { get; set; }
        public int wtime { get; set; }
        public int btime { get; set; }
        public int winc { get; set; }
        public int binc { get; set; }
        public string status { get; set; }
    }


    public class LichessGame
    {
        public string id { get; set; }
        public LichessGameState state { get; set; }
    }

}
