namespace AMaze;

internal class Game
{
	public Renderer Renderer { get; }
	public Player Player { get; }
	public Camera Camera { get; }
	public Entities.IEnity[] Entities { get; }

	public Game(int viewportWidth, int viewportHeight)
	{
		Renderer = new Renderer(viewportWidth, viewportHeight);
		Player = new Player(0.8);
		Camera = new Camera(Player, viewportWidth, viewportHeight, Math.PI / 2, 20, 2.0);
		Entities = new[] {
			// Left and right walls
			new Entities.Wall(new Geometry.HorSeg(-10, -10, 20)),
			new Entities.Wall(new Geometry.HorSeg(-10, 10, 20)),
			// Front wall and door
			new Entities.Wall(new Geometry.VertSeg(10, -10, 9)),
			new Entities.Wall(new Geometry.HorSeg(10, -1, 1)),
			new Entities.Wall(new Geometry.VertSeg(10, -1, 2), bottom: 0.8, isGhost: true, opaque: false),
			new Entities.Wall(new Geometry.VertSeg(11, -1, 2), isGhost: true, altPalette: true),
			new Entities.Wall(new Geometry.HorSeg(10, 1, 1)),
			new Entities.Wall(new Geometry.VertSeg(10, 1, 9)),
			// Invisible back wall
			new Entities.Wall(new Geometry.VertSeg(-10, -10, 20), isVisible: false),
		};
	}
}
