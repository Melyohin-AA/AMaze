namespace AMaze.Shapes;

internal interface IShape
{
	public void Intersect(Ray ray, List<(double, double)> intersections);
}
