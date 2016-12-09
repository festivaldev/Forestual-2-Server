﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Forestual2CoreCS;
using Forestual2CoreCS.Extension;
using Forestual2CoreCS.Management;

namespace Forestual2ServerCS.Management
{
    public class ListenerManager
    {
        public static List<Listener> Listeners = new List<Listener>();

        public static void RegisterListener(Listener listener) {
            Listeners.Add(listener);
        }

        public static void InvokeEvent(Event e, params object[] args) {
            try {
                Listeners.FindAll(l => l.Event == e).ForEach(l => l.Delegate.DynamicInvoke(args));
            } catch (Exception Ex) {
                ServerManagement.Server.PrintToConsole($"Exception:  {Ex.Message}\nStacktrace: {Ex.StackTrace}\nSource:{Ex.Source}", ColorTranslator.FromHtml("#FC3539"));
            }
        }

        public static void InvokeSpecialEvent(EventArguments e) {
            try {
                Listeners.FindAll(l => l.Event == Event.Dynamic).ForEach(l => l.Delegate.DynamicInvoke(e));
            } catch (Exception Ex) {
                ServerManagement.Server.PrintToConsole($"Exception:  {Ex.Message}\nStacktrace: {Ex.StackTrace}\nSource:{Ex.Source}",ColorTranslator.FromHtml("#FC3539"));
            }
        }
    }
}
