namespace AMaze;

internal class Player
{
	public double HitboxHalf { get; }
	public double InteractionDist { get; }

	public double X { get; set; }
	public double Y { get; set; }
	public double Rot { get; set; }
	public int KeyCount { get; set; }

	public Player(double hitboxSize, double interactionDist)
	{
		HitboxHalf = hitboxSize / 2.0;
		InteractionDist = interactionDist;
	}

	private Geometry.Rect MakeHitbox(double x, double y)
	{
		return new Geometry.Rect { x1 = x - HitboxHalf, y1 = y - HitboxHalf, x2 = x + HitboxHalf, y2 = y + HitboxHalf };
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
		foreach (Entities.IEntity entity in entities)
			if (entity.DoesCollide(hitbox))
				return true;
		return false;
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
		var seg = new Geometry.Seg {
			x1 = X,
			y1 = Y,
			x2 = X + Math.Cos(Rot) * InteractionDist,
			y2 = Y + Math.Sin(Rot) * InteractionDist,
		};
		foreach (Entities.IEntity entity in entities)
			if (entity is Entities.IInteractable interactable)
				if (entity.Intersect(seg, out var _))
					interactable.Interact();
	}
}
