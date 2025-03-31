using DynamicData;
using Newtonsoft.Json.Bson;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LimitedSizeStack;

public class ListModel<TItem>
{
    public List<TItem> Items { get; }
    private int undoLimit;
    private LimitedSizeStack<ICommand<TItem>> history;

    public int UndoLimit
    {
        get => undoLimit;
        set
        {
            undoLimit = value;
            history = new LimitedSizeStack<ICommand<TItem>>(undoLimit);
        }
    }

    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit) { }

    public ListModel(List<TItem> items, int undoLimit)
    {
        Items = items;
        this.undoLimit = undoLimit;
        history = new LimitedSizeStack<ICommand<TItem>>(undoLimit);
    }

    public void AddItem(TItem item)
    {
        var command = new AddCommand<TItem>(Items, item);
        command.Execute();
        history.Push(command);
    }

    public void RemoveItem(int index)
    {
        var command = new RemoveCommand<TItem>(Items, index);
        command.Execute();
        history.Push(command);
    }

    public bool CanUndo() => history.Count > 0;

    public void Undo()
    {
        if (CanUndo())
            history.Pop().Undo();
    }

    public void MoveUp(int index)
    {
        if (index <= 0 || index >= Items.Count)
            return;
        var command = new MoveUpCommand<TItem>(Items, index);
        command.Execute();
        history.Push(command);
    }

    public void MoveDown(int index)
    {
        if (index >= Items.Count - 1)
            return;
        var command = new MoveDownCommand<TItem>(Items, index);
        command.Execute();
        history.Push(command);
    }

    public static void ChangePosition(List<TItem> items, int previousIndex, int newIndex)
    {
        var indexItem = items[previousIndex];
        var nextItem = items[newIndex];
        items[previousIndex] = nextItem;
        items[newIndex] = indexItem;
    }
}

public interface ICommand<TItem>
{
    void Execute();
    void Undo();
}

public class MoveDownCommand<TItem> : ICommand<TItem>
{
    private readonly List<TItem> items;
    private readonly int index;
    private readonly TItem item;

    public MoveDownCommand(List<TItem> items, int index)
    {
        this.items = items;
        this.index = index;
        item = items[index];
    }


    public void Execute()
    {
        if (items.Count > 0 && index < items.Count - 1)
            ListModel<TItem>.ChangePosition(items, index, index + 1);
    }

    public void Undo() =>
        ListModel<TItem>.ChangePosition(items, index + 1, index);
}

public class MoveUpCommand<TItem> : ICommand<TItem>
{
    private readonly List<TItem> items;
    private readonly int index;
    private readonly TItem item;

    public MoveUpCommand(List<TItem> items, int index)
    {
        this.items = items;
        this.index = index;
        item = items[index];
    }

    public void Execute()
    {
        if (items.Count > 0 && index >= 1)
            ListModel<TItem>.ChangePosition(items, index, index - 1);
    }

    public void Undo() =>
        ListModel<TItem>.ChangePosition(items, index - 1, index);
}

public class AddCommand<TItem> : ICommand<TItem>
{
    private readonly List<TItem> items;
    private readonly TItem item;

    public AddCommand(List<TItem> items, TItem item)
    {
        this.items = items;
        this.item = item;
    }

    public void Execute() =>
        items.Add(item);

    public void Undo() =>
        items.RemoveAt(items.Count - 1);
}

public class RemoveCommand<TItem> : ICommand<TItem>
{
    private readonly List<TItem> items;
    private readonly TItem item;
    private readonly int index;

    public RemoveCommand(List<TItem> items, int index)
    {
        this.items = items;
        this.index = index;
        item = items[index];
    }

    public void Execute() =>
        items.RemoveAt(index);

    public void Undo() =>
        items.Insert(index, item);
}
