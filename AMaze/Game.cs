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
		var door = new Entities.Wall(new Geometry.VertSeg(11, -1, 2), altPalette: true);
		Entities = new Entities.IEnity[] {
			new Entities.Key(-3, -6, door),
			// Left and right walls
			new Entities.Wall(new Geometry.HorSeg(-10, -10, 20)),
			new Entities.Wall(new Geometry.HorSeg(-10, 10, 20)),
			// Front wall and door
			new Entities.Wall(new Geometry.VertSeg(10, -10, 9)),
			new Entities.Wall(new Geometry.HorSeg(10, -1, 1)),
			new Entities.Wall(new Geometry.VertSeg(10, -1, 2), bottom: 0.8, isGhost: true, opaque: false),
			door,
			new Entities.Wall(new Geometry.HorSeg(10, 1, 1)),
			new Entities.Wall(new Geometry.VertSeg(10, 1, 9)),
			// Invisible back wall
			new Entities.Wall(new Geometry.VertSeg(-10, -10, 20), isVisible: false),
			// Mid grid wall
			new Entities.Wall(new Geometry.VertSeg(8, -3, 6), bottom: 0.8, opaque: false),
			new Entities.Wall(new Geometry.VertSeg(8, -3, 6), top: -0.8, opaque: false),
			new Entities.Grid(new Geometry.VertSeg(8, -3, 6), top: 0.8, bottom: -0.8),
			new Entities.Wall(new Geometry.VertSeg(8, -8, 5)),
			new Entities.Wall(new Geometry.VertSeg(8, 3, 5)),
		};
	}

	public bool Tick()
	{
		const double speed = 0.1;
		ConsoleKey key = default;
		while (Console.KeyAvailable)
			key = Console.ReadKey(true).Key;
		bool moved = false;
		switch (key)
		{
			case ConsoleKey.W:
				Player.Move(speed, 0.0, Entities);
				moved = true;
				break;
			case ConsoleKey.S:
				Player.Move(-speed, 0.0, Entities);
				moved = true;
				break;
			case ConsoleKey.A:
				Player.Move(-speed, Math.PI / 2, Entities);
				moved = true;
				break;
			case ConsoleKey.D:
				Player.Move(speed, Math.PI / 2, Entities);
				moved = true;
				break;
			case ConsoleKey.Q:
				Player.Rotate(-Camera.FovStep * 5);
				break;
			case ConsoleKey.E:
				Player.Rotate(Camera.FovStep * 5);
				break;
		}
		if (moved)
			Camera.BobbingPhi += 0.3;
		else Camera.BobbingPhi = 0.0;
		Camera.Scan(Entities, Renderer.Buffer);
		Renderer.Render();
		return key != ConsoleKey.None;
	}
}
