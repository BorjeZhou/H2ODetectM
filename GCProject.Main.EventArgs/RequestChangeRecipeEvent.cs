namespace GCProject.Main.EventArgs;

public class RequestChangeRecipeEvent
{
	public int SortNum;

	public bool Success;

	public RequestChangeRecipeEvent(int num)
	{
		SortNum = num;
	}
}
