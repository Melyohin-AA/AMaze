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

	public bool Intersect(Ray ray, out (double, double) intersection)
	{
		intersection = default;
		if (ray.dirY == 0.0)
			return false;
		double tg = (Y - ray.originY) / ray.dirY;
		if (tg < 0.0)
			return false;
		double x = ray.originX + tg * ray.dirX;
		if ((x < X) || (x > X + Length))
			return false;
		intersection = (x, Y);
		return true;
	}

	public bool DoesIntersect(Rect rect)
	{
		//if ((Y < rect.y1) || (Y > rect.y2))
		//	return false;
		//if ((X + Length < rect.x1) || (X > rect.x2))
		//	return false;
		//return true;
		return (Y >= rect.y1) && (Y <= rect.y2) && (X + Length >= rect.x1) && (X <= rect.x2);
	}
}
