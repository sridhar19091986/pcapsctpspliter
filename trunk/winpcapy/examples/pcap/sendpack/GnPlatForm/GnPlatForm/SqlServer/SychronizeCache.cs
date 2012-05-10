/*
 * 
 * 生产者消费者的共享区
 * 
 * 
 * */





using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GnPlatForm.SqlServer
{
    public class SychronizeCache<T>
    {
        public ConcurrentQueue<T> _queue;
        private T ttt;
        private int maxlength;

        public SychronizeCache(int maxlength)
        {
            this.maxlength = maxlength;
            _queue = new ConcurrentQueue<T>();
            ttt = default(T);
        }

        public void Enqueue(T data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            lock (_queue)
            {
                while (_queue.Count >= maxlength)
                    Monitor.Wait(_queue);
                _queue.Enqueue(data);
                Monitor.Pulse(_queue);
            }
        }

        public T Dequeue()
        {
            lock (_queue)
            {
                while (_queue.Count == 0)
                    Monitor.Wait(_queue);
                _queue.TryDequeue(out ttt);
                return ttt;
            }
        }
    }
}
