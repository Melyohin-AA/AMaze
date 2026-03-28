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

	public bool Intersect(Seg sight, out (double, double) intersection)
	{
		intersection = default;
		if (sight.x1 == sight.x2)
			return false;
		double t = (X - sight.x1) / (sight.x2 - sight.x1);
		if ((t < 0.0) || (t > 1.0))
			return false;
		double y = sight.y1 + t * (sight.y2 - sight.y1);
		if ((y < Y) || (y > Y + Length))
			return false;
		intersection = (X, y);
		return true;
	}

	public bool DoesIntersect(Rect rect)
	{
		return (X >= rect.x1) && (X <= rect.x2) && (Y + Length >= rect.y1) && (Y <= rect.y2);
	}
}
