namespace AMaze.Entities;

internal class Key : IEntity, IInteractable
{
	private readonly Game game;
	private readonly KeyShape shape;
	private readonly Geometry.Face face;

	public Key(Game game, KeyShape shape, double x, double y)
	{
		this.game = game;
		this.shape = shape;
		face = new Geometry.Face(x, y, KeyShapeExt.Half);
	}

	public bool Intersect(Geometry.Seg sight, out ScanIntersection intersection)
	{
		if (face.Intersect(sight, out intersection, out double dist2))
		{
			double half = shape.Height(dist2);
			intersection.top = half;
			intersection.headSec = new ScanIntersectionSection { bottom = -half, altPalette = true };
			return true;
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Geometry.Rect rect) => false;

	public void Interact()
	{
		game.Player.AddKey(shape);
		game.DespawnEntity(this);
		(_, var panned) = game.StereoPlayer.GetAudio("rustle");
		(float lVol, float rVol) = game.Player.GetSpatialVolume(face.X, face.Y);
		panned.Pan(lVol, rVol);
		panned.SubmitSourceBuffer(0);
		panned.Play();
	}
	public bool CanInteract() => true;
}
