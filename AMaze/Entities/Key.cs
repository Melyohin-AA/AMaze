namespace AMaze.Entities;

internal class Key : IEntity
{
	private readonly Geometry.Face face;

	public event Action<Key>? PickupCallback;

	public Key(double x, double y)
	{
		face = new Geometry.Face(x, y, 0.5);
	}

	public bool Intersect(Geometry.Seg sight, out ((double, double), ScanIntersectionExtra) intersection)
	{
		if (face.Intersect(sight, out var point, out double dist2))
		{
			double dist = Math.Sqrt(dist2);
			var extra = new ScanIntersectionExtra {
				top = face.Radius - dist - 1, bottom = dist - 1, altPalette = true,
			};
			intersection = (point, extra);
			return true;
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Geometry.Rect rect)
	{
		if (face.DoesIntersect(rect))
			PickupCallback?.Invoke(this);
		return false;
	}
}
