using System.Diagnostics;

namespace AMaze;

internal class Renderer
{
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }

	public List<Line>[] Buffer { get; private set; }

	public Renderer(int viewportWidth, int viewportHeight)
	{
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		Buffer = new List<Line>[viewportWidth];
		for (int i = 0; i < ViewportWidth; i++)
			Buffer[i] = new List<Line>();
	}

	public void Render()
	{
		//var sw = Stopwatch.StartNew();
		for (short x = 0; x < ViewportWidth; x++)
		{
			var lines = Buffer[x];
			for (short y = 0; y < ViewportHeight; y++)
			{
				int lineSelected = -1;
				for (short i = 0; i < lines.Count; i++)
				{
					if ((y >= lines[i].top) && (y < lines[i].bottom))
					{
						lineSelected = i;
						break;
					}
				}
				ConsoleUpdater.Color color = (lineSelected == -1) ? default : lines[lineSelected].color;
				ConsoleUpdater.SetCell(x, y, '#', color);
			}
		}
		//var e1 = sw.Elapsed;
		ConsoleUpdater.Flush();
		//var e2 = sw.Elapsed - e1;
		//Console.Title = $"{e1.Microseconds} | {e2.Microseconds}";
	}

	public struct Line
	{
		public const byte MaxBrightness = 14;

		public int top, bottom;
		public ConsoleUpdater.Color color;

		public static Line FromNative(int viewportHeight,
			double top, double bottom, double brightness, ScanIntersectionExtra extra)
		{
			brightness = Math.Min(1.0, Math.Max(0.0, brightness));
			return new Line {
				top = ProjectNormHeight(top, viewportHeight),
				bottom = ProjectNormHeight(bottom, viewportHeight),
				color = GetColor((byte)Math.Round(brightness * MaxBrightness), extra.altPalette),
			};
		}
		private static int ProjectNormHeight(double h, int viewportHeight)
		{
			double renormedHeight = 1.0 - (h + 1.0) / 2;
			return (int)(renormedHeight * viewportHeight);
		}
		private static ConsoleUpdater.Color GetColor(byte br, bool altPalette)
		{
			if (br > MaxBrightness)
				throw new ArgumentOutOfRangeException(nameof(br));
			int paletteBr = altPalette ? br + MaxBrightness + 2 : br;
			int bg = paletteBr >> 1, fg = (paletteBr + 1) >> 1;
			return new ConsoleUpdater.Color((short)((bg << 4) | fg));
		}
	}
}
