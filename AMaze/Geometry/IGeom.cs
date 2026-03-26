namespace AMaze.Geometry;

internal interface IGeom
{
	public bool Intersect(Ray ray, out (double, double) intersection);

	public bool DoesIntersect(Rect rect);
}
