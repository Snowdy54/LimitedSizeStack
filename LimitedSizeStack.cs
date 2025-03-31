using System;
using System.Collections.Generic;

namespace LimitedSizeStack
{
    public class LimitedSizeStack<T>
    {
        private LinkedList<T> list;
        private int limit;


        public LimitedSizeStack(int undoLimit)
        {
            limit = undoLimit;
            list = new LinkedList<T>();
        }

        public void Push(T item)
        {
            if (limit > 0)
            {
                if (list.Count == limit)
                    list.RemoveFirst();
                list.AddLast(item);
            }
        }

        public T Pop()
        {
            if (list.Count == 0)
                throw new InvalidOperationException("Стек пуст");
            var result = list.Last.Value;
            list.RemoveLast();
            return result;
        }

        public int Limit
        {
            get => limit;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Лимит не может быть отрицательным");
                limit = value;

                while (list.Count > limit)
                {
                    list.RemoveFirst();
                }
            }
        }

        public int Count => list.Count;
    }
}
