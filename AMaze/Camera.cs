namespace AMaze;

internal class Camera
{
	public Player Player { get; }
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }
	public double FovStep { get; }
	public double DepthCap { get; }
	public double Perpective { get; }

	public Camera(Player player, int viewportWidth, int viewportHeight, double fov, double depthCap, double perpective)
	{
		Player = player;
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		FovStep = fov / viewportWidth;
		DepthCap = depthCap;
		Perpective = perpective;
	}

	public void Scan(Geometry.IGeom[] walls, Renderer.Line[] scanBuffer)
	{
		var ray = new Geometry.Ray { originX = Player.X, originY = Player.Y };
		for (int i = 0; i < ViewportWidth; i++)
		{
			double dir = (i - ViewportWidth / 2) * FovStep;
			ray.dirX = Math.Cos(Player.Rot + dir);
			ray.dirY = Math.Sin(Player.Rot + dir);
			double dist = GetDistToNearestPoint(ray, walls);// * (Math.Cos(dir) / 2 + 0.5);
			double value = Perpective / dist;
			double brightness = 1.0 - dist / DepthCap; //(dist - Player.HitboxHalf)
			scanBuffer[i] = Renderer.Line.FromNative(ViewportHeight, value, brightness);
		}
	}
	private double GetDistToNearestPoint(Geometry.Ray ray, Geometry.IGeom[] walls)
	{
		double minDist2 = double.MaxValue;
		foreach (Geometry.IGeom wall in walls)
		{
			if (!wall.Intersect(ray, out (double, double) intersection)) continue;
			(double px, double py) = intersection;
			double dx = Player.X - px, dy = Player.Y - py;
			double dist2 = dx * dx + dy * dy;
			if (minDist2 > dist2)
				minDist2 = dist2;
		}
		return Math.Sqrt(minDist2);
	}
}
