using System.Runtime.InteropServices;

namespace MSLabs.WpfApp.Helpers
{
    public class InternetAvailability
    {
        public static bool GetIsInternetAvailable()
        {
            return InternetGetConnectedState(out int description, 0);
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);
    }
}
