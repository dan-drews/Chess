using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.MoveGeneration
{
    public ref struct MoveListContainer
    {
        public MoveListContainer(Span<Move> moves)
        {
            _moves = moves;
            Count = 0;
        }

        private Span<Move> _moves { get; }
        public int Count { get; private set; }

        public void Add(Move move)
        {
            _moves[Count] = move;
            Count++;
        }

        public Move this[int index] => _moves[index];

    }
}
