namespace AMaze.Geometry;

internal interface IGeom
{
	bool Intersect(Seg ray, out (double, double) intersection);

	bool DoesIntersect(Rect rect);
}
