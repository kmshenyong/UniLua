using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrinderApp.Core.Mvvm
{
    /// <summary>
    /// 订阅事件的 Holder
    /// </summary>
    public class EventSubscriptionHolder
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public EventSubscriptionHolder(EventBase @event, SubscriptionToken token, params object[] keeps)
        {
            Event = @event;
            Token = token;
            Keeps = keeps;
        }

        /// <summary>
        /// 事件对象
        /// </summary>
        public EventBase Event { get; }

        /// <summary>
        /// 卸载用 Token
        /// </summary>
        public SubscriptionToken Token { get; }

        /// <summary>
        /// 需要引用持有的对象
        /// </summary>
        public object[] Keeps { get; }

        public override string ToString() => $"Hold << {Event.GetType().Name} >>";
    }

}
