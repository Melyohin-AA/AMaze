namespace AMaze.Geometry;

internal interface IGeom
{
	public bool Intersect(Seg seg, out (double, double) intersection);
}
