using AMaze.Geometry;

namespace AMaze.Entities;

internal class Door : IEntity, IInteractable, IDynamic
{
	public const double KeyHoleSize = 0.5;
	public const int StagesUntilOpen = 5000 / 33; // 5 seconds

	private readonly Game game;
	private readonly IGeom mainGeom;
	private readonly IGeom keyHoleGeom;

	public double CenterX { get; }
	public double CenterY { get; }
	public bool AltPalette { get; }
	public bool KeyApplied { get; set; }

	private int stagesUntilOpen = StagesUntilOpen;

	public Door(Game game, (double, double) center, double size, Orientation normal, bool altPalette = true)
	{
		this.game = game;
		(CenterX, CenterY) = center;
		(mainGeom, keyHoleGeom) = normal.IsVert() ? (
			(IGeom)new HorSeg(CenterX - size / 2, CenterY, size),
			(IGeom)new HorSeg(CenterX - KeyHoleSize / 2, CenterY + normal.Sign() / 16.0, KeyHoleSize)
		) : (
			(IGeom)new VertSeg(CenterX, CenterY - size / 2, size),
			(IGeom)new VertSeg(CenterX + normal.Sign() / 16.0, CenterY - KeyHoleSize / 2, KeyHoleSize)
		);
		AltPalette = altPalette;
	}

	private double CalcBottom()
	{
		// maps [0, max] to [0.8, -1]
		return 0.8 - stagesUntilOpen / (double)StagesUntilOpen * 1.8;
	}

	public bool Intersect(Seg sight, out ScanIntersection intersection)
	{
		if (mainGeom.Intersect(sight, out intersection))
		{
			intersection.top = 1.0;
			intersection.opaque = stagesUntilOpen == StagesUntilOpen;
			if (!KeyApplied && keyHoleGeom.Intersect(sight, out var khInter))
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
			intersection.headSec = new ScanIntersectionSection { bottom = CalcBottom(), altPalette = AltPalette };
			return true;
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Rect rect)
	{
		return (CalcBottom() < 0.0) && mainGeom.DoesIntersect(rect);
	}

	public void Interact()
	{
		if (!KeyApplied && (game.Player.KeyCount > 0))
		{
			KeyApplied = true;
			game.Player.KeyCount--;
		}
	}

	public void Tick()
	{
		if (KeyApplied && (stagesUntilOpen > 0))
			stagesUntilOpen--;
	}
}
