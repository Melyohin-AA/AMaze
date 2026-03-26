namespace AMaze;

internal class Renderer
{
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }

	private Line[] prevBuffer;
	public Line[] Buffer { get; private set; }

	public Renderer(int viewportWidth, int viewportHeight)
	{
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		prevBuffer = new Line[viewportWidth];
		Buffer = new Line[viewportWidth];
	}

	public void Render()
	{
		const ConsoleColor cc0 = ConsoleColor.Black;
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
					Console.MoveBufferArea(i, prevTop, 1, nextTop - prevTop, ViewportWidth, 0, '#', cc0, cc0);
					Console.MoveBufferArea(i, nextBottom, 1, prevBottom - nextBottom, ViewportWidth, 0, '#', cc0, cc0);
				}
				(var bg, var fg) = next.Colors();
				Console.MoveBufferArea(i, nextTop, 1, next.val, ViewportWidth, 0, '#', fg, bg);
			}
			else
			{
				(int top1, int top2, int bottom1, int bottom2, (var bg, var fg)) = (next.val > prev.val)
					? (nextTop, prevTop, prevTop + prev.val, nextTop + next.val, next.Colors())
					: (prevTop, nextTop, nextTop + next.val, prevTop + prev.val, (cc0, cc0));
				Console.MoveBufferArea(i, top1, 1, top2 - top1, ViewportWidth, 0, '0', fg, bg);
				Console.MoveBufferArea(i, bottom1, 1, bottom2 - bottom1, ViewportWidth, 0, '0', fg, bg);
			}
		}
		(prevBuffer, Buffer) = (Buffer, prevBuffer);
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

		public readonly (ConsoleColor, ConsoleColor) Colors()
		{
			//if (br < 128)
			//{
			//	return br switch {
			//		0 => (ConsoleColor.Black, ConsoleColor.Black),
			//		1 => (ConsoleColor.Black, ConsoleColor.DarkGray),
			//		2 => (ConsoleColor.DarkGray, ConsoleColor.DarkGray),
			//		3 => (ConsoleColor.DarkGray, ConsoleColor.Gray),
			//		4 => (ConsoleColor.Gray, ConsoleColor.Gray),
			//		5 => (ConsoleColor.Gray, ConsoleColor.White),
			//		_ => (ConsoleColor.White, ConsoleColor.White),
			//	};
			//}
			//return br switch {
			//	128 => (ConsoleColor.Black, ConsoleColor.Black),
			//	129 => (ConsoleColor.Black, ConsoleColor.Black),
			//	130 => (ConsoleColor.Black, ConsoleColor.Black),
			//	131 => (ConsoleColor.Black, ConsoleColor.DarkRed),
			//	132 => (ConsoleColor.DarkRed, ConsoleColor.DarkRed),
			//	133 => (ConsoleColor.DarkRed, ConsoleColor.Red),
			//	_ => (ConsoleColor.Red, ConsoleColor.Red),
			//};
			return ((ConsoleColor)(br >> 1), (ConsoleColor)((br + 1) >> 1));
		}

		public static bool operator ==(Line a, Line b) => (a.val == b.val) && (a.br == b.br);
		public static bool operator !=(Line a, Line b) => !(a == b);
		public override readonly bool Equals(object? obj) => (obj is Line oth) && (this == oth);
		public override readonly int GetHashCode() => val.GetHashCode() ^ br.GetHashCode();
	}
}
