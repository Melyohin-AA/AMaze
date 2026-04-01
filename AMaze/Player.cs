namespace AMaze;

internal class Player
{
	private readonly int[] keyCount = new int[KeyShapeExt.Count];

	public double HitboxHalf { get; }
	public double InteractionDist { get; }

	public double X { get; set; }
	public double Y { get; set; }
	public double Rot { get; set; }

	public Player(double hitboxSize, double interactionDist)
	{
		HitboxHalf = hitboxSize / 2.0;
		InteractionDist = interactionDist;
	}

	public void Move(double step, double rotOffset, Span<Entities.IEntity> entities)
	{
		double dx = Math.Cos(Rot + rotOffset) * step;
		double dy = Math.Sin(Rot + rotOffset) * step;
		(double, double) vxy = (dx, dy), vx = (dx, 0.0), vy = (0.0, dy);
		Span<(double, double)> vectors = stackalloc (double, double)[3];
		vectors[0] = vxy;
		(vectors[1], vectors[2]) = (Math.Abs(dx) > Math.Abs(dy)) ? (vx, vy) : (vy, vx);
		foreach ((double vdx, double vdy) in vectors)
		{
			if (DoesCollide(MakeHitbox(X + vdx, Y + vdy), entities)) continue;
			X += vdx;
			Y += vdy;
			return;
		}
	}
	private static bool DoesCollide(Geometry.Rect hitbox, Span<Entities.IEntity> entities)
	{
		for (int i = 0; i < entities.Length; i++)
			if (entities[i].DoesCollide(hitbox))
				return true;
		return false;
	}
	private Geometry.Rect MakeHitbox(double x, double y)
	{
		return new Geometry.Rect { x1 = x - HitboxHalf, y1 = y - HitboxHalf, x2 = x + HitboxHalf, y2 = y + HitboxHalf };
	}

	public void Rotate(double step)
	{
		Rot += step;
		if (Rot > Math.PI * 2)
			Rot -= Math.PI * 2;
		else if (Rot < -Math.PI * 2)
			Rot += Math.PI * 2;
	}

	public void Interact(Span<Entities.IEntity> entities)
	{
		var seg = MakeInteractionSeg();
		for (int i = 0; i < entities.Length; i++)
			if ((entities[i] is Entities.IInteractable interactable) && entities[i].Intersect(seg, out var _))
				interactable.Interact();
	}
	public bool CanInteract(Span<Entities.IEntity> entities)
	{
		var seg = MakeInteractionSeg();
		for (int i = 0; i < entities.Length; i++)
			if ((entities[i] is Entities.IInteractable interactable) && entities[i].Intersect(seg, out var _)
					&& interactable.CanInteract())
				return true;
		return false;
	}
	private Geometry.Seg MakeInteractionSeg()
	{
		return new Geometry.Seg {
			x1 = X, x2 = X + Math.Cos(Rot) * InteractionDist,
			y1 = Y, y2 = Y + Math.Sin(Rot) * InteractionDist,
		};
	}

	public int KeyCount(KeyShape shape)
	{
		return keyCount[(int)shape];
	}
	public void AddKey(KeyShape shape)
	{
		keyCount[(int)shape]++;
	}
	public bool UseKey(KeyShape shape)
	{
		if (keyCount[(int)shape] > 0)
		{
			keyCount[(int)shape]--;
			return true;
		}
		return false;
	}
}
