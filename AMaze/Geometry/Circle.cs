namespace AMaze.Geometry;

internal class Circle : IGeom
{
	public double X { get; }
	public double Y { get; }
	public double Radius { get; }

	public Circle(double x, double y, double radius)
	{
		X = x;
		Y = y;
		Radius = radius;
	}

	public bool Intersect(Seg ray, out (double, double) intersection)
	{
		intersection = default;
		double dx = ray.x1 - X, dy = ray.y1 - Y;
		double a = ray.x2 * ray.x2 + ray.y2 * ray.y2;
		double b = (dx * ray.x2 + dy * ray.y2) * 2.0;
		double c = dx * dx + dy * dy - Radius * Radius;
		double D = b * b - 4 * a * c;
		if (D < 0) return false;
		double sqrtD = Math.Sqrt(D);
		double t1 = (-b - sqrtD) / (2 * a);
		double t2 = (-b + sqrtD) / (2 * a);
		if ((t1 < 0.0) && (t2 < 0.0)) return false;
		// Returning only one of two solutions
		double t = (t1 >= 0.0) ? t1 : t2;
		intersection = (ray.x1 + t * ray.x2, ray.y1 + t * ray.y2);
		return true;
	}

	public bool DoesIntersect(Rect rect) => throw new NotSupportedException();
}
