namespace AMaze.Entities;

internal interface IEnity
{
	bool Intersect(Geometry.Seg sight, out ((double, double), ScanIntersectionExtra) intersection);

	bool DoesCollide(Geometry.Rect rect);
}
