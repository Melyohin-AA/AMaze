namespace AMaze.Entities;

internal class Key : IEntity
{
	private readonly Geometry.Face face;

	public event Action<Key>? PickupCallback;

	public Key(double x, double y)
	{
		face = new Geometry.Face(x, y, 0.25, 0.5);
	}

	public bool Intersect(Geometry.Seg sight, out ScanIntersection intersection)
	{
		if (face.Intersect(sight, out intersection, out double dist2))
		{
			double dist = Math.Sqrt(dist2);
			intersection.top = face.VisRadius - dist;
			intersection.headSec = new ScanIntersectionSection { bottom = dist - face.VisRadius, altPalette = true };
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
