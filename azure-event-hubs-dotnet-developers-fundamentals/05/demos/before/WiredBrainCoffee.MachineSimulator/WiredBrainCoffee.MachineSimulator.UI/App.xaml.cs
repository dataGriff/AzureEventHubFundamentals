using System.Windows;

namespace WiredBrainCoffee.MachineSimulator.UI
{
  public partial class App : Application
  {
    private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
      var ex = e.Exception;
      while (ex.InnerException != null)
      {
        ex = ex.InnerException;
      }

      MessageBox.Show(ex.Message);
    }
  }
}
