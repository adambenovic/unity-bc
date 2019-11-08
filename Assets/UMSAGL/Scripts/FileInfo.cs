public struct FileInfo
{
	public string filePath;
	public int lineFrom;
	public int lineTo;

	public FileInfo(string filePath, int lineFrom, int lineTo)
	{
		this.filePath = filePath;
		this.lineFrom = lineFrom;
		this.lineTo = lineTo;
	}
}
