using System.Runtime.CompilerServices;

namespace FusionCalculator.Utility
{
	public interface MathUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Gcd(int a, int b)
		{
			while (b > 0)
				(a, b) = (b, a % b);

			return a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinaryGcd(int a, int b)
		{
			if (b == 0)
				return a;

			if (a == 0)
				return b;

			if (a < 0)
				a = Math.Abs(a);

			if (b < 0)
				b = Math.Abs(b);

			if ((a & 1) == 0 && (b & 1) == 0)
				return BinaryGcd(a >> 1, b >> 1) << 1;

			if ((a & 1) == 0)
				return BinaryGcd(a >> 1, b);

			if ((b & 1) == 0)
				return BinaryGcd(a, b >> 1);

			if (a >= b)
				return BinaryGcd((a - b) >> 1, b);

			return BinaryGcd(a, (b - a) >> 1);
		}
	}
}
