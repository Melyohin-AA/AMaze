using System.Diagnostics;

namespace AMaze;

internal class Program
{
	static void Main()
	{
		const int vph = 120, vpw = vph * 3 / 2;
		const int targetPeriod = 33, possibleOversleepAmount = 16;
		Console.Title = "AMaze";
		ConfigureViewport(vpw, vph);
		var game = new Game(vpw, vph);
		var sw = new Stopwatch();
		while (true)
		{
			sw.Restart();
			bool c = game.TickPlayerControls();
			game.TickLogic();
			game.TickGraphics();
			int frametime = (int)sw.ElapsedMilliseconds;
			int left = targetPeriod - frametime;
			if (left > possibleOversleepAmount)
				Thread.Sleep(left - possibleOversleepAmount);
			while (sw.ElapsedMilliseconds < targetPeriod) ;
			Console.Title = $"{c} p={sw.ElapsedMilliseconds} ft={frametime}";
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
