namespace AMaze;

internal class Player
{
	public double HitboxHalf { get; }

	public double X { get; set; }
	public double Y { get; set; }
	public double Rot { get; set; }

	public Player(double hitboxSize)
	{
		HitboxHalf = hitboxSize / 2.0;
	}

	private Geometry.Rect MakeHitbox(double x, double y)
	{
		return new Geometry.Rect { x1 = x - HitboxHalf, y1 = y - HitboxHalf, x2 = x + HitboxHalf, y2 = y + HitboxHalf };
	}

	public void Move(double step, double rotOffset, Entities.IEnity[] entities)
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
	private static bool DoesCollide(Geometry.Rect hitbox, Entities.IEnity[] entities)
	{
		foreach (Entities.IEnity enity in entities)
			if (enity.DoesCollide(hitbox))
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
}
