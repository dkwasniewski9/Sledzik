using System.ServiceProcess;
namespace SledzikService
{
    static class Program
    {
        static void Main()
        {
            ServiceBase.Run(new SledzikService());
        }
    }
}