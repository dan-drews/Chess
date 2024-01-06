using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChessLibrary.Boards
{
    public static class ULongExtensions
    //public readonly struct BitBoard
    {
        //private readonly ulong _valueInternal;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private BitBoard(ulong value)
        //{
        //    _valueInternal = value;
        //}

        //public static implicit operator ulong(BitBoard b) => b._valueInternal;
        //public static implicit operator BitBoard(ulong u) => new BitBoard(u);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static BitBoard operator &(BitBoard a, BitBoard b) => a._valueInternal & b._valueInternal;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static BitBoard operator |(BitBoard a, BitBoard b) => a._valueInternal | b._valueInternal;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static BitBoard operator ~(BitBoard a) => ~a._valueInternal;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static BitBoard operator ^(BitBoard a, BitBoard b) => a._valueInternal ^ b._valueInternal;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static BitBoard operator <<(BitBoard a, int b) => a._valueInternal << b;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static BitBoard operator >>(BitBoard a, int b) => a._valueInternal >> b;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool operator ==(BitBoard a, BitBoard b) => a._valueInternal == b._valueInternal;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool operator !=(BitBoard a, BitBoard b) => a._valueInternal != b._valueInternal;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FirstBit(this ulong value) => value.NumberOfTrailingZeros();

        public static int Count(this ulong value) => value.CountSetBits();
        //public int Count => _valueInternal.CountSetBits();

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

        //public override bool Equals(object? obj)
        //{
        //    return obj is BitBoard board &&
        //           _valueInternal == board._valueInternal;
        //}

        //public override int GetHashCode()
        //{
        //    return _valueInternal.GetHashCode();
        //}

        public static BitBoardIterator GetEnumerator(this ulong value)
        {
            var count = value.Count();
            return new BitBoardIterator(value, count);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if(Index >= Count)
            {
                return false;
            }
            if (Index > 0)
            {
                Value &= ~(1UL << Value.NumberOfTrailingZeros());
            }
            Index++;
            return true;
        }

        public int Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Value.NumberOfTrailingZeros();
            }
        }
    }
}
