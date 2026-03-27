namespace AMaze.Entities;

internal interface IEnity
{
	bool Intersect(Geometry.Ray ray, out ((double, double), ScanIntersectionExtra) intersection);

	bool DoesCollide(Geometry.Rect rect);
}
