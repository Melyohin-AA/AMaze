namespace AMaze.Geometry;

internal enum Orientation
{
	PosX = 0,
	NegX = 1,
	PosY = 2,
	NegY = 3,
}

internal static class OrientationExt
{
	public static bool IsHor(this Orientation orientation)
	{
		return ((int)orientation & 0b10) == 0;
	}
	public static bool IsVert(this Orientation orientation)
	{
		return ((int)orientation & 0b10) != 0;
	}

	public static int Sign(this Orientation orientation)
	{
		return (((int)orientation & 0b01) == 0) ? +1 : -1;
	}
}
