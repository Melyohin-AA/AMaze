namespace AMaze.Geometry;

internal class VertSeg : IGeom
{
	public double X { get; }
	public double Y { get; }
	public double Length { get; }

	public VertSeg(double x, double y, double length)
	{
		X = x;
		Y = y;
		Length = length;
	}

	public bool Intersect(Seg seg, out (double, double) intersection)
	{
		intersection = default;
		if (seg.x1 == seg.x2)
			return false;
		double tg = (X - seg.x1) / (seg.x2 - seg.x1);
		if ((tg < 0.0) || (tg > 1.0))
			return false;
		double y = seg.y1 + tg * (seg.y2 - seg.y1);
		if ((y < Y) || (y > Y + Length))
			return false;
		intersection = (X, y);
		return true;
	}
}
