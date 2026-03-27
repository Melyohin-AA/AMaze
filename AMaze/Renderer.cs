namespace AMaze;

internal class Renderer
{
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }

	private Line[] prevBuffer;
	public Line[] Buffer { get; private set; }

	private readonly ConsoleUpdater.Cell[] cuCells;
	private int cuCellI;

	public Renderer(int viewportWidth, int viewportHeight)
	{
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		prevBuffer = new Line[viewportWidth];
		Buffer = new Line[viewportWidth];
		cuCells = new ConsoleUpdater.Cell[viewportWidth * viewportHeight];
	}

	public void Render()
	{
		cuCellI = 0;
		for (int i = 0; i < ViewportWidth; i++)
		{
			Line prev = prevBuffer[i], next = Buffer[i];
			if (Buffer[i] == prevBuffer[i]) continue;
			int nextTop = next.Top(ViewportHeight), prevTop = prev.Top(ViewportHeight);
			if (next.br != prev.br)
			{
				if (next.val < prev.val)
				{
					int prevBottom = prevTop + prev.val, nextBottom = nextTop + next.val;
					AddCuCellLine(i, prevTop, nextTop - prevTop, '#', default);
					AddCuCellLine(i, nextBottom, prevBottom - nextBottom, '#', default);
				}
				var color = next.Color();
				AddCuCellLine(i, nextTop, next.val, '#', color);
			}
			else
			{
				(int top1, int top2, int bottom1, int bottom2, var color) = (next.val > prev.val)
					? (nextTop, prevTop, prevTop + prev.val, nextTop + next.val, next.Color())
					: (prevTop, nextTop, nextTop + next.val, prevTop + prev.val, default);
				AddCuCellLine(i, top1, top2 - top1, '0', color);
				AddCuCellLine(i, bottom1, bottom2 - bottom1, '0', color);
			}
		}
		(prevBuffer, Buffer) = (Buffer, prevBuffer);
		ConsoleUpdater.Update(cuCells.AsSpan(0, cuCellI));
	}
	private void AddCuCellLine(int left, int top, int height, char ch, ConsoleUpdater.Color color)
	{
		for (int i = 0; i < height; i++)
			cuCells[cuCellI++] = new ConsoleUpdater.Cell((short)left, (short)(top + i), ch, color);
	}

	public struct Line
	{
		public const byte MaxBrightness = 30;

		public int val;
		public byte br;

		public static Line FromNative(int viewportHeight, double value, double brightness)
		{
			brightness = Math.Min(1.0, Math.Max(0.0, brightness));
			return new Line {
				val = (int)Math.Min(value, viewportHeight),
				br = (byte)Math.Round(brightness * MaxBrightness),
			};
		}

		public readonly int Top(int viewportHeight)
		{
			int top = (viewportHeight - val) / 2;
			return (top >= 0) ? top : 0;
		}

		public readonly ConsoleUpdater.Color Color()
		{
			if (br > 30)
				throw new ArgumentOutOfRangeException(nameof(br));
			int bg = br >> 1, fg = (br + 1) >> 1;
			return new ConsoleUpdater.Color((short)((bg << 4) | fg));
		}

		public static bool operator ==(Line a, Line b) => (a.val == b.val) && (a.br == b.br);
		public static bool operator !=(Line a, Line b) => !(a == b);
		public override readonly bool Equals(object? obj) => (obj is Line oth) && (this == oth);
		public override readonly int GetHashCode() => val.GetHashCode() ^ br.GetHashCode();
	}
}
