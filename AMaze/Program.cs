using System.Drawing;

namespace AMaze;

internal class Program
{
	static void Main()
	{
		const double speed = 0.2;
		Console.Title = "AMaze";
		ConfigureViewport(120, 80);
		ApplyPalette();
		var game = new Game(120, 80);
		while (true)
		{
			game.Camera.Scan(game.Walls, game.Renderer.Buffer);
			game.Renderer.Render();
			while (Console.KeyAvailable)
				Console.ReadKey(true);
			switch (Console.ReadKey(true).Key)
			{
				case ConsoleKey.W:
					game.Player.Move(speed, 0.0, game.Walls);
					break;
				case ConsoleKey.S:
					game.Player.Move(-speed, 0.0, game.Walls);
					break;
				case ConsoleKey.A:
					game.Player.Move(-speed, Math.PI / 2, game.Walls);
					break;
				case ConsoleKey.D:
					game.Player.Move(speed, Math.PI / 2, game.Walls);
					break;
				case ConsoleKey.Q:
					game.Player.Rotate(-game.Camera.FovStep * 5);
					break;
				case ConsoleKey.E:
					game.Player.Rotate(game.Camera.FovStep * 5);
					break;
			}
		}
	}

	private static void ConfigureViewport(int w, int h)
	{
		Console.BackgroundColor = ConsoleColor.Black;
		Console.ForegroundColor = ConsoleColor.White;
		Console.CursorVisible = true;
		Console.SetWindowSize(80, 1);
		Console.SetBufferSize(w, h);
		while (true)
		{
			try
			{
				Console.SetWindowSize(w, h);
				break;
			}
			catch (ArgumentOutOfRangeException)
			{
				Console.Write("Font is too big. Select a tinier one and press any key");
				Console.ReadKey(true);
				Console.Clear();
			}
		}
		Console.Clear();
		Console.CursorVisible = false;
	}

	private static void ApplyPalette(byte light = 17)
	{
		if (light > 17)
			throw new ArgumentOutOfRangeException(nameof(light));
		var colors = new Color[16];
		for (int i = 0; i < 16; i++)
		{
			int brightness = i * light;
			colors[i] = Color.FromArgb(brightness, brightness, brightness);
		}
		Recolorer.Recolor(colors);
	}
}
