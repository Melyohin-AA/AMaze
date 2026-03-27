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

	public static void SetCell(short x, short y, char ch, Color color)
	{
		int index = y * bufferWidth + x;
		backBuffer[index].UnicodeChar = ch;
		backBuffer[index].Attributes = color.Value;
	}

	public static void Flush()
	{
		var writeRegion = new SmallRect() { left = 0, top = 0, right = bufferWidth, bottom = bufferHeight };
		var bufferSize = new Coord { x = bufferWidth, y = bufferHeight };
		var bufferCoord = new Coord { x = 0, y = 0 };
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
