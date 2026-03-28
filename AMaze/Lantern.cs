namespace AMaze;

internal class Lantern
{
	public Camera Camera { get; }
	public double MaxDepth { get; }
	public double MinDepth { get; }
	public double DepthReductionSpeed { get; }

	public Lantern(Camera camera, double maxDepth, double minDepth, double depthReductionSpeed)
	{
		Camera = camera;
		MaxDepth = maxDepth;
		MinDepth = minDepth;
		DepthReductionSpeed = depthReductionSpeed;
	}

	public void Tick()
	{
		Camera.DepthCap = Math.Max(Camera.DepthCap - DepthReductionSpeed, MinDepth);
	}

	public void FuelUp()
	{
		Camera.DepthCap = Math.Min(Camera.DepthCap + 1.0, MaxDepth);
	}
}
