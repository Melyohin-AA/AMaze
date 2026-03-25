namespace AMaze;

internal class Camera
{
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }
	public double FovStep { get; }
	public double DepthCap { get; }

	public double X { get; set; }
	public double Y { get; set; }
	public double Rot { get; set; }
	public (int, byte)[] Buffer { get; set; }

	public Camera(int viewportWidth, int viewportHeight, double fov, double depthCap)
	{
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		FovStep = fov / viewportWidth;
		DepthCap = depthCap;
		Buffer = new (int, byte)[ViewportWidth];
	}

	public void Move(double step, double rotOffset, Geometry.IGeom[] walls)
	{
		double dx = Math.Cos(Rot + rotOffset) * step;
		double dy = Math.Sin(Rot + rotOffset) * step;
		var vxy = new Geometry.Seg { x1 = X, y1 = Y, x2 = X + dx, y2 = Y + dy };
		var vx = new Geometry.Seg { x1 = X, y1 = Y, x2 = X + dx, y2 = Y };
		var vy = new Geometry.Seg { x1 = X, y1 = Y, x2 = X, y2 = Y + dy };
		Span<Geometry.Seg> vectors = stackalloc Geometry.Seg[3];
		vectors[0] = vxy;
		(vectors[1], vectors[2]) = (Math.Abs(dx) > Math.Abs(dy)) ? (vx, vy) : (vy, vx);
		foreach (Geometry.Seg vector in vectors)
		{
			if (DoesCollide(vector, walls)) continue;
			X = vector.x2;
			Y = vector.y2;
			return;
		}
	}
	private static bool DoesCollide(Geometry.Seg vector, Geometry.IGeom[] walls)
	{
		foreach (Geometry.IGeom wall in walls)
			if (wall.Intersect(vector, out var _))
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

	public void Scan(Geometry.IGeom[] walls)
	{
		var seg = new Geometry.Seg { x1 = X, y1 = Y };
		for (int i = 0; i < ViewportWidth; i++)
		{
			double rot = Rot + (i - ViewportWidth / 2) * FovStep;
			seg.x2 = X + Math.Cos(rot) * DepthCap;
			seg.y2 = Y + Math.Sin(rot) * DepthCap;
			double dist = GetDistToNearestPoint(seg, walls);
			int value = (int)Math.Min(300 / dist, ViewportHeight);
			byte brightness = (dist < DepthCap) ? (byte)Math.Round((1.0 - dist / DepthCap) * 7) : (byte)0;
			Buffer[i] = (value, brightness);
		}
	}
	private double GetDistToNearestPoint(Geometry.Seg seg, Geometry.IGeom[] walls)
	{
		double minDist2 = double.MaxValue;
		foreach (Geometry.IGeom wall in walls)
		{
			if (!wall.Intersect(seg, out (double, double) intersection)) continue;
			(double px, double py) = intersection;
			double dx = X - px, dy = Y - py;
			double dist2 = dx * dx + dy * dy;
			if (minDist2 > dist2)
				minDist2 = dist2;
		}
		return Math.Sqrt(minDist2);
	}
}
