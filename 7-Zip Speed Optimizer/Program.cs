global using static SevenZipSpeedOptimizer.Global;
using System;
using System.Windows.Forms;

namespace SevenZipSpeedOptimizer;

file static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Logger.SetLogFile(Paths.LogFile);
        Logger.LogStartup(Application.ProductVersion + " Started session" + $"{NL}" +
                          "-------------------------------------------------------");

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new AppContext());
    }

    private sealed class AppContext : ApplicationContext
    {
        internal AppContext() => Core.Init();
    }
}
