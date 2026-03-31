using AMaze.Geometry;

namespace AMaze.Entities;

internal class Door : IEntity
{
	public const double KeyHoleSize = 0.5;
	public const int StagesUntilOpen = 66;

	public double CenterX { get; }
	public double CenterY { get; }
	public IGeom MainGeom { get; }
	public IGeom KeyHoleGeom { get; }
	public bool AltPalette { get; }
	public bool KeyApplied { get; set; }

	private int stagesUntilOpen = StagesUntilOpen;

	public Door((double, double) center, double size, Orientation normal, bool altPalette = true)
	{
		(CenterX, CenterY) = center;
		(MainGeom, KeyHoleGeom) = normal.IsVert() ? (
			(IGeom)new HorSeg(CenterX - size / 2, CenterY, size),
			(IGeom)new HorSeg(CenterX - KeyHoleSize / 2, CenterY + normal.Sign() / 16.0, KeyHoleSize)
		) : (
			(IGeom)new VertSeg(CenterX, CenterY - size / 2, size),
			(IGeom)new VertSeg(CenterX + normal.Sign() / 16.0, CenterY - KeyHoleSize / 2, KeyHoleSize)
		);
		AltPalette = altPalette;
	}

	public bool Intersect(Seg sight, out ScanIntersection intersection)
	{
		if (MainGeom.Intersect(sight, out intersection))
		{
			intersection.top = 1.0;
			intersection.opaque = stagesUntilOpen == StagesUntilOpen;
			if (!KeyApplied && KeyHoleGeom.Intersect(sight, out var khInter))
			{
				double i2cDX = intersection.x - CenterX, i2cDY = intersection.y - CenterY;
				double i2cDist = Math.Sqrt(i2cDX * i2cDX + i2cDY * i2cDY);
				double keyHoleHalf = Math.Clamp(KeyHoleSize / 2 - i2cDist, 0.0, KeyHoleSize / 2);
				intersection.headSec = new ScanIntersectionSection { bottom = keyHoleHalf, altPalette = AltPalette };
				intersection.extraSecs = new[] {
					new ScanIntersectionSection { bottom = -keyHoleHalf, vantablack = true },
					new ScanIntersectionSection { bottom = -1.0, altPalette = AltPalette },
				};
				return true;
			}
			intersection.headSec = new ScanIntersectionSection { bottom = -1.0, altPalette = AltPalette };
			return true;
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Rect rect)
	{
		return MainGeom.DoesIntersect(rect);
	}
}
