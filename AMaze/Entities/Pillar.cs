namespace AMaze.Entities;

internal class Pillar : IEntity
{
	private readonly Geometry.Face face;
	private readonly double top;

	public Pillar(double x, double y, double top)
	{
		face = new Geometry.Face(x, y, 0.3);
		this.top = top;
	}

	public bool Intersect(Geometry.Seg sight, out ScanIntersection intersection)
	{
		if (face.Intersect(sight, out intersection))
		{
			intersection.top = top;
			intersection.headSec = new ScanIntersectionSection { bottom = -1.0 };
			return true;
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Geometry.Rect rect)
	{
		return face.DoesIntersect(rect);
	}
}
