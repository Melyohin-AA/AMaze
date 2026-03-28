namespace AMaze.Entities;

internal class Grid : IEnity
{
	public Geometry.IGeom Geom { get; }
	public double Top { get; }
	public double Bottom { get; }
	public double PhaseMult { get; }
	public double Threshold { get; }

	public Grid(Geometry.IGeom geom, double top = 1.0, double bottom = -1.0,
		double phaseMult = 2.0, double threshold = 0.5)
	{
		Geom = geom;
		Top = top;
		Bottom = bottom;
		PhaseMult = phaseMult;
		Threshold = threshold;
	}

	public bool Intersect(Geometry.Seg ray, out ((double, double), ScanIntersectionExtra) intersection)
	{
		if (Geom.Intersect(ray, out var point))
		{
			double phase = (point.Item1 + point.Item2) * PhaseMult;
			if (phase - Math.Truncate(phase) < Threshold)
			{
				var extra = new ScanIntersectionExtra { top = Top, bottom = Bottom, opaque = true };
				intersection = (point, extra);
				return true;
			}
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Geometry.Rect rect)
	{
		return Geom.DoesIntersect(rect);
	}
}
