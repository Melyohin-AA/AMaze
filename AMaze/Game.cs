namespace AMaze;

internal class Game
{
	public Renderer Renderer { get; }
	public Player Player { get; }
	public Camera Camera { get; }
	public Wall[] Walls { get; }

	public Game(int viewportWidth, int viewportHeight)
	{
		Renderer = new Renderer(viewportWidth, viewportHeight);
		Player = new Player(0.8);
		Camera = new Camera(Player, viewportWidth, viewportHeight, Math.PI / 2, 20, 200);
		Walls = new[] {
			// Left and right walls
			new Wall(new Geometry.HorSeg(-10, -10, 20), true, false, false),
			new Wall(new Geometry.HorSeg(-10, 10, 20), true, false, false),
			// Front wall and door
			new Wall(new Geometry.VertSeg(10, -10, 9), true, false, false),
			new Wall(new Geometry.HorSeg(10, -1, 1), true, false, false),
			new Wall(new Geometry.VertSeg(11, -1, 2), true, true, true),
			new Wall(new Geometry.HorSeg(10, 1, 1), true, false, false),
			new Wall(new Geometry.VertSeg(10, 1, 9), true, false, false),
			// Invisible back wall
			new Wall(new Geometry.VertSeg(-10, -10, 20), false, false, false),
		};
	}
}
