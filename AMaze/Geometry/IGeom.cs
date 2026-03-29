namespace AMaze.Geometry;

internal interface IGeom
{
	bool Intersect(Seg sight, out (double, double) intersection);

	bool DoesIntersect(Rect rect);
}
