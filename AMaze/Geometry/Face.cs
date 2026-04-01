namespace AMaze.Geometry;

internal class Face : IGeom
{
	public double X { get; }
	public double Y { get; }
	public double Radius { get; }
	public double Radius2 { get; }

	public Face(double x, double y, double radius)
	{
		X = x;
		Y = y;
		Radius = radius;
		Radius2 = radius * radius;
	}

	public bool Intersect(Seg sight, out ScanIntersection intersection)
	{
		return Intersect(sight, out intersection, out _);
	}
	public bool Intersect(Seg sight, out ScanIntersection intersection, out double dist2)
	{
		intersection = default;
		dist2 = double.NaN;
		double sightVecX = sight.x2 - sight.x1, sightVecY = sight.y2 - sight.y1;
		double projVecX = X - sight.x1, projVecY = Y - sight.y1;
		double sightLen2 = sightVecX * sightVecX + sightVecY * sightVecY;
		double t = (projVecX * sightVecX + projVecY * sightVecY) / sightLen2;
		if ((t < 0.0) || (t > 1.0))
			return false;
		intersection = new ScanIntersection { x = sight.x1 + t * sightVecX, y = sight.y1 + t * sightVecY };
		double dx = intersection.x - X, dy = intersection.y - Y;
		dist2 = dx * dx + dy * dy;
		return dist2 <= Radius2;
	}

	public bool DoesIntersect(Rect rect)
	{
		// This is very simplified
		double dx = (rect.x1 + rect.x2) / 2.0 - X, dy = (rect.y1 + rect.y2) / 2.0 - Y;
		double dist2 = dx * dx + dy * dy;
		return dist2 <= Radius2;
	}
}
