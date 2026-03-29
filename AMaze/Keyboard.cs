using System.Runtime.InteropServices;

namespace AMaze;

internal class Keyboard
{
	private readonly ConsoleKey[] keysToCheck;

	public Keyboard(ConsoleKey[] keysToCheck)
	{
		this.keysToCheck = keysToCheck;
	}

	public ConsoleKey ReadKey()
	{
		for (int i = 0; i < keysToCheck.Length; i++)
			if ((GetAsyncKeyState((int)keysToCheck[i]) & 0x8000) != 0)
				return keysToCheck[i];
		return ConsoleKey.None;
	}

	[DllImport("user32.dll")]
	private static extern short GetAsyncKeyState(int vKey);
}
