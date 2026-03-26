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

	public bool Intersect(Ray ray, out (double, double) intersection)
	{
		intersection = default;
		if (ray.dirX == 0.0)
			return false;
		double tg = (X - ray.originX) / ray.dirX;
		if (tg < 0.0)
			return false;
		double y = ray.originY + tg * ray.dirY;
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
