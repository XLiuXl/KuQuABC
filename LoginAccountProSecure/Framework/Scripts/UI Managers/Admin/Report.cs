using UnityEngine.UI;


public class Report
{
	public string Id { get; set; }
	public bool ToBeRemoved { get; set; }
	public Toggle IsDoneCheckbox { get; set; }

	public Report(string id, Toggle isDoneCheckbox)
	{
		Id = id;
		ToBeRemoved = false;
		IsDoneCheckbox = isDoneCheckbox;
	}
}
