namespace AMaze.Entities;

internal class Key : IEnity
{
	public Geometry.IGeom Geom { get; }
	public Wall Door { get; }

	private bool pickedUp;

	public Key(double x, double y, Wall door)
	{
		Geom = new Geometry.Face(x, y, 0.25);
		Door = door;
	}

	public bool Intersect(Geometry.Seg ray, out ((double, double), ScanIntersectionExtra) intersection)
	{
		if (!pickedUp && Geom.Intersect(ray, out var point))
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
		if (!pickedUp && Geom.DoesIntersect(rect))
		{
			pickedUp = true;
			Door.IsGhost = true;
			Door.IsVisible = false;
		}
		return false;
	}
}
