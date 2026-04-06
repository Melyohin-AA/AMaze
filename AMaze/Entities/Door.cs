using AMaze.Geometry;

namespace AMaze.Entities;

internal class Door : IEntity, IInteractable, IDynamic
{
	public const int Dur = 5000, StagesUntilOpen = Dur / 33; // 5 seconds

	private readonly Game game;
	private readonly IGeom mainGeom;
	private readonly IGeom keyHoleGeom;

	public double CenterX { get; }
	public double CenterY { get; }
	public KeyShape KeyHoleShape { get; }
	public bool AltPalette { get; }
	public bool KeyApplied { get; set; }

	private int stagesUntilOpen = StagesUntilOpen;
	private Sound.AudioPanned? shiftSound;

	public Door(Game game, (double, double) center, double size, Orientation normal, KeyShape keyHoleShape,
		bool altPalette = true)
	{
		this.game = game;
		(CenterX, CenterY) = center;
		KeyHoleShape = keyHoleShape;
		double khOffset = normal.Sign() / 256.0;
		(mainGeom, keyHoleGeom) = normal.IsVert() ? (
			(IGeom)new HorSeg(CenterX - size / 2, CenterY, size),
			(IGeom)new HorSeg(CenterX - KeyShapeExt.Half, CenterY + khOffset, KeyShapeExt.Size)
		) : (
			(IGeom)new VertSeg(CenterX, CenterY - size / 2, size),
			(IGeom)new VertSeg(CenterX + khOffset, CenterY - KeyShapeExt.Half, KeyShapeExt.Size)
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
				double keyHoleHalf = KeyHoleShape.Height(intersection.x - CenterX, intersection.y - CenterY);
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
		return (CalcBottom() < 0.1) && mainGeom.DoesIntersect(rect);
	}

	public void Interact()
	{
		KeyApplied = KeyApplied || game.Player.UseKey(KeyHoleShape);
	}
	public bool CanInteract()
	{
		return !KeyApplied && (game.Player.KeyCount(KeyHoleShape) > 0);
	}

	public void Tick()
	{
		if (KeyApplied && (stagesUntilOpen > 0))
		{
			if (stagesUntilOpen == StagesUntilOpen)
			{
				(float lVol, float rVol) = game.Player.GetSpatialVolume(CenterX, CenterY);
				(var mono, shiftSound) = game.StereoPlayer.GetAudio("shift");
				shiftSound.Pan(lVol, rVol);
				uint loopCount = (uint)(mono.Format.SampleRate * Dur / (mono.Size * 500)) + 1;
				shiftSound.SubmitSourceBuffer(loopCount);
				shiftSound.Play();
			}
			else
			{
				(float lVol, float rVol) = game.Player.GetSpatialVolume(CenterX, CenterY);
				shiftSound!.Pan(lVol, rVol);
			}
			if (--stagesUntilOpen == 0)
			{
				shiftSound.Stop();
				shiftSound = null;
			}
		}
	}
}
