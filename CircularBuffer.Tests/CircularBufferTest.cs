using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CircularBuffer.Tests
{
    public class CircularBufferTest
    {
        [Fact]
        public void SimplePushTest()
        {
            var buffer = new CircularBuffer<int>();

            buffer.Push(1);
            buffer.Push(2);
            buffer.Push(3);
            buffer.Push(4);
            buffer.Push(5);
            buffer.Push(6);
            buffer.Push(7);

            Assert.Equal(7, buffer.Count);
        }

        [Fact]
        public void SimplePushPopTest()
        {
            var buffer = new CircularBuffer<int>();
            buffer.Push(1);
            var actual = buffer.Pop();

            Assert.Equal(1, actual);
        }


        [Fact]
        public void OverPushModeReplaceTest()
        {
            var buffer = new CircularBuffer<int>(5, BufferMode.Replace);
            buffer.Push(1);
            buffer.Push(2);
            buffer.Push(3);
            buffer.Push(4);
            buffer.Push(5);
            buffer.Push(6);
            buffer.Push(7);
            var actual1 = buffer.Pop();
            var actual2 = buffer.Pop();

            Assert.Equal(3, actual1);
            Assert.Equal(4, actual2);
        }

        [Fact]
        public void OverPushModeWaitTest()
        {
            var buffer = new CircularBuffer<int>(5);
            buffer.Push(1);
            buffer.Push(2);
            buffer.Push(3);
            buffer.Push(4);
            buffer.Push(5);

            Task.Run(() => buffer.Push(6));
            Task.Delay(1000).GetAwaiter().GetResult();

            var actual1 = buffer.Pop();//1
            Task.Delay(1000).GetAwaiter().GetResult();
            buffer.Pop();//2
            buffer.Pop();//3
            buffer.Pop();//4
            buffer.Pop();//5
            var actual2 = buffer.Pop();//6

            Assert.Equal(1, actual1);
            Assert.Equal(6, actual2);
        }

        [Fact]
        public void SingleBufferTest()
        {
            var buffer = new CircularBuffer<int>(1, BufferMode.Replace);
            buffer.Push(1);
            buffer.Push(2);
            buffer.Push(3);
            buffer.Push(4);
            buffer.Push(5);

            var actual1 = buffer.Pop();//5
           
            Assert.Equal(5, actual1);
        }

        [Fact]
        public void SingleBufferObjectTest()
        {
            var buffer = new CircularBuffer<object>(1, BufferMode.Replace);
            var buf = new List<object>();
            for (var i = 0; i < 20; i++)
            {
                var o = new object();
                buf.Add(o);
                buffer.Push(o);
            }

            var actual1 = buffer.Pop();

            Assert.Equal(buf.Last(), actual1);
        }



    }
}
