namespace AMaze;

internal struct Ray
{
	public double originX;
	public double originY;
	public double dirX;
	public double dirY;

	public Ray(double originX, double originY, double dirX, double dirY)
	{
		this.originX = originX;
		this.originY = originY;
		this.dirX = dirX;
		this.dirY = dirY;
	}
}
