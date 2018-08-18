using System;
using System.Threading.Tasks;

namespace CircularBuffer
{
    /// <summary>
    /// Интерфейс кольцевого буфера
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICircularBuffer<T>
    {
        /// <summary>
        /// Текущий размер буфера
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Количество элементов в буфере
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Режим работы буфера
        /// </summary>
        BufferMode Mode { get; }

        /// <summary>
        /// Вставляет элемент в буфер
        /// </summary>
        /// <param name="item"></param>
        void Push(T item);

        /// <summary>
        /// Асинхронно вставляет элемент в буфер
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task PushAsync(T item);

        /// <summary>
        /// Извлекает элемент из буфера
        /// </summary>
        /// <returns></returns>
        T Pop();

        /// <summary>
        /// Асинхронно извлекает элемент из буфера
        /// </summary>
        /// <returns></returns>
        Task<T> PopAsync();

        /// <summary>
        /// Изменяет размер буфера
        /// </summary>
        /// <param name="size"></param>
        void Resize(int size);
    }
}