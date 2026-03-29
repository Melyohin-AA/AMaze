using System.Drawing;

namespace AMaze;

internal static class PaletteManager
{
	public static void Set(bool warm)
	{
		const byte light = 32;
		const float warmFactor = 1.14f;
		var colors = new Color[16];
		for (int i = 0; i < 8; i++)
		{
			int br = i * light;
			int brRG = warm ? (int)(br * warmFactor) : br;
			int altBrG = warm ? (int)(i * warmFactor) : i;
			colors[i] = Color.FromArgb(brRG, brRG, br);
			colors[i + 8] = Color.FromArgb(brRG, altBrG, 0);
		}
		Recolorer.Recolor(colors);
	}
}
