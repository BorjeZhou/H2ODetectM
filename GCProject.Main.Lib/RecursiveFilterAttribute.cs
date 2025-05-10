namespace GCProject.Main.Lib;

internal class RecursiveFilterAttribute : CalculatorBaseAttribute
{
	public override CalTypeEnum CalType { get; set; } = CalTypeEnum.Recursive;


	public int RecursiveDuration_s { get; set; } = 1;

}
