using System;
using System.Collections.Generic;

namespace GeneralizedProgramming
{
    public static class BinaryTreeSort
    {
        private class Node<T>
        {
            public T Info;
            public Node<T>? Left;
            public Node<T>? Right;

            public Node(T info)
            {
                Info = info;
            }
        }

        public static void Sort<T>(T[] array) where T : IComparable<T>
        {
            Sort(array, Comparer<T>.Default);
        }

        public static void Sort<T>(T[] array, IComparer<T> comparer)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (array.Length < 2) return;

            Node<T> root = BuildTree(array, comparer);

            int i = 0;
            foreach (T item in InOrder(root))
                array[i++] = item;
        }

        public static void Sort<T>(List<T> list) where T : IComparable<T>
        {
            Sort(list, Comparer<T>.Default);
        }

        public static void Sort<T>(List<T> list, IComparer<T> comparer)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (list.Count < 2) return;

            Node<T> root = BuildTree(list, comparer);

            int i = 0;
            foreach (T item in InOrder(root))
                list[i++] = item;
        }
        private static Node<T> BuildTree<T>(IReadOnlyList<T> source, IComparer<T> comparer)
        {
            var root = new Node<T>(source[0]);

            for (int i = 1; i < source.Count; i++)
            {
                T item = source[i];
                Node<T> cur = root;

                while (true)
                {
                    if (comparer.Compare(item, cur.Info) < 0)
                    {
                        if (cur.Left == null)
                        {
                            cur.Left = new Node<T>(item);
                            break;
                        }
                        cur = cur.Left;
                    }
                    else
                    {
                        if (cur.Right == null)
                        {
                            cur.Right = new Node<T>(item);
                            break;
                        }
                        cur = cur.Right;
                    }
                }
            }

            return root;
        }

        private static IEnumerable<T> InOrder<T>(Node<T> root)
        {
            var stack = new Stack<Node<T>>();
            Node<T>? cur = root;

            while (cur != null || stack.Count > 0)
            {
                while (cur != null)
                {
                    stack.Push(cur);
                    cur = cur.Left;
                }

                cur = stack.Pop();
                yield return cur.Info;
                cur = cur.Right;
            }
        }
    }
}