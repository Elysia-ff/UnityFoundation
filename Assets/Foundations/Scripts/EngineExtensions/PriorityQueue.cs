using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public class PriorityQueue<T>
    {
        /// <summary>
        /// lhs < rhs == ascending
        /// </summary>
        public delegate bool Comparer(T lhs, T rhs);

        private T[] _list;
        private readonly Comparer _comparer;

        public int Count { get; private set; }

        public PriorityQueue(int capacity, Comparer comparer)
        {
            _list = new T[capacity];
            _comparer = comparer;

            Count = 0;
        }

        public void Clear()
        {
            Array.Clear(_list, 0, Count);
            Count = 0;
        }

        public void Enqueue(T item)
        {
            GrowArray();

            _list[Count] = item;

            int current = Count;
            while (current > 0)
            {
                int next = GetParent(current);
                if (_comparer(_list[next], _list[current]))
                {
                    break;
                }

                (_list[current], _list[next]) = (_list[next], _list[current]);

                current = next;
            }

            Count++;
        }

        public T Dequeue()
        {
            T item = _list[0];

            Count--;
            _list[0] = _list[Count];
            _list[Count] = default;

            int current = 0;
            while (true)
            {
                int candidate = current;

                int left = GetLeftChild(current);
                if (left < Count && _comparer(_list[left], _list[current]))
                {
                    candidate = left;
                }

                int right = GetRightChild(current);
                if (right < Count && _comparer(_list[right], _list[candidate]))
                {
                    candidate = right;
                }

                if (current == candidate)
                {
                    break;
                }

                (_list[current], _list[candidate]) = (_list[candidate], _list[current]);

                current = candidate;
            }

            return item;
        }

        public bool TryDequeue(out T outItem)
        {
            if (Count == 0)
            {
                outItem = default;
                return false;
            }

            outItem = Dequeue();
            return true;
        }

        public bool Contains(Predicate<T> predicate)
        {
            for (int i = 0; i < Count; i++)
            {
                if (predicate(_list[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public T Peek()
        {
            return _list[0];
        }

        private void GrowArray()
        {
            if (Count < _list.Length)
            {
                return;
            }

            T[] newList = new T[_list.Length * 2];
            Array.Copy(_list, newList, _list.Length);

            _list = newList;
        }

        private int GetLeftChild(int parent)
        {
            return parent * 2 + 1;
        }

        private int GetRightChild(int parent)
        {
            return parent * 2 + 2;
        }

        private int GetParent(int child)
        {
            return (child - 1) / 2;
        }
    }
}
