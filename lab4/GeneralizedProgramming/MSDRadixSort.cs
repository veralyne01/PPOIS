using System;
using System.Collections.Generic;

namespace GeneralizedProgramming
{
    /// <summary>
    /// MSD radix sort (от старшего разряда к младшему).
    ///
    /// Radix-сортировка не сравнивает элементы, ей нужны «цифры». Поэтому для
    /// произвольного типа T передаётся делегат-ключ: Func&lt;T, int&gt; или Func&lt;T, string&gt;.
    /// Для int и string есть короткие перегрузки без делегата.
    ///
    /// Сортировка стабильная. Сортирует массив или List&lt;T&gt; на месте.
    /// </summary>
    public static class MSDRadixSort
    {
        private const int IntRadix = 256;         // основание: один байт
        private const int IntStartShift = 24;     // начинаем со старшего байта
        private const uint SignBit = 0x80000000u; // сдвиг для корректной работы с отрицательными числами

        // Для строк разряд = один байт символа UTF-16 (старший, затем младший).
        // Корзина 0 = «строка закончилась», корзины 1..256 = значения байта + 1.
        private const int StringBuckets = 257;

        // ---------- Публичный API: готовые типы ----------

        public static void Sort(int[] array) => Sort(array, x => x);

        public static void Sort(List<int> list) => Sort(list, x => x);

        public static void Sort(string[] array) => Sort(array, x => x);

        public static void Sort(List<string> list) => Sort(list, x => x);

        // ---------- Публичный API: произвольный T с целочисленным ключом ----------

        public static void Sort<T>(T[] array, Func<T, int> keySelector)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            SortCore(array, keySelector);
        }

        public static void Sort<T>(List<T> list, Func<T, int> keySelector)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            T[] buffer = list.ToArray();
            SortCore(buffer, keySelector);
            for (int i = 0; i < buffer.Length; i++)
                list[i] = buffer[i];
        }

        // ---------- Публичный API: произвольный T со строковым ключом ----------

        public static void Sort<T>(T[] array, Func<T, string> keySelector)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            SortCore(array, keySelector);
        }

        public static void Sort<T>(List<T> list, Func<T, string> keySelector)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            T[] buffer = list.ToArray();
            SortCore(buffer, keySelector);
            for (int i = 0; i < buffer.Length; i++)
                list[i] = buffer[i];
        }

        // ---------- Целочисленные ключи ----------

        private static void SortCore<T>(T[] items, Func<T, int> keySelector)
        {
            int n = items.Length;
            if (n < 2) return;

            // Ключи переводим в uint со сдвигом знака: так порядок uint совпадает с порядком int
            var keys = new uint[n];
            for (int i = 0; i < n; i++)
                keys[i] = unchecked((uint)keySelector(items[i])) ^ SignBit;

            SortIntRange(items, keys, new T[n], new uint[n], 0, n, IntStartShift);
        }

        private static void SortIntRange<T>(
            T[] items, uint[] keys, T[] auxItems, uint[] auxKeys, int lo, int hi, int shift)
        {
            if (hi - lo < 2 || shift < 0) return;

            // 1. Подсчёт размеров корзин (со сдвигом на 1, чтобы получить начала корзин префиксными суммами)
            var count = new int[IntRadix + 1];
            for (int i = lo; i < hi; i++)
                count[(int)((keys[i] >> shift) & 0xFF) + 1]++;

            // 2. Теперь count[r] = начало корзины r (относительно lo)
            for (int r = 0; r < IntRadix; r++)
                count[r + 1] += count[r];

            // 3. Стабильная раскладка по корзинам во вспомогательный буфер
            for (int i = lo; i < hi; i++)
            {
                int bucket = (int)((keys[i] >> shift) & 0xFF);
                int pos = lo + count[bucket]++;
                auxItems[pos] = items[i];
                auxKeys[pos] = keys[i];
            }

            Array.Copy(auxItems, lo, items, lo, hi - lo);
            Array.Copy(auxKeys, lo, keys, lo, hi - lo);

            // 4. После раскладки count[r] = конец корзины r; рекурсивно сортируем каждую корзину по следующему байту
            int start = 0;
            for (int r = 0; r < IntRadix; r++)
            {
                int end = count[r];
                if (end - start > 1)
                    SortIntRange(items, keys, auxItems, auxKeys, lo + start, lo + end, shift - 8);
                start = end;
            }
        }

        // ---------- Строковые ключи ----------

        private static void SortCore<T>(T[] items, Func<T, string> keySelector)
        {
            int n = items.Length;
            if (n < 2) return;

            var keys = new string[n];
            for (int i = 0; i < n; i++)
            {
                keys[i] = keySelector(items[i])
                    ?? throw new ArgumentException("Ключ сортировки не может быть null.", nameof(keySelector));
            }

            SortStringRange(items, keys, new T[n], new string[n], 0, n, 0);
        }

        /// <summary>Разряд строки на данном уровне: 0 = строка закончилась, иначе байт символа + 1.</summary>
        private static int Digit(string key, int level)
        {
            int index = level >> 1; // два уровня на один символ
            if (index >= key.Length) return 0;

            char c = key[index];
            int b = (level & 1) == 0 ? c >> 8 : c & 0xFF;
            return b + 1;
        }

        private static void SortStringRange<T>(
            T[] items, string[] keys, T[] auxItems, string[] auxKeys, int lo, int hi, int level)
        {
            if (hi - lo < 2) return;

            var count = new int[StringBuckets + 1];
            for (int i = lo; i < hi; i++)
                count[Digit(keys[i], level) + 1]++;

            for (int r = 0; r < StringBuckets; r++)
                count[r + 1] += count[r];

            for (int i = lo; i < hi; i++)
            {
                int bucket = Digit(keys[i], level);
                int pos = lo + count[bucket]++;
                auxItems[pos] = items[i];
                auxKeys[pos] = keys[i];
            }

            Array.Copy(auxItems, lo, items, lo, hi - lo);
            Array.Copy(auxKeys, lo, keys, lo, hi - lo);

            // Корзина 0 (закончившиеся строки) содержит равные ключи, её дальше не сортируем
            int start = 0;
            for (int r = 0; r < StringBuckets; r++)
            {
                int end = count[r];
                if (r > 0 && end - start > 1)
                    SortStringRange(items, keys, auxItems, auxKeys, lo + start, lo + end, level + 1);
                start = end;
            }
        }
    }
}