using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChessLibrary.Boards
{
    public readonly struct BitBoard
    {
        private readonly ulong _valueInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BitBoard(ulong value)
        {
            _valueInternal = value;
        }

        public static implicit operator ulong(BitBoard b) => b._valueInternal;
        public static implicit operator BitBoard(ulong u) => new BitBoard(u);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard operator &(BitBoard a, BitBoard b) => a._valueInternal & b._valueInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard operator |(BitBoard a, BitBoard b) => a._valueInternal | b._valueInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard operator ~(BitBoard a) => ~a._valueInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard operator ^(BitBoard a, BitBoard b) => a._valueInternal ^ b._valueInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard operator <<(BitBoard a, int b) => a._valueInternal << b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitBoard operator >>(BitBoard a, int b) => a._valueInternal >> b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(BitBoard a, BitBoard b) => a._valueInternal == b._valueInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(BitBoard a, BitBoard b) => a._valueInternal != b._valueInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FirstBit() => _valueInternal.NumberOfTrailingZeros();

        public int Count => _valueInternal.CountSetBits();

        //public bool this[int index]
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    get => (_valueInternal & (1UL << index)) != 0;
        //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //    set
        //    {
        //        if (value)
        //        {
        //            _valueInternal |= 1UL << index;
        //        }
        //        else
        //        {
        //            _valueInternal &= ~(1UL << index);
        //        }
        //    }
        //}

        public override bool Equals(object? obj)
        {
            return obj is BitBoard board &&
                   _valueInternal == board._valueInternal;
        }

        public override int GetHashCode()
        {
            return _valueInternal.GetHashCode();
        }

        public BitBoardIterator GetEnumerator()
        {
            return new BitBoardIterator(_valueInternal, Count);
        }
    }

    public ref struct BitBoardIterator
    {
        private readonly int Count;
        private ulong Value;
        private int Index;

        public BitBoardIterator(ulong value, int count)
        {
            Value = value;
            Count = count;
            Index = 0;
        }

        public bool MoveNext()
        {
            Index++;
            return Index <= Count;
        }

        public int Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int i = Value.NumberOfTrailingZeros();
                Value &= ~(1UL << i);
                return i;
            }
        }
    }
}
