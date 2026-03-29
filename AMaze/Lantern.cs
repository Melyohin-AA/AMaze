namespace AMaze;

internal class Lantern
{
	private readonly Camera camera;
	private readonly double maxDepth, minDepth;
	private readonly double depthReductionSpeed, fuelingSpeed;
	private readonly double flicker;
	private readonly Random rng;

	private double currentDepth;
	private bool wasWarm;

	private bool IsWarm => currentDepth > minDepth;

	public Lantern(Camera camera, double maxDepth, double minDepth, double depthReductionSpeed, double fuelingSpeed,
		double flicker)
	{
		this.camera = camera;
		this.maxDepth = currentDepth = maxDepth;
		this.minDepth = minDepth;
		this.depthReductionSpeed = depthReductionSpeed;
		this.fuelingSpeed = fuelingSpeed;
		this.flicker = flicker;
		rng = new Random();
	}

	public void Tick()
	{
		camera.DepthCap = currentDepth = Math.Max(currentDepth - depthReductionSpeed, minDepth);
		if (IsWarm)
			camera.DepthCap += (rng.NextDouble() - 0.5) * flicker;
		if (wasWarm != IsWarm)
			PaletteManager.Set(IsWarm);
		wasWarm = IsWarm;
	}

	public void FuelUp()
	{
		currentDepth = Math.Min(currentDepth + fuelingSpeed, maxDepth);
	}
}
