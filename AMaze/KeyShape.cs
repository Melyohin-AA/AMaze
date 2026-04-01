namespace AMaze;

internal enum KeyShape
{
	Diamond = 0,
	Eye = 1,
	H = 2,
}

internal static class KeyShapeExt
{
	public const int Count = 3;
	public const double Size = 0.5, Half = Size / 2.0;

	public static double Height(this KeyShape shape, double i2cDX, double i2cDY)
	{
		return shape.Height(i2cDX * i2cDX + i2cDY * i2cDY);
	}
	public static double Height(this KeyShape shape, double i2cDist2)
	{
		return shape switch {
			KeyShape.Diamond => Math.Clamp(Half - Math.Sqrt(i2cDist2), 0.0, Half),
			KeyShape.Eye => (Half * Half - i2cDist2) * 2.0,
			KeyShape.H => i2cDist2 * 2.0 + 0.05,
			_ => throw new NotSupportedException(),
		};
	}
}
