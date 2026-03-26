namespace AMaze.Recolor;

internal class RecolorException : Exception
{
	public int ErrorCode { get; }

	public RecolorException(int errorCode) : base($"Color mapping failed: code {errorCode}")
	{
		ErrorCode = errorCode;
	}
}
