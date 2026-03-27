namespace AMaze.Geometry;

internal interface IGeom
{
	bool Intersect(Ray ray, out (double, double) intersection);

	bool DoesIntersect(Rect rect);
}
