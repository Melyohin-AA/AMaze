namespace AMaze.Shapes;

internal class Circle : IShape
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

	public void Intersect(Ray ray, List<(double, double)> intersections)
	{
		double dx = ray.originX - X, dy = ray.originY - Y;
		double a = ray.dirX * ray.dirX + ray.dirY * ray.dirY;
		double b = (dx * ray.dirX + dy * ray.dirY) * 2.0;
		double c = dx * dx + dy * dy - Radius * Radius;
		double D = b * b - 4 * a * c;
		if (D < 0) return;
		double sqrtD = Math.Sqrt(D);
		double t1 = (-b - sqrtD) / (2 * a);
		double t2 = (-b + sqrtD) / (2 * a);
		if (t1 >= 0.0)
			intersections.Add((ray.originX + t1 * ray.dirX, ray.originY + t1 * ray.dirY));
		if ((t2 >= 0.0) && (D != 0.0))
			intersections.Add((ray.originX + t2 * ray.dirX, ray.originY + t2 * ray.dirY));
	}
}
