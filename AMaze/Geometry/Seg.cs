namespace AMaze.Geometry;

internal struct Seg
{
	public double x1, y1;
	public double x2, y2;

	public double DX => x2 - x1;
	public double DY => y2 - y1;
}
