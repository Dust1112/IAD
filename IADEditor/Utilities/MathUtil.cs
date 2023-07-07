using System;
using System.Diagnostics;

namespace IADEditor.Utilities
{
    public static class MathUtil
    {
        public static float Epsilon => 0.00001f;

        public static bool IsTheSameAs(this float value, float other)
        {
            return Math.Abs(value - other) < Epsilon;
        }

        public static bool IsTheSameAs(this float? value, float? other)
        {
            if (!value.HasValue || !other.HasValue) { return false; }
            return Math.Abs(value.Value - other.Value) < Epsilon;
        }

        // Align by rounding up. Will result in a multiple of 'alignment' that is greater than or equal to 'size'.
        public static long AlignSizeUp(long size, long alignment)
        {
            Debug.Assert(alignment > 0, "Alignment must be non-zero.");
            long mask = alignment - 1;
            Debug.Assert((alignment & mask) == 0, "Alignment should be a power of 2.");
            return ((size + mask) & ~mask);
        }

        // Align by rounding down. Will result in a multiple of 'alignment' that is less than or equal to 'size'.
        public static long AlignSizeDown(long size, long alignment)
        {
            Debug.Assert(alignment > 0, "Alignment must be non-zero.");
            long mask = alignment - 1;
            Debug.Assert((alignment & mask) == 0, "Alignment should be a power of 2.");
            return (size & ~mask);
        }
    }
}
