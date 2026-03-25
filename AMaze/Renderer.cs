namespace AMaze;

internal class Renderer
{
	public int ViewportWidth { get; }
	public int ViewportHeight { get; }

	private (int, byte)[] buffer;

	public Renderer(int viewportWidth, int viewportHeight)
	{
		ViewportWidth = viewportWidth;
		ViewportHeight = viewportHeight;
		buffer = new (int, byte)[viewportWidth];
	}

	public (int, byte)[] Render((int, byte)[] data)
	{
		for (int i = 0; i < ViewportWidth; i++)
		{
			/* With overdraw
			if (data[i] == buffer[i]) continue;
			(int newV, byte newB) = data[i];
			(int oldV, byte oldB) = buffer[i];
			int newTop = ValueToTop(newV), oldTop = ValueToTop(oldV);
			if (newV < oldV)
				Console.MoveBufferArea(
					i, ValueToTop(oldV), 1, oldV, ViewportWidth, 0, '#', ConsoleColor.Black, ConsoleColor.Black);
			(var bg, var fg) = ConvertBrightness(newB);
			Console.MoveBufferArea(i, newTop, 1, newV, ViewportWidth, 0, '#', fg, bg);
			//*/
			//* No overdraw
			if (data[i] == buffer[i]) continue;
			(int newV, byte newB) = data[i];
			(int oldV, byte oldB) = buffer[i];
			int newTop = ValueToTop(newV), oldTop = ValueToTop(oldV);
			if (newB != oldB)
			{
				if (newV < oldV)
				{
					int oldBottom = oldTop + oldV, newBottom = newTop + newV;
					Console.MoveBufferArea(i, oldTop, 1, newTop - oldTop, ViewportWidth, 0,
						'#', ConsoleColor.Black, ConsoleColor.Black);
					Console.MoveBufferArea(i, newBottom, 1, oldBottom - newBottom, ViewportWidth, 0,
						'#', ConsoleColor.Black, ConsoleColor.Black);
				}
				(var bg, var fg) = ConvertBrightness(newB);
				Console.MoveBufferArea(i, newTop, 1, newV, ViewportWidth, 0, '#', fg, bg);
			}
			else
			{
				(int top1, int top2, int bottom1, int bottom2, (var bg, var fg)) = (newV > oldV)
					? (newTop, oldTop, oldTop + oldV, newTop + newV, ConvertBrightness(newB))
					: (oldTop, newTop, newTop + newV, oldTop + oldV, (ConsoleColor.Black, ConsoleColor.Black));
				Console.MoveBufferArea(i, top1, 1, top2 - top1, ViewportWidth, 0, '0', fg, bg);
				Console.MoveBufferArea(i, bottom1, 1, bottom2 - bottom1, ViewportWidth, 0, '0', fg, bg);
			}
			//*/
		}
		(buffer, data) = (data, buffer);
		return data;
	}

	private int ValueToTop(int value)
	{
		int top = (ViewportHeight - value) / 2;
		return (top >= 0) ? top : 0;
	}

	private static (ConsoleColor, ConsoleColor) ConvertBrightness(byte value)
	{
		return value switch {
			0 => (ConsoleColor.Black, ConsoleColor.Black),
			1 => (ConsoleColor.Black, ConsoleColor.DarkGray),
			2 => (ConsoleColor.DarkGray, ConsoleColor.DarkGray),
			3 => (ConsoleColor.DarkGray, ConsoleColor.Gray),
			4 => (ConsoleColor.Gray, ConsoleColor.Gray),
			5 => (ConsoleColor.Gray, ConsoleColor.White),
			_ => (ConsoleColor.White, ConsoleColor.White),
		};
	}
}
