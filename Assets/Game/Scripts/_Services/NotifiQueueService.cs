using Assets.Game.Scripts.BaseFramework.Architecture;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts._Services
{
    public class NotifiQueueService : Service
    {
        private List<INotifiQueue> queue;
        public override void OnInit()
        {
            base.OnInit();
            queue = new List<INotifiQueue>();
        }

        public INotifiQueue Next()
        {
            if (queue.Count == 0) return null;
            var data = queue[0];
            queue.RemoveAt(0);
            return data;
        }

        public void EnQueue(INotifiQueue data)
        {
            if (data.IsShowOnly)
            {
                queue.RemoveAll(t => t.Type == data.Type);
            }
            queue.Add(data);
        }


        public async UniTask FireNow(INotifiQueue data)
        {
            await data.Notify();
        }
        public async UniTask Update()
        {
            var obj = Next();
            if (obj == null) return;
            await obj.Notify();
        }
    }

    public interface INotifiQueueRespone
    {

    }

    public interface INotifiQueue
    {
        int _id { get; set; }
        string Type { set; get; }
        bool IsShowOnly { set; get; }
        UniTask Notify();
    }

    public abstract class NotifiQueueData : INotifiQueue
    {
        public static int Id = 0;
        public virtual bool IsShowOnly { set; get; } = true;

        public int _id { set; get; }
        public string Type { set; get; }

        public NotifiQueueData()
        {
            _id = Id++;
        }

        public NotifiQueueData(string type) : this()
        {
            Type = type;
        }

        public abstract UniTask Notify();
    }
}
