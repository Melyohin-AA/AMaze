namespace AMaze;

internal class Program
{
	static void Main()
	{
		const double speed = 0.2;
		Console.Title = "AMaze";
		ConfigureViewport(120, 80);
		var game = new Game(120, 80);
		while (true)
		{
			game.Camera.Scan(game.Shapes);
			game.Camera.Buffer = game.Renderer.Render(game.Camera.Buffer);
			while (Console.KeyAvailable)
				Console.ReadKey(true);
			switch (Console.ReadKey(true).Key)
			{
				case ConsoleKey.W:
					game.Camera.Move(speed, 0.0);
					break;
				case ConsoleKey.S:
					game.Camera.Move(-speed, 0.0);
					break;
				case ConsoleKey.A:
					game.Camera.Move(-speed, Math.PI / 2);
					break;
				case ConsoleKey.D:
					game.Camera.Move(speed, Math.PI / 2);
					break;
				case ConsoleKey.Q:
					game.Camera.Rot -= game.Camera.Step * 5;
					break;
				case ConsoleKey.E:
					game.Camera.Rot += game.Camera.Step * 5;
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
}
