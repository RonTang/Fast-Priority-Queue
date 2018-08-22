using System.IO;
using System;



public class PriorityQueue<T> where T : IComparable<T>
{

    public int numberOfItems;

    /** The tree will grow by at least this factor every time it is expanded */
    public float growthFactor = 2;

    /**
	* Number of children of each node in the tree.
	* Different values have been tested and 4 has been empirically found to perform the best.
	*  https://en.wikipedia.org/wiki/D-ary_heap
	*/
    const int D = 4;

    public const ushort MaxSize = 0xFFFF;

    /** Internal backing array for the heap */
    private T[] heap;
    /** True if the heap does not contain any elements */
    public bool isEmpty
    {
        get
        {
            return numberOfItems <= 0;
        }
    }

    /** Create a new heap with the specified initial capacity */
    public PriorityQueue(int capacity)
    {

        heap = new T[capacity];
        numberOfItems = 0;
    }

    public PriorityQueue(T[] data)
    {

        heap = new T[data.Length];
        numberOfItems = data.Length;
        Array.Copy(data, heap, data.Length);
        BuildHeap();
    }

    /** Removes all elements from the heap */
    public void Clear()
    {
        numberOfItems = 0;
    }
    /** Expands to a larger backing array when the current one is too small */
    void Expand()
    {
        
        int newSize = System.Math.Max(heap.Length + 4, System.Math.Min(MaxSize - 1, (int)System.Math.Round(heap.Length * growthFactor)));


        // Check if the heap is really large
        // Also note that heaps larger than this are not supported

        if (newSize >= MaxSize)
        {
            throw new System.Exception($"{D}-ary Heap Size really large (>={MaxSize}). A heap size this large is probably the cause of data structure in an infinite loop. ");
        }

        var newHeap = new T[newSize];
        heap.CopyTo(newHeap, 0);
        heap = newHeap;
    }
    /** Adds a node to the heap */
    public void Push(T node)
    {
        if (node == null) throw new System.ArgumentNullException("node");

        if (numberOfItems == heap.Length)
        {
            Expand();
        }

        DecreaseKey(node, (ushort)numberOfItems);
        numberOfItems++;
    }

    void DecreaseKey(T node, ushort index)
    {

        if (index < numberOfItems)
        {
            if (node.CompareTo(heap[index]) > 0)
            {
                throw new System.Exception("New node key greater than orginal key");
            }
        }
        int bubbleIndex = index;


        while (bubbleIndex != 0)
        {
            // Parent node of the bubble node
            int parentIndex = (bubbleIndex - 1) / D;

            if (node.CompareTo(heap[parentIndex]) < 0)
            {
                // Swap the bubble node and parent node
                // (we don't really need to store the bubble node until we know the final index though
                // so we do that after the loop instead)
                heap[bubbleIndex] = heap[parentIndex];
                bubbleIndex = parentIndex;
            }
            else
            {
                break;
            }
        }

        heap[bubbleIndex] = node;
    }

    public T ExtractMin()
    {
        T returnItem = heap[0];


        numberOfItems--;
        if (numberOfItems == 0) return returnItem;

        // Last item in the heap array
        var swapItem = heap[numberOfItems];


        int swapIndex = 0, parent;

        // Trickle upwards
        while (true)
        {
            parent = swapIndex;
            var curSwapItem = swapItem;
            int pd = parent * D + 1;

            // If this holds, then the indices used
            // below are guaranteed to not throw an index out of bounds
            // exception since we choose the size of the array in that way
            if (pd <= numberOfItems)
            {


                for (int i = 0; i < D - 1; i++)
                {
                    if (pd + i < numberOfItems && (heap[pd + i].CompareTo(curSwapItem) < 0))
                    {
                        curSwapItem = heap[pd + i];
                        swapIndex = pd + i;
                    }


                }


                if (pd + D - 1 < numberOfItems && (heap[pd + D - 1].CompareTo(curSwapItem) < 0))
                {
                    swapIndex = pd + D - 1;
                }
            }

            // One if the parent's children are smaller or equal, swap them
            // (actually we are just pretenting we swapped them, we hold the swapData
            // in local variable and only assign it once we know the final index)
            if (parent != swapIndex)
            {
                heap[parent] = heap[swapIndex];
            }
            else
            {
                break;
            }
        }

        // Assign element to the final position
        heap[swapIndex] = swapItem;

        // For debugging remove "//" before Validate
        // Validate ();

        return returnItem;
    }


    public T Pop()
    {
        return ExtractMin();
    }

    void Validate()
    {
        for (int i = 1; i < numberOfItems; i++)
        {
            int parentIndex = (i - 1) / D;
            if (heap[parentIndex].CompareTo(heap[i]) > 0)
            {
                throw new System.Exception("Invalid state at " + i + ":" + parentIndex);
            }
        }
    }

    /** Builds the heap by trickeling down all items. */
    public void BuildHeap()
    {
        for (int i = 1; i < numberOfItems; i++)
        {
            int bubbleIndex = i;
            var node = heap[i];

            while (bubbleIndex != 0)
            {
                int parentIndex = (bubbleIndex - 1) / D;

                if (node.CompareTo(heap[parentIndex]) < 0)
                {
                    heap[bubbleIndex] = heap[parentIndex];

                    heap[parentIndex] = node;

                    bubbleIndex = parentIndex;

                    
                }
                else
                {
                    break;
                }
            }
        }
    }
}
