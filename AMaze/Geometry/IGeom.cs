namespace AMaze.Geometry;

internal interface IGeom
{
	bool Intersect(Seg sight, out ScanIntersection intersection);

	bool DoesIntersect(Rect rect);
}
