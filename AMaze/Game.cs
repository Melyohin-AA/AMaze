namespace AMaze;

internal class Game
{
	public Renderer Renderer { get; }
	public Player Player { get; }
	public Camera Camera { get; }
	public Geometry.IGeom[] Walls { get; }

	public Game(int viewportWidth, int viewportHeight)
	{
		Renderer = new Renderer(viewportWidth, viewportHeight);
		Player = new Player(0.8);
		Camera = new Camera(Player, viewportWidth, viewportHeight, Math.PI / 2, 40, 200);
		Walls = new Geometry.IGeom[] {
			new Geometry.HorSeg(-10, -10, 20),
			new Geometry.VertSeg(10, -10, 20),
			new Geometry.HorSeg(-10, 10, 20),
		};
	}
}
