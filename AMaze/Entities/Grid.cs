namespace AMaze.Entities;

internal class Grid : IEntity
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

	public bool Intersect(Geometry.Seg ray, out ScanIntersection intersection)
	{
		if (Geom.Intersect(ray, out intersection))
		{
			double phase = (intersection.x + intersection.y) * PhaseMult;
			if (phase - Math.Truncate(phase) < Threshold)
			{
				intersection.top = Top;
				intersection.opaque = true;
				intersection.headSec.bottom = Bottom;
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
