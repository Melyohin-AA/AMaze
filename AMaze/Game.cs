using AMaze.Entities;
using AMaze.Geometry;

namespace AMaze;

internal class Game : IDisposable
{
	private const double MaxCameraDepth = 20.0;

	private readonly List<Entities.IEntity> entitiesToSpawn;
	private readonly HashSet<Entities.IEntity> entitiesToDespawn;

	public Renderer Renderer { get; }
	public Keyboard Keyboard { get; }
	public Sound.StereoPlayer StereoPlayer { get; }
	public Player Player { get; }
	public Camera Camera { get; }
	public Lantern Lantern { get; }
	public List<IEntity> Entities { get; private set; }

	public Span<IEntity> EntitiesSpan => System.Runtime.InteropServices.CollectionsMarshal.AsSpan(Entities);

	public Game(int viewportWidth, int viewportHeight)
	{
		entitiesToSpawn = new List<IEntity>();
		entitiesToDespawn = new HashSet<IEntity>();
		Renderer = new Renderer(viewportWidth, viewportHeight);
		Keyboard = new Keyboard([
			ConsoleKey.V,
			ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D,
			ConsoleKey.Q, ConsoleKey.E,
		]);
		StereoPlayer = new Sound.StereoPlayer();
		Player = new Player(0.8);
		Camera = new Camera(Player, viewportWidth, viewportHeight, Math.PI / 2, MaxCameraDepth, 2.0);
		Lantern = new Lantern(Camera, MaxCameraDepth, 2.5, 0.02, 0.5, 0.1);
		var key = new Key(-3, -6);
		var door = new Door((11, 0), 2, Orientation.NegX);
		key.PickupCallback += key => {
			DespawnEntity(key);
			DespawnEntity(door);
		};
		Entities = new List<IEntity> {
			// Left and right walls
			new Wall(new HorSeg(-10, -10, 20)),
			new Wall(new HorSeg(-10, 10, 20)),
			// Front wall and doorway
			new Wall(new VertSeg(10, -10, 9)),
			new Wall(new HorSeg(10, -1, 1)),
			new Wall(new VertSeg(10, -1, 2), bottom: 0.8, isGhost: true, opaque: false),
			new Wall(new HorSeg(10, 1, 1)),
			new Wall(new VertSeg(10, 1, 9)),
			// Invisible back wall
			new Wall(new VertSeg(-10, -10, 20), isVisible: false),
			// Mid grid wall
			new Wall(new VertSeg(8, -3, 6), bottom: 0.8, opaque: false),
			new Wall(new VertSeg(8, -3, 6), top: -0.8, opaque: false),
			new Grid(new VertSeg(8, -3, 6), top: 0.8, bottom: -0.8),
			new Wall(new VertSeg(8, -8, 5)),
			new Wall(new VertSeg(8, 3, 5)),
			// Door and key
			door,
			key,
			new Pillar(-3, -6, -0.3),
		};
	}

	~Game() => Dispose();
	public void Dispose()
	{
		StereoPlayer.Dispose();
	}

	public void SpawnEntity(IEntity enity) => entitiesToSpawn.Add(enity);
	public void DespawnEntity(IEntity enity) => entitiesToDespawn.Add(enity);

	public bool TickPlayerControls()
	{
		const double speed = 0.1;
		ConsoleKey key = Keyboard.ReadKey();
		bool moved = false, startingWithLeftLeg = false;
		switch (key)
		{
			case ConsoleKey.W:
				Player.Move(speed, 0.0, EntitiesSpan);
				moved = true;
				startingWithLeftLeg = true;
				break;
			case ConsoleKey.S:
				Player.Move(-speed, 0.0, EntitiesSpan);
				moved = true;
				startingWithLeftLeg = false;
				break;
			case ConsoleKey.A:
				Player.Move(-speed, Math.PI / 2, EntitiesSpan);
				moved = true;
				startingWithLeftLeg = true;
				break;
			case ConsoleKey.D:
				Player.Move(speed, Math.PI / 2, EntitiesSpan);
				moved = true;
				startingWithLeftLeg = false;
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
			BobCamera(startingWithLeftLeg);
		else Camera.BobbingPhi = 0.0;
		return key != ConsoleKey.None;
	}
	private void BobCamera(bool startingWithLeftLeg)
	{
		double stepSine0 = CalcStepSine(startingWithLeftLeg);
		Camera.BobbingPhi += 0.3;
		double stepSine1 = CalcStepSine(startingWithLeftLeg);
		if ((stepSine0 > 0.0) != (stepSine1 > 0.0))
		{
			(float lv, float rv) = (stepSine1 > 0.0) ? (0.4f, 0.5f) : (0.5f, 0.4f);
			StereoPlayer.PanAndPlay("step", lv, rv);
		}
	}
	private double CalcStepSine(bool startingWithLeftLeg)
	{
		double x = Camera.BobbingPhi / 2.0 + 1.2;
		return Math.Cos(startingWithLeftLeg ? x : x + Math.PI);
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
