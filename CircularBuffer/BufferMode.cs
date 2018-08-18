namespace CircularBuffer
{
    /// <summary>
    /// Режим работы буфера
    /// </summary>
    public enum BufferMode
    {
        /// <summary>
        /// Режим замены старых элементов
        /// </summary>
        Replace,

        /// <summary>
        /// Режим ожидания извлечения элементов
        /// </summary>
        Wait
    }
}