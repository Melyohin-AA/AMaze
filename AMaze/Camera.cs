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
		var intersections = new List<ScanIntersection>(64);
		var sight = new Geometry.Seg { x1 = Player.X, y1 = Player.Y };
		for (int i = 0; i < ViewportWidth; i++)
		{
			double dir = (i - ViewportWidth / 2) * FovStep;
			sight.x2 = sight.x1 + Math.Cos(Player.Rot + dir) * DepthCap;
			sight.y2 = sight.y1 + Math.Sin(Player.Rot + dir) * DepthCap;
			GetIntersectionsSortedByDist2UntilOpaque(sight, entities, intersections);
			scanBuffer[i].Clear();
			foreach (ScanIntersection inter in intersections)
			{
				double dist = Math.Sqrt(inter.dist2);
				inter.Screen(InnerDist, dist, bobbingY);
				double brightness = 1.0 - dist / DepthCap;
				var line = Renderer.Line.FromNative(ViewportHeight, brightness, inter);
				scanBuffer[i].Add(line);
				if (inter.opaque) break;
			}
		}
	}

	private void GetIntersectionsSortedByDist2UntilOpaque(Geometry.Seg sight, Span<Entities.IEntity> entities,
		List<ScanIntersection> intersections)
	{
		intersections.Clear();
		bool allOpaque = true;
		var minOpaqueInter = new ScanIntersection { dist2 = double.MaxValue, opaque = true };
		foreach (Entities.IEntity enity in entities)
		{
			if (!enity.Intersect(sight, out var inter)) continue;
			double dx = Player.X - inter.x, dy = Player.Y - inter.y;
			inter.dist2 = dx * dx + dy * dy;
			intersections.Add(inter);
			allOpaque = allOpaque && inter.opaque;
			if (inter.opaque && (minOpaqueInter.dist2 > inter.dist2))
				minOpaqueInter = inter;
		}
		if (allOpaque)
		{
			intersections.Clear();
			intersections.Add(minOpaqueInter);
			return;
		}
		intersections.RemoveAll(inter => inter.dist2 > minOpaqueInter.dist2);
		intersections.Sort((a, b) => a.dist2.CompareTo(b.dist2));
	}
}
