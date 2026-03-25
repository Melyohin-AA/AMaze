namespace AMaze;

internal class Game
{
	public Renderer Renderer { get; }
	public Camera Camera { get; }
	public Geometry.IGeom[] Walls { get; }

	public Game(int viewportWidth, int viewportHeight)
	{
		Renderer = new Renderer(viewportWidth, viewportHeight);
		Camera = new Camera(viewportWidth, viewportHeight, Math.PI / 2, 20);
		Walls = new Geometry.IGeom[] {
			new Geometry.HorSeg(-10, -10, 20),
			new Geometry.VertSeg(10, -10, 20),
			new Geometry.HorSeg(-10, 10, 20),
		};
	}
}
