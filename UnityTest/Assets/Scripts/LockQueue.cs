using System.Collections.Generic;

public interface IPoolable
{
    void Dispose();
}

public class LockQueue<T> : IPoolable
{
    object lockObj = new object();
    Queue<T> queue = new Queue<T>();

    public void Enqueue(T item)
    {
        lock (lockObj)
        {
            queue.Enqueue(item);
        }
    }

    public T Dequeue()
    {
        lock (lockObj)
        {
            return queue.Dequeue();
        }
    }

    public int Count
    {
        get
        {
            lock (lockObj)
            {
                return queue.Count;
            }
        }
    }

    public T Get()
    {
        lock (lockObj)
        {
            return queue.Peek();
        }
    }

    public void Clear()
    {
        lock (lockObj)
        {
            queue.Clear();
        }
    }

    public void Dispose()
    {
    }
}