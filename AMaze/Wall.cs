namespace AMaze;

internal class Wall
{
	public Geometry.IGeom Geom { get; }
	public bool IsVisible { get; }
	public bool IsGhost { get; }
	public bool AltPalette { get; }

	public Wall(Geometry.IGeom geom, bool isVisible, bool isGhost, bool altPalette)
	{
		Geom = geom;
		IsVisible = isVisible;
		IsGhost = isGhost;
		AltPalette = altPalette;
	}
}
