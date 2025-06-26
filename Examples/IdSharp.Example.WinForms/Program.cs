namespace IdSharp.Tagging.Harness.WinForms;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.ThreadException += Application_ThreadException;
        Application.Run(new MainForm());
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        _ = MessageBox.Show(e.Exception.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}