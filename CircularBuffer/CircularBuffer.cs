/*
 * Проблемы:
 * 1. Возможны извлечения из буфера default значений для типов значений, что говорит о том, что буфер пуст, нужен другой механизм уведомления о пустом буфере
 * 2. При многопоточной работе, возможна ситуация, когда все попы прошли, а 1 пуш еще ждал когда ему вставить значение
 *
 */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircularBuffer
{
    /// <summary>
    /// Реализация кольцевого буфера
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T> : ICircularBuffer<T>
    {
        public int Capacity { get; }

        public BufferMode Mode { get; }

        public int Count { get; private set; }

        private T[] _buffer;

        private int _headIndex;
        private int _freeIndex;

        private readonly ManualResetEventSlim _eventSlim = new ManualResetEventSlim();

        private readonly object _sync = new object();

        /// <summary>
        /// Конструктор буфера
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="mode"></param>
        public CircularBuffer(int capacity = 7, BufferMode mode = BufferMode.Wait)
        {
            if (capacity <= 0)
                Capacity = 7;

            Capacity = capacity;
            Mode = mode;
            _buffer = new T[capacity];
            _freeIndex = _buffer.Length - 1; //Свободная ячейка указаывает на последний элемент массива
            _headIndex = -1; //Голова ни на что не указывает, т.к. нет значения
        }

        public void Push(T item)
        {
            if (item.Equals(null))
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Mode == BufferMode.Wait && Count == _buffer.Length)
            {
                _eventSlim.Wait(); //Тут сразу после выхода надо хватать поток на себя
                    
                Monitor.Enter(_sync);
            }

            if (Mode == BufferMode.Replace && Count == _buffer.Length)
            {
                Pop(); //Впустую извлекаем элемент из головы
            }
            if (!Monitor.IsEntered(_sync))
                Monitor.Enter(_sync);
            _buffer[_freeIndex] = item;

            if (_headIndex == -1)
                _headIndex = _freeIndex;

            _freeIndex--;
            if (_freeIndex == -1)
                _freeIndex = _buffer.Length - 1;

            Count++;

            if (Mode == BufferMode.Wait && Count == _buffer.Length)
            {
                _eventSlim.Reset();
            }
            Monitor.Exit(_sync);
        }

        public Task PushAsync(T item)
        {
            return Task.Run(() => Push(item));
        }

        public T Pop()
        {
            lock (_sync)
            {
                if (_headIndex == -1)
                {
                    return default; //todo надо понять нужно ли тут возвращать что-то по default
                }

                var el = _buffer[_headIndex];
                if (!(el is ValueType))
                {
                    // Для ссылочных типов тут встанет null. Нужно для того что бы не хранить лишнюю ссылку на объект, когда он извлечен
                    // для valuetype это значение не имеет, т.к. возвращается копия, а не ссылка
                    _buffer[_headIndex] = default;
                }

                if (_headIndex == 0)
                    _headIndex = _buffer.Length - 1;
                else
                    _headIndex--;
                if (_headIndex == _freeIndex)
                {
                    _headIndex = -1;
                }

                Count--;
                _eventSlim.Set();
                return el;
            }
        }

        public Task<T> PopAsync()
        {
            return Task.FromResult(Pop());
        }

        public void Resize(int size)
        {
            if (size < Capacity)
                throw new ArgumentException("Не допустимы размер. Буфер можно только увеличить.");

            if (size == Capacity) return;

            Array.Resize(ref _buffer, size);

        }
    }
}
