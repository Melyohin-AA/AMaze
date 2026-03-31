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
				lines[i].AddHeights(ViewportHeight, breakPoints);
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
		for (int i = 0; i < lines.Count; i++)
		{
			Line line = lines[i];
			if (y >= line.top)
			{
				if (y < line.headSec.bottom)
					return line.headSec.color;
				if (line.extraSecs != null)
					for (int j = 0; j < line.extraSecs.Length; j++)
						if (y < line.extraSecs[j].bottom)
							return line.extraSecs[j].color;
			}
		}
		return default;
	}

	private static int ProjectNormHeight(double h, int viewportHeight)
	{
		double renormedHeight = 1.0 - (h + 1.0) / 2;
		return (int)(renormedHeight * viewportHeight);
	}

	public struct Line
	{
		public int top;
		public LineSection headSec;
		public LineSection[]? extraSecs;

		public static Line FromNative(int viewportHeight, double brightness, ScanIntersection inter)
		{
			brightness = Math.Clamp(brightness, 0.0, 1.0);
			var line = new Line {
				top = ProjectNormHeight(inter.top, viewportHeight),
				headSec = LineSection.FromNative(viewportHeight, brightness, inter.headSec),
			};
			if (inter.extraSecs != null)
			{
				line.extraSecs = new LineSection[inter.extraSecs.Length];
				for (int i = 0; i < line.extraSecs.Length; i++)
					line.extraSecs[i] = LineSection.FromNative(viewportHeight, brightness, inter.extraSecs[i]);
			}
			return line;
		}

		public void AddHeights(int viewportHeight, List<int> heights)
		{
			if (top < viewportHeight)
			{
				heights.Add(top);
				headSec.AddHeights(viewportHeight, heights);
				if (extraSecs != null)
					for (int i = 0; i < extraSecs.Length; i++)
						extraSecs[i].AddHeights(viewportHeight, heights);
			}
		}
	}

	public struct LineSection
	{
		public const byte MaxBrightness = 14;

		public int bottom;
		public ConsoleUpdater.Color color;

		public static LineSection FromNative(
			int viewportHeight, double brightness, ScanIntersectionSection interSec)
		{
			return new LineSection {
				bottom = ProjectNormHeight(interSec.bottom, viewportHeight),
				color = GetColor((byte)Math.Round(brightness * MaxBrightness), interSec),
			};
		}
		private static ConsoleUpdater.Color GetColor(byte br, ScanIntersectionSection interSec)
		{
			if (interSec.vantablack)
				return default;
			if (br > MaxBrightness)
				throw new ArgumentOutOfRangeException(nameof(br));
			int paletteBr = interSec.altPalette ? br + MaxBrightness + 2 : br;
			int bg = paletteBr >> 1, fg = (paletteBr + 1) >> 1;
			return new ConsoleUpdater.Color((short)((bg << 4) | fg));
		}

		public void AddHeights(int viewportHeight, List<int> heights)
		{
			if (bottom < viewportHeight)
				heights.Add(bottom);
		}
	}
}
