namespace AMaze.Geometry;

internal class HorSeg : IGeom
{
	public double X { get; }
	public double Y { get; }
	public double Length { get; }

	public HorSeg(double x, double y, double length)
	{
		X = x;
		Y = y;
		Length = length;
	}

	public bool Intersect(Seg seg, out (double, double) intersection)
	{
		intersection = default;
		if (seg.y1 == seg.y2)
			return false;
		double tg = (Y - seg.y1) / (seg.y2 - seg.y1);
		if ((tg < 0.0) || (tg > 1.0))
			return false;
		double x = seg.x1 + tg * (seg.x2 - seg.x1);
		if ((x < X) || (x > X + Length))
			return false;
		intersection = (x, Y);
		return true;
	}
}
