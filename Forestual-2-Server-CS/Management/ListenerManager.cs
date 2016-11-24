using System.Collections.Generic;
using Forestual2CoreCS;
using Forestual2CoreCS.Extension;

namespace Forestual2ServerCS.Management
{
    public class ListenerManager
    {
        public static List<Listener> Listeners = new List<Listener>();

        public static void RegisterListener(Listener listener) {
            Listeners.Add(listener);
        }

        public static void InvokeEvent(Event e, params object[] args) {
            Listeners.FindAll(l => l.Event == e).ForEach(l => l.Delegate.DynamicInvoke(args));
        }
    }
}
