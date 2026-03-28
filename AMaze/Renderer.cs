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

	//private long elapsedTicksV;
	//private int elapsedTicksI;

	public void Render()
	{
		//var sw = Stopwatch.StartNew();
		var breakPoints = new List<int>(64);
		for (short x = 0; x < ViewportWidth; x++)
		{
			var lines = Buffer[x];
			breakPoints.Clear();
			for (short i = 0; i < lines.Count; i++)
			{
				Line line = lines[i];
				if (line.top < ViewportHeight)
				{
					breakPoints.Add(line.top);
					if (line.bottom < ViewportHeight)
						breakPoints.Add(line.bottom);
				}
			}
			breakPoints.Sort();
			breakPoints.Add(ViewportHeight);
			short y = 0;
			for (byte j = 0; j < breakPoints.Count; j++)
			{
				int bp = breakPoints[j];
				ConsoleUpdater.Color color = SelectLineColor(lines, y);
				for (; y < bp; y++)
					ConsoleUpdater.SetCell(x, y, '#', color);
			}
		}
		//elapsedTicksV += sw.ElapsedTicks;
		//elapsedTicksI++;
		//var e1 = sw.ElapsedMilliseconds;
		ConsoleUpdater.Flush();
		//var e2 = sw.ElapsedMilliseconds - e1;
		//Console.Title = $"{elapsedTicksV / elapsedTicksI}";
		//Console.Title = $"{e1} | {e2}";
	}
	private static ConsoleUpdater.Color SelectLineColor(List<Line> lines, short y)
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
		return (lineSelected == -1) ? default : lines[lineSelected].color;
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
