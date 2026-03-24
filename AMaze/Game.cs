namespace AMaze;

internal class Game
{
	public Renderer Renderer { get; }
	public Camera Camera { get; }
	public Shapes.IShape[] Shapes { get; }

	public Game(int viewportWidth, int viewportHeight)
	{
		Renderer = new Renderer(viewportWidth, viewportHeight);
		Camera = new Camera(viewportWidth, viewportHeight, Math.PI / 2 / viewportWidth, 20.0);
		Shapes = new Shapes.IShape[] {
			new Shapes.Rect(-10, -10, 10, -8),
			new Shapes.Rect(8, -10, 10, 10),
			new Shapes.Rect(-10, 8, 10, 10),
			new Shapes.Circle(-9, -9, 3),
			new Shapes.Circle(-9, 9, 3),
		};
	}
}
