namespace CarService
{
    public static class QueueExtentions
    {
        public static void EnqueueRange<T>(this Queue<T> queue, List<T> items)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }
}
