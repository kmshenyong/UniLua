
using Serilog;

namespace UniLua.Tools
{
    // using Logger = DebugAssist;
    //  using Logger = UnityEngine.Debug;

    // thanks to dharco
    // refer to https://github.com/dharco/UniLua/commit/2854ddf2500ab2f943f01a6d3c9af767c092ce75
    public class ULDebug
    {
        public static System.Action<object> Log = NoAction;
        public static System.Action<object> LogError = NoAction;

        private static void NoAction(object msg) { }

        static ULDebug()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                          .MinimumLevel.Debug()
                          .WriteTo.Debug()
                          .CreateLogger();
            Log = (ob) =>
            {
                Serilog.Log.Information("LUA:" + (String)ob);
            };
            LogError = (ob) =>
            {
                Serilog.Log.Error("LUA:" + (String)ob);
            };

        }
    }
}

