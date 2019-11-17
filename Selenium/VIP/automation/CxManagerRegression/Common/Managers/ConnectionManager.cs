using System.Diagnostics;

namespace Common.Managers
{
    /// <summary>
    /// Manages network connections
    /// </summary>
    public class ConnectionManager
    {
        /// <summary>
        /// Flag that shows is this PC connected to LAN
        /// </summary>
        public bool IsConnected;

        /// <summary>
        /// Disconnects Ethernet and WLAN cards from network
        /// </summary>
        public void DisconnectNetwork()
        {
            using (var process = new Process())
            {
                var procInfo = new ProcessStartInfo("cmd.exe", " /C ipconfig /release")
                {
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                process.StartInfo = procInfo;
                process.Start();
                process.WaitForExit();
                procInfo = new ProcessStartInfo("cmd.exe", " /C ipconfig /release")
                {
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                process.StartInfo = procInfo;
                process.Start();
                process.WaitForExit();
                IsConnected = false;
            }
        }

        /// <summary>
        /// Connects default network interface to LAN 
        /// </summary>
        public void ConnectNetwork()
        {
            using (var process = new Process())
            {
                var procInfo = new ProcessStartInfo("cmd.exe", " /C ipconfig /renew")
                {
                    WindowStyle = ProcessWindowStyle.Hidden
                };         
                process.StartInfo = procInfo;
                process.Start();
                process.WaitForExit();
                IsConnected = true;
            }
        }
    }
}
