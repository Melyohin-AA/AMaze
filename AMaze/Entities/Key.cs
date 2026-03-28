namespace AMaze.Entities;

internal class Key : IEntity
{
	public Geometry.IGeom Geom { get; }

	public event Action<Key>? PickupCallback;

	public Key(double x, double y)
	{
		Geom = new Geometry.Face(x, y, 0.25);
	}

	public bool Intersect(Geometry.Seg ray, out ((double, double), ScanIntersectionExtra) intersection)
	{
		if (Geom.Intersect(ray, out var point))
		{
			var extra = new ScanIntersectionExtra { top = -0.5, bottom = -1.0, altPalette = true };
			intersection = (point, extra);
			return true;
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Geometry.Rect rect)
	{
		if (Geom.DoesIntersect(rect))
			PickupCallback?.Invoke(this);
		return false;
	}
}
