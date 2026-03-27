namespace AMaze;

internal class Renderer
{
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }

	public Line[] Buffer { get; private set; }

	public Renderer(int viewportWidth, int viewportHeight)
	{
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		Buffer = new Line[viewportWidth];
	}

	public void Render()
	{
		for (int x = 0; x < ViewportWidth; x++)
		{
			Line next = Buffer[x];
			int nextTop = next.Top(ViewportHeight);
			ConsoleUpdater.Color color = next.Color();
			DrawLine(x, 0, nextTop, '#', default);
			DrawLine(x, nextTop, next.val, '#', color);
			int bottom = nextTop + next.val;
			DrawLine(x, bottom, ViewportHeight - bottom, '#', default);
		}
		ConsoleUpdater.Flush();
	}
	private static void DrawLine(int left, int top, int height, char ch, ConsoleUpdater.Color color)
	{
		for (int i = 0; i < height; i++)
			ConsoleUpdater.SetCell((short)left, (short)(top + i), ch, color);
	}

	public struct Line
	{
		public const byte MaxBrightness = 14;

		public int val;
		public byte br;
		public bool altPalette;

		public static Line FromNative(int viewportHeight, double value, double brightness, bool altPalette)
		{
			brightness = Math.Min(1.0, Math.Max(0.0, brightness));
			return new Line {
				val = (int)Math.Min(value, viewportHeight),
				br = (byte)Math.Round(brightness * MaxBrightness),
				altPalette = altPalette,
			};
		}

		public readonly int Top(int viewportHeight)
		{
			int top = (viewportHeight - val) / 2;
			return (top >= 0) ? top : 0;
		}

		public readonly ConsoleUpdater.Color Color()
		{
			if (br > MaxBrightness)
				throw new ArgumentOutOfRangeException(nameof(br));
			int paletteBr = altPalette ? br + MaxBrightness + 2 : br;
			int bg = paletteBr >> 1, fg = (paletteBr + 1) >> 1;
			return new ConsoleUpdater.Color((short)((bg << 4) | fg));
		}
	}
}
