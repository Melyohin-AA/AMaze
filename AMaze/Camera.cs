namespace AMaze;

internal class Camera
{
	public Player Player { get; }
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }
	public double FovStep { get; }
	public double DepthCap { get; }
	public double InnerDist { get; }

	public Camera(Player player, int viewportWidth, int viewportHeight, double fov, double depthCap, double innerDist)
	{
		Player = player;
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		FovStep = fov / viewportWidth;
		DepthCap = depthCap;
		InnerDist = innerDist;
	}

	public void Scan(Entities.IEnity[] entities, List<Renderer.Line>[] scanBuffer)
	{
		var intersections = new List<(double, ScanIntersectionExtra)>(64);
		var ray = new Geometry.Ray { originX = Player.X, originY = Player.Y };
		for (int i = 0; i < ViewportWidth; i++)
		{
			double dir = (i - ViewportWidth / 2) * FovStep;
			ray.dirX = Math.Cos(Player.Rot + dir);
			ray.dirY = Math.Sin(Player.Rot + dir);
			GetIntersectionsSortedByDist2UntilOpaque(ray, entities, intersections);
			scanBuffer[i].Clear();
			foreach ((double dist2, ScanIntersectionExtra extra) in intersections)
			{
				double dist = Math.Sqrt(dist2);
				double screenedTop = extra.top * InnerDist / dist;
				double screenedBottom = extra.bottom * InnerDist / dist;
				double brightness = 1.0 - dist / DepthCap;
				var line = Renderer.Line.FromNative(ViewportHeight, screenedTop, screenedBottom, brightness, extra);
				scanBuffer[i].Add(line);
				if (extra.opaque) break;
			}
		}
	}

	private void GetIntersectionsSortedByDist2UntilOpaque(Geometry.Ray ray, Entities.IEnity[] entities,
		List<(double, ScanIntersectionExtra)> intersections)
	{
		intersections.Clear();
		bool allOpaque = true;
		double minOpaqueDist2 = double.MaxValue;
		ScanIntersectionExtra minExtra = default;
		foreach (Entities.IEnity enity in entities)
		{
			if (!enity.Intersect(ray, out var intersection)) continue;
			((double px, double py), ScanIntersectionExtra extra) = intersection;
			double dx = Player.X - px, dy = Player.Y - py;
			double dist2 = dx * dx + dy * dy;
			intersections.Add((dist2, extra));
			allOpaque = allOpaque && extra.opaque;
			if (extra.opaque && (minOpaqueDist2 > dist2))
			{
				minOpaqueDist2 = dist2;
				minExtra = extra;
			}
		}
		if (allOpaque)
		{
			intersections.Clear();
			intersections.Add((minOpaqueDist2, minExtra));
			return;
		}
		intersections.RemoveAll(item => item.Item1 > minOpaqueDist2);
		intersections.Sort((a, b) => a.Item1.CompareTo(b.Item1));
	}
}
