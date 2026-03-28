namespace AMaze;

internal class Game
{
	private const double MaxCameraDepth = 20.0;

	private readonly List<Entities.IEntity> entitiesToSpawn;
	private readonly HashSet<Entities.IEntity> entitiesToDespawn;

	public Renderer Renderer { get; }
	public Player Player { get; }
	public Camera Camera { get; }
	public Lantern Lantern { get; }
	public List<Entities.IEntity> Entities { get; private set; }

	public Span<Entities.IEntity> EntitiesSpan => System.Runtime.InteropServices.CollectionsMarshal.AsSpan(Entities);

	public Game(int viewportWidth, int viewportHeight)
	{
		entitiesToSpawn = new List<Entities.IEntity>();
		entitiesToDespawn = new HashSet<Entities.IEntity>();
		Renderer = new Renderer(viewportWidth, viewportHeight);
		Player = new Player(0.8);
		Camera = new Camera(Player, viewportWidth, viewportHeight, Math.PI / 2, MaxCameraDepth, 2.0);
		Lantern = new Lantern(Camera, MaxCameraDepth, 2.5, 0.05);
		var key = new Entities.Key(-3, -6);
		var door = new Entities.Wall(new Geometry.VertSeg(11, -1, 2), altPalette: true);
		var keyhole = new Entities.Wall(new Geometry.VertSeg(10.9, -0.1, 0.2), top: 0.1, bottom: -0.1,
			opaque: false, vantablack: true);
		key.PickupCallback += key => {
			DespawnEntity(key);
			DespawnEntity(door);
			DespawnEntity(keyhole);
		};
		Entities = new List<Entities.IEntity> {
			// Left and right walls
			new Entities.Wall(new Geometry.HorSeg(-10, -10, 20)),
			new Entities.Wall(new Geometry.HorSeg(-10, 10, 20)),
			// Front wall and doorway
			new Entities.Wall(new Geometry.VertSeg(10, -10, 9)),
			new Entities.Wall(new Geometry.HorSeg(10, -1, 1)),
			new Entities.Wall(new Geometry.VertSeg(10, -1, 2), bottom: 0.8, isGhost: true, opaque: false),
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
			// Door and key
			door,
			key,
			keyhole,
		};
	}

	public void SpawnEntity(Entities.IEntity enity) => entitiesToSpawn.Add(enity);
	public void DespawnEntity(Entities.IEntity enity) => entitiesToDespawn.Add(enity);

	public bool TickPlayerControls()
	{
		const double speed = 0.1;
		ConsoleKey key = default;
		while (Console.KeyAvailable)
			key = Console.ReadKey(true).Key;
		bool moved = false;
		switch (key)
		{
			case ConsoleKey.W:
				Player.Move(speed, 0.0, EntitiesSpan);
				moved = true;
				break;
			case ConsoleKey.S:
				Player.Move(-speed, 0.0, EntitiesSpan);
				moved = true;
				break;
			case ConsoleKey.A:
				Player.Move(-speed, Math.PI / 2, EntitiesSpan);
				moved = true;
				break;
			case ConsoleKey.D:
				Player.Move(speed, Math.PI / 2, EntitiesSpan);
				moved = true;
				break;
			case ConsoleKey.Q:
				Player.Rotate(-Camera.FovStep * 5);
				break;
			case ConsoleKey.E:
				Player.Rotate(Camera.FovStep * 5);
				break;
			case ConsoleKey.V:
				Lantern.FuelUp();
				break;
		}
		if (moved)
			Camera.BobbingPhi += 0.3;
		else Camera.BobbingPhi = 0.0;
		return key != ConsoleKey.None;
	}

	public void TickLogic()
	{
		if ((entitiesToSpawn.Count > 0) || (entitiesToDespawn.Count > 0))
		{
			Entities.RemoveAll(entitiesToDespawn.Contains);
			Entities.AddRange(entitiesToSpawn);
			entitiesToSpawn.Clear();
			entitiesToDespawn.Clear();
		}
	}

	public void TickGraphics()
	{
		Lantern.Tick();
		Camera.Scan(EntitiesSpan, Renderer.Buffer);
		Renderer.Render();
	}
}
