namespace AMaze;

internal struct ScanIntersection
{
	public double x, y;
	public double dist2;
	public double top;
	public bool opaque;
	public ScanIntersectionSection headSec;
	public ScanIntersectionSection[]? extraSecs;

	public void Screen(double innerDist, double dist, double offset)
	{
		top = Screen(top, innerDist, dist, offset);
		headSec.bottom = Screen(headSec.bottom, innerDist, dist, offset);
		if (extraSecs != null)
			for (int i = 0; i < extraSecs.Length; i++)
				extraSecs[i].bottom = Screen(extraSecs[i].bottom, innerDist, dist, offset);
	}

	private static double Screen(double y, double innerDist, double dist, double offset)
	{
		return y * innerDist / dist - offset;
	}
}

internal struct ScanIntersectionSection
{
	public double bottom;
	public bool altPalette, vantablack;
}
