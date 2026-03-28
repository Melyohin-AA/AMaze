namespace AMaze.Entities;

internal interface IEntity
{
	bool Intersect(Geometry.Seg sight, out ((double, double), ScanIntersectionExtra) intersection);

	bool DoesCollide(Geometry.Rect rect);
}
