namespace AMaze.Shapes;

internal class Rect : IShape
{
	public double X1 { get; }
	public double Y1 { get; }
	public double X2 { get; }
	public double Y2 { get; }

	public Rect(double x1, double y1, double x2, double y2)
	{
		X1 = x1;
		Y1 = y1;
		X2 = x2;
		Y2 = y2;
	}

	public void Intersect(Ray ray, List<(double, double)> intersections)
	{
		(double, double) point;
		if (IntersectVertically(X1, ray, out point))
			intersections.Add(point);
		if (IntersectVertically(X2, ray, out point))
			intersections.Add(point);
		if (IntersectHorizontally(Y1, ray, out point))
			intersections.Add(point);
		if (IntersectHorizontally(Y2, ray, out point))
			intersections.Add(point);
	}

	private bool IntersectVertically(double x, Ray ray, out (double, double) point)
	{
		point = default;
		if (ray.dirX == 0.0)
			return false;
		double tg = (x - ray.originX) / ray.dirX;
		if (tg < 0.0)
			return false;
		double y = ray.originY + tg * ray.dirY;
		if ((y < Y1) || (y > Y2))
			return false;
		point = (x, y);
		return true;
	}
	private bool IntersectHorizontally(double y, Ray ray, out (double, double) point)
	{
		point = default;
		if (ray.dirY == 0.0)
			return false;
		double tg = (y - ray.originY) / ray.dirY;
		if (tg < 0.0)
			return false;
		double x = ray.originX + tg * ray.dirX;
		if ((x < X1) || (x > X2))
			return false;
		point = (x, y);
		return true;
	}
}
