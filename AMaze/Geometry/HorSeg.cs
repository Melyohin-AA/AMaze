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

	public bool Intersect(Seg sight, out (double, double) intersection)
	{
		intersection = default;
		if (sight.y1 == sight.y2)
			return false;
		double t = (Y - sight.y1) / (sight.y2 - sight.y1);
		if ((t < 0.0) || (t > 1.0))
			return false;
		double x = sight.x1 + t * (sight.x2 - sight.x1);
		if ((x < X) || (x > X + Length))
			return false;
		intersection = (x, Y);
		return true;
	}

	public bool DoesIntersect(Rect rect)
	{
		return (Y >= rect.y1) && (Y <= rect.y2) && (X + Length >= rect.x1) && (X <= rect.x2);
	}
}
