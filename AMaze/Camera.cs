namespace AMaze;

internal class Camera
{
	public Player Player { get; }
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }
	public double FovStep { get; }
	public double DepthCap { get; set; }
	public double InnerDist { get; }

	public double BobbingPhi { get; set; }

	public Camera(Player player, int viewportWidth, int viewportHeight, double fov, double depthCap, double innerDist)
	{
		Player = player;
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		FovStep = fov / viewportWidth;
		DepthCap = depthCap;
		InnerDist = innerDist;
	}

	public void Scan(Span<Entities.IEntity> entities, List<Renderer.Line>[] scanBuffer)
	{
		double bobbingY = Math.Sin(BobbingPhi) * 0.05;
		var intersections = new List<(double, ScanIntersectionExtra)>(64);
		var sight = new Geometry.Seg { x1 = Player.X, y1 = Player.Y };
		for (int i = 0; i < ViewportWidth; i++)
		{
			double dir = (i - ViewportWidth / 2) * FovStep;
			sight.x2 = sight.x1 + Math.Cos(Player.Rot + dir) * DepthCap;
			sight.y2 = sight.y1 + Math.Sin(Player.Rot + dir) * DepthCap;
			GetIntersectionsSortedByDist2UntilOpaque(sight, entities, intersections);
			scanBuffer[i].Clear();
			foreach ((double dist2, ScanIntersectionExtra extra) in intersections)
			{
				double dist = Math.Sqrt(dist2);
				double screenedTop = extra.top * InnerDist / dist + bobbingY;
				double screenedBottom = extra.bottom * InnerDist / dist + bobbingY;
				double brightness = 1.0 - dist / DepthCap;
				var line = Renderer.Line.FromNative(ViewportHeight, screenedTop, screenedBottom, brightness, extra);
				scanBuffer[i].Add(line);
				if (extra.opaque) break;
			}
		}
	}

	private void GetIntersectionsSortedByDist2UntilOpaque(Geometry.Seg sight, Span<Entities.IEntity> entities,
		List<(double, ScanIntersectionExtra)> intersections)
	{
		intersections.Clear();
		bool allOpaque = true;
		double minOpaqueDist2 = double.MaxValue;
		ScanIntersectionExtra minExtra = default;
		foreach (Entities.IEntity enity in entities)
		{
			if (!enity.Intersect(sight, out var intersection)) continue;
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
