using ReactiveUI;

namespace LimitedSizeStack.UI;

public class TaskListViewModel : ReactiveObject
{
	public string[] Items => items;

	public bool CanUndo => model.CanUndo();

	private readonly ListModel<string> model;

	private string[] items;

	public TaskListViewModel(ListModel<string> listModel)
	{
		model = listModel;
		Update();
	}

	public void MoveDownItem(int index)
	{
		model.MoveDown(index);
		Update();

	}

	public void MoveUpItem(int index)
	{
		model.MoveUp(index);
		Update();
	}

	public void AddItem(string item)
	{
		model.AddItem(item);
		Update();
	}

	public void RemoveItem(int index)
	{
		model.RemoveItem(index);
		Update();
	}

	public void Undo()
	{
		model.Undo();
		Update();
	}

	private void Update()
	{
		this.RaiseAndSetIfChanged(ref items, model.Items.ToArray(), nameof(Items));
	}
}