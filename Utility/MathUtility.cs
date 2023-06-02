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
		public static double Ldexp(double x, int exp) => x * Math.Pow(2, exp);
	}
}
