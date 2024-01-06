using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    internal static class ArrayUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T UnsafeArrayAccess<T>(this T[] array, int position)
        {
            return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), position);
        }
    }
}
