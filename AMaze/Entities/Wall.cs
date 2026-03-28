namespace AMaze.Entities;

internal class Wall : IEnity
{
	public Geometry.IGeom Geom { get; }
	public double Top { get; }
	public double Bottom { get; }
	public bool IsVisible { get; set; }
	public bool IsGhost { get; set; }
	public bool Opaque { get; }
	public bool AltPalette { get; }

	public Wall(Geometry.IGeom geom, double top = 1.0, double bottom = -1.0,
		bool isVisible = true, bool isGhost = false, bool opaque = true, bool altPalette = false)
	{
		Geom = geom;
		Top = top;
		Bottom = bottom;
		IsVisible = isVisible;
		IsGhost = isGhost;
		Opaque = opaque;
		AltPalette = altPalette;
	}

	public bool Intersect(Geometry.Seg ray, out ((double, double), ScanIntersectionExtra) intersection)
	{
		if (IsVisible && Geom.Intersect(ray, out var point))
		{
			var extra = new ScanIntersectionExtra {
				top = Top, bottom = Bottom,
				opaque = Opaque, altPalette = AltPalette,
			};
			intersection = (point, extra);
			return true;
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Geometry.Rect rect)
	{
		return !IsGhost && Geom.DoesIntersect(rect);
	}
}
