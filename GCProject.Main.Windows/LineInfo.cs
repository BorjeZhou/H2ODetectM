namespace GCProject.Main.Windows;

public class LineInfo
{
	public double Slope { get; set; }

	public double Offset { get; set; }

	public double StartTemp { get; set; }

	public double EndTemp { get; set; }

	public double callValue(double value)
	{
		return Slope * value + Offset;
	}
}
