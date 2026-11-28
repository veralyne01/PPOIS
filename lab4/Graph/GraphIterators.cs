using System;
using System.Collections;
using System.Collections.Generic;

namespace GeneralizedProgramming
{
    /// <summary>Внутренний контракт владельца итератора (нужен для проверки версии графа).</summary>
    internal interface IGraphOwner
    {
        int Version { get; }
    }

    /// <summary>
    /// Двунаправленный итератор. Позиции: -1 = перед началом, Count = за концом.
    /// Итератор работает по снимку, сделанному в момент создания. Если граф
    /// изменился после создания итератора, итератор становится недействительным.
    /// </summary>
    public abstract class GraphIteratorBase<TItem> : IEnumerator<TItem>
    {
        private readonly List<TItem> _items;
        private readonly int _version;
        private int _index = -1;

        internal IGraphOwner Owner { get; }

        internal GraphIteratorBase(IGraphOwner owner, List<TItem> items)
        {
            Owner = owner;
            _items = items;
            _version = owner.Version;
        }

        public bool IsValid => _version == Owner.Version;

        public bool HasCurrent => IsValid && _index >= 0 && _index < _items.Count;

        public TItem Current
        {
            get
            {
                EnsureValid();
                if (_index < 0 || _index >= _items.Count)
                    throw new InvalidIteratorException("Итератор не указывает на элемент (до начала или за концом).");
                return _items[_index];
            }
        }

        object? IEnumerator.Current => Current;

        public bool MoveNext()
        {
            EnsureValid();
            if (_index < _items.Count) _index++;
            return _index < _items.Count;
        }

        public bool MovePrev()
        {
            EnsureValid();
            if (_index >= 0) _index--;
            return _index >= 0;
        }

        public void Reset()
        {
            EnsureValid();
            _index = -1;
        }

        public void Dispose() { }

        private void EnsureValid()
        {
            if (!IsValid)
                throw new InvalidIteratorException("Граф был изменён, итератор недействителен.");
        }
    }

    /// <summary>Изменяющий итератор: его можно передать в RemoveVertex / RemoveEdge.</summary>
    public sealed class GraphIterator<TItem> : GraphIteratorBase<TItem>
    {
        internal GraphIterator(IGraphOwner owner, List<TItem> items) : base(owner, items) { }
    }

    /// <summary>Константный итератор: только чтение.</summary>
    public sealed class ConstGraphIterator<TItem> : GraphIteratorBase<TItem>
    {
        internal ConstGraphIterator(IGraphOwner owner, List<TItem> items) : base(owner, items) { }
    }

    /// <summary>Обобщённые алгоритмы обхода поверх итераторов графа.</summary>
    public static class GraphAlgorithms
    {
        /// <summary>Применяет действие ко всем элементам, начиная с позиции итератора.</summary>
        public static void ForEach<TItem>(GraphIteratorBase<TItem> iterator, Action<TItem> action)
        {
            if (iterator == null) throw new ArgumentNullException(nameof(iterator));
            if (action == null) throw new ArgumentNullException(nameof(action));

            while (iterator.MoveNext())
                action(iterator.Current);
        }
    }
}