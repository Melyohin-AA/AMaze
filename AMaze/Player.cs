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

	public void Move(double step, double rotOffset, Wall[] walls)
	{
		double dx = Math.Cos(Rot + rotOffset) * step;
		double dy = Math.Sin(Rot + rotOffset) * step;
		(double, double) vxy = (dx, dy), vx = (dx, 0.0), vy = (0.0, dy);
		Span<(double, double)> vectors = stackalloc (double, double)[3];
		vectors[0] = vxy;
		(vectors[1], vectors[2]) = (Math.Abs(dx) > Math.Abs(dy)) ? (vx, vy) : (vy, vx);
		foreach ((double vdx, double vdy) in vectors)
		{
			if (DoesCollide(MakeHitbox(X + vdx, Y + vdy), walls)) continue;
			X += vdx;
			Y += vdy;
			return;
		}
	}
	private static bool DoesCollide(Geometry.Rect hitbox, Wall[] walls)
	{
		foreach (Wall wall in walls)
			if (!wall.IsGhost && wall.Geom.DoesIntersect(hitbox))
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
