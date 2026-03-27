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

	public void Scan(Wall[] walls, Renderer.Line[] scanBuffer)
	{
		var ray = new Geometry.Ray { originX = Player.X, originY = Player.Y };
		for (int i = 0; i < ViewportWidth; i++)
		{
			double dir = (i - ViewportWidth / 2) * FovStep;
			ray.dirX = Math.Cos(Player.Rot + dir);
			ray.dirY = Math.Sin(Player.Rot + dir);
			(double dist, Wall? nearestWall) = GetDistToNearestPoint(ray, walls);// * (Math.Cos(dir) / 2 + 0.5);
			double value = Perpective / dist;
			double brightness = 1.0 - dist / DepthCap; //(dist - Player.HitboxHalf)
			bool altPalette = nearestWall?.AltPalette ?? false;
			scanBuffer[i] = Renderer.Line.FromNative(ViewportHeight, value, brightness, altPalette);
		}
	}
	private (double, Wall?) GetDistToNearestPoint(Geometry.Ray ray, Wall[] walls)
	{
		double minDist2 = double.MaxValue;
		Wall? nearestWall = null;
		foreach (Wall wall in walls)
		{
			if (wall.IsVisible && wall.Geom.Intersect(ray, out (double, double) intersection))
			{
				(double px, double py) = intersection;
				double dx = Player.X - px, dy = Player.Y - py;
				double dist2 = dx * dx + dy * dy;
				if (minDist2 > dist2)
				{
					minDist2 = dist2;
					nearestWall = wall;
				}
			}
		}
		return (Math.Sqrt(minDist2), nearestWall);
	}
}
