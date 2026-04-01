namespace AMaze.Entities;

internal class Key : IEntity, IInteractable
{
	private readonly Game game;
	private readonly Geometry.Face face;

	public Key(Game game, double x, double y)
	{
		this.game = game;
		face = new Geometry.Face(x, y, 0.25);
	}

	public bool Intersect(Geometry.Seg sight, out ScanIntersection intersection)
	{
		if (face.Intersect(sight, out intersection, out double dist2))
		{
			double dist = Math.Sqrt(dist2);
			intersection.top = face.Radius - dist;
			intersection.headSec = new ScanIntersectionSection { bottom = dist - face.Radius, altPalette = true };
			return true;
		}
		intersection = default;
		return false;
	}

	public bool DoesCollide(Geometry.Rect rect) => false;

	public void Interact()
	{
		game.Player.KeyCount++;
		game.DespawnEntity(this);
	}
}
