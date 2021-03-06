﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Forestual2ServerCS.Internal;

namespace Forestual2ServerCS.Management
{
    public class TraceManager
    {
        private static readonly string TracePath = Path.Combine(Application.StartupPath, "Traces");
        private static DateTime SessionStart;

        public static void Log(string content, string prefix) {
            if (Server.Config.TracingEnabled) {
                var TraceFile = DateTime.Today.ToShortDateString().Replace(".", "-") + ".trace";
                if (!File.Exists(Path.Combine(TracePath, TraceFile))) {
                    File.Create(Path.Combine(TracePath, TraceFile));
                }
                var Trace = File.ReadAllLines(Path.Combine(TracePath, TraceFile)).ToList();

                Trace.Add($"{DateTime.Now.ToLongTimeString()} [{prefix}] >> {content}");

                File.WriteAllLines(Path.Combine(TracePath, TraceFile), Trace);
            }
        }

        public static void StartSession() {
            if (Server.Config.TracingEnabled) {
                SessionStart = DateTime.Now;

                var TraceFile = DateTime.Today.ToShortDateString().Replace(".", "-") + ".trace";
                if (!File.Exists(Path.Combine(TracePath, TraceFile))) {
                    File.WriteAllText(Path.Combine(TracePath, TraceFile), "");
                }

                var Trace = File.ReadAllLines(Path.Combine(TracePath, TraceFile)).ToList();

                Trace.Add("=========== Session Begin ===========");
                Trace.Add($"Version: F2S {new Version().ToMediumString()}");
                Trace.Add($"CoreVersion: F2C {new F2Core.Compatibility.Version().ToMediumString()}");
                Trace.Add($"Start: {SessionStart.ToLongTimeString()}");
                Trace.Add("=====================================");
                Trace.Add("");

                File.WriteAllLines(Path.Combine(TracePath, TraceFile), Trace);
            }
        }

        public static void EndSession() {
            if (Server.Config.TracingEnabled) {
                var SessionEnd = DateTime.Now;

                var TraceFile = DateTime.Today.ToShortDateString().Replace(".", "-") + ".trace";
                if (!File.Exists(Path.Combine(TracePath, TraceFile))) {
                    File.Create(Path.Combine(TracePath, TraceFile));
                }
                var Trace = File.ReadAllLines(Path.Combine(TracePath, TraceFile)).ToList();

                Trace.Add("");
                Trace.Add("============ Session End ============");
                Trace.Add($"Duration: {SessionEnd.Subtract(SessionStart)}");
                Trace.Add("=====================================");
                Trace.Add("");

                File.WriteAllLines(Path.Combine(TracePath, TraceFile), Trace);
            }
        }
    }
}
