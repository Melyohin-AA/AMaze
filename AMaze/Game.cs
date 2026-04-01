using AMaze.Entities;
using AMaze.Geometry;

namespace AMaze;

internal class Game : IDisposable
{
	private const double MaxCameraDepth = 20.0;

	private readonly List<IEntity> entitiesToSpawn = new List<IEntity>();
	private readonly HashSet<IEntity> entitiesToDespawn = new HashSet<IEntity>();

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
		Renderer = new Renderer(viewportWidth, viewportHeight);
		Keyboard = new Keyboard([
			ConsoleKey.C, ConsoleKey.V,
			ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D,
			ConsoleKey.Q, ConsoleKey.E,
		]);
		StereoPlayer = new Sound.StereoPlayer();
		Player = new Player(0.8, 2.0);
		Camera = new Camera(Player, viewportWidth, viewportHeight, Math.PI / 2, MaxCameraDepth, 2.0);
		Lantern = new Lantern(Camera, MaxCameraDepth, 2.5, 0.02, 0.5, 0.1);
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
			// Door and keys
			new Door(this, (11, 0), 2, Orientation.NegX, KeyShape.Diamond),
			new Pillar(-6, -3, -0.3),
			new Key(this, KeyShape.Diamond, -6, -3),
			new Pillar(-6, 0, -0.3),
			new Key(this, KeyShape.Eye, -6, 0),
			new Pillar(-6, 3, -0.3),
			new Key(this, KeyShape.H, -6, 3),
		};
	}

	~Game() => Dispose();
	public void Dispose()
	{
		StereoPlayer.Dispose();
	}

	public void SpawnEntity(IEntity entity) => entitiesToSpawn.Add(entity);
	public void DespawnEntity(IEntity entity) => entitiesToDespawn.Add(entity);

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
			case ConsoleKey.C:
				Player.Interact(EntitiesSpan);
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
		foreach (IEntity entity in Entities)
			if (entity is IDynamic dynamic)
				dynamic.Tick();
		if (entitiesToDespawn.Count > 0)
		{
			Entities.RemoveAll(entitiesToDespawn.Contains);
			entitiesToDespawn.Clear();
		}
		if (entitiesToSpawn.Count > 0)
		{
			Entities.AddRange(entitiesToSpawn);
			entitiesToSpawn.Clear();
		}
	}

	public void TickGraphics()
	{
		Lantern.Tick();
		Camera.Scan(EntitiesSpan, Renderer.Buffer);
		Renderer.Render(Player.CanInteract(EntitiesSpan));
	}
}
