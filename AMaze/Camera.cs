namespace AMaze;

internal class Camera
{
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }
	public double Step { get; }
	public double DepthCap { get; }

	public double X { get; set; }
	public double Y { get; set; }
	public double Rot { get; set; }
	public (int, byte)[] Buffer { get; set; }

	public Camera(int viewportWidth, int viewportHeight, double step, double depthCap)
	{
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		Step = step;
		DepthCap = depthCap;
		Buffer = new (int, byte)[ViewportWidth];
	}

	public void Move(double speed, double rotOffset)
	{
		X += Math.Cos(Rot + rotOffset) * speed;
		Y += Math.Sin(Rot + rotOffset) * speed;
	}

	public void Scan(Shapes.IShape[] shapes)
	{
		Ray ray = new Ray { originX = X, originY = Y };
		var intersections = new List<(double, double)>();
		for (int i = 0; i < ViewportWidth; i++)
		{
			double rot = Rot + (i - ViewportWidth / 2) * Step;
			ray.dirX = Math.Cos(rot);
			ray.dirY = Math.Sin(rot);
			intersections.Clear();
			foreach (Shapes.IShape shape in shapes)
				shape.Intersect(ray, intersections);
			double dist = GetDistToNearestPoint(intersections);
			int value = (int)Math.Min(300 / dist, ViewportHeight);
			byte brightness = (dist < DepthCap) ? (byte)Math.Round((1.0 - dist / DepthCap) * 7) : (byte)0;
			Buffer[i] = (value, brightness);
		}
	}

	private double GetDistToNearestPoint(IEnumerable<(double, double)> points)
	{
		double minDist2 = double.MaxValue;
		foreach ((double px, double py) in points)
		{
			double dx = X - px, dy = Y - py;
			double dist2 = dx * dx + dy * dy;
			if (minDist2 > dist2)
				minDist2 = dist2;
		}
		return Math.Sqrt(minDist2);
	}
}
