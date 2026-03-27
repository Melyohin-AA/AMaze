using System.ComponentModel;
using System.Runtime.InteropServices;

namespace AMaze;

internal static class ConsoleUpdater
{
	private static readonly IntPtr stdout = GetStdHandle(-11);
	private static readonly CharInfo[] backBuffer;
	private static readonly short bufferWidth, bufferHeight;

	static ConsoleUpdater()
	{
		(backBuffer, bufferWidth, bufferHeight) = EnsureBuffer();
	}

	private static (CharInfo[], short, short) EnsureBuffer()
	{
		ConsoleScreenBufferInfo info;
		if (!GetConsoleScreenBufferInfo(stdout, out info))
			throw new Win32Exception(Marshal.GetLastWin32Error());
		short width = (short)(info.srWindow.right - info.srWindow.left + 1);
		short height = (short)(info.srWindow.bottom - info.srWindow.top + 1);
		var buffer = new CharInfo[width * height];
		short defaultAttr = info.wAttributes;
		for (int i = 0; i < buffer.Length; i++)
		{
			buffer[i].UnicodeChar = ' ';
			buffer[i].Attributes = defaultAttr;
		}
		return (buffer, width, height);
	}

	public static void Update(Span<Cell> cells)
	{
		short minX = short.MaxValue, minY = short.MaxValue, maxX = short.MinValue, maxY = short.MinValue;
		for (int i = 0; i < cells.Length; i++)
		{
			ref readonly Cell c = ref cells[i];
			int index = c.Y * bufferWidth + c.X;
			backBuffer[index].UnicodeChar = c.Char;
			backBuffer[index].Attributes = c.Color.Value;
			if (c.X < minX)
				minX = c.X;
			if (c.Y < minY)
				minY = c.Y;
			if (c.X > maxX)
				maxX = c.X;
			if (c.Y > maxY)
				maxY = c.Y;
		}
		if ((minX > maxX) || (minY > maxY)) return;
		var writeRegion = new SmallRect() { left = minX, top = minY, right = maxX, bottom = maxY };
		var bufferSize = new Coord { x = bufferWidth, y = bufferHeight };
		var bufferCoord = new Coord { x = (short)minX, y = (short)minY };
		if (!WriteConsoleOutputW(stdout, backBuffer, bufferSize, bufferCoord, ref writeRegion))
			throw new Win32Exception(Marshal.GetLastWin32Error());
	}

	public readonly struct Color
	{
		public short Value { get; }
		
		public Color(short value)
		{
			Value = value;
		}
		public Color(ConsoleColor backColor, ConsoleColor foreColor)
		{
			Value = (short)(((int)backColor << 4) | (int)foreColor);
		}
	}

	public readonly struct Cell
	{
		public short X { get; }
		public short Y { get; }
		public char Char { get; }
		public Color Color { get; }

		public Cell(short x, short y, char ch, Color color)
		{
			X = x;
			Y = y;
			Char = ch;
			Color = color;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct Coord
	{
		public short x, y;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct SmallRect
	{
		public short left, top, right, bottom;
	}

	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	private struct CharInfo
	{
		[FieldOffset(0)] public char UnicodeChar;
		[FieldOffset(2)] public short Attributes;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct ConsoleScreenBufferInfo
	{
		public Coord dwSize;
		public Coord dwCursorPosition;
		public short wAttributes;
		public SmallRect srWindow;
		public Coord dwMaximumWindowSize;
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool GetConsoleScreenBufferInfo(
		IntPtr hConsoleOutput, out ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

	[DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", SetLastError = true)]
	private static extern bool WriteConsoleOutputW(IntPtr hConsoleOutput, [In] CharInfo[] lpBuffer,
		Coord dwBufferSize, Coord dwBufferCoord, ref SmallRect lpWriteRegion);
}
