using MediaBrowser.Model.ApiClient;
using MediaBrowser.Model.Logging;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Emby.ApiClient.Model;

namespace Emby.ApiClient.Net
{
    public class NetworkConnection : INetworkConnection
    {
        private readonly ILogger _logger;

        public NetworkConnection(ILogger logger)
        {
            _logger = logger;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        private void OnNetworkChange()
        {
            NetworkChanged?.Invoke(this, EventArgs.Empty);
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            OnNetworkChange();
        }

        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            OnNetworkChange();
        }

        public event EventHandler<EventArgs> NetworkChanged;
        
        public Task SendWakeOnLan(string macAddress, string ipAddress, int port, CancellationToken cancellationToken)
        {
            return SendWakeOnLan(macAddress, new IPEndPoint(IPAddress.Parse(ipAddress), port), cancellationToken);
        }

        public Task SendWakeOnLan(string macAddress, int port, CancellationToken cancellationToken)
        {
            return SendWakeOnLan(macAddress, new IPEndPoint(IPAddress.Broadcast, port), cancellationToken);
        }

        private async Task SendWakeOnLan(string macAddress, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            const int payloadSize = 102;

            var macBytes = PhysicalAddress.Parse(macAddress).GetAddressBytes();
            _logger.Debug(string.Format("Sending magic packet to {0}", macAddress));

            // Construct magic packet
            var payload = new byte[payloadSize];
            Buffer.BlockCopy(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 0, payload, 0, 6);

            for (var i = 1; i < 17; i++) 
            { 
                Buffer.BlockCopy(macBytes, 0, payload, 6 * i, 6);
            }

            // Send packet LAN
            using (var udp = new UdpClient())
            {
                udp.Connect(endPoint);

                cancellationToken.ThrowIfCancellationRequested();

                await udp.SendAsync(payload, payloadSize).ConfigureAwait(false);
            }
        }
        
        public NetworkStatus GetNetworkStatus()
        {
            return new NetworkStatus
            {
                IsNetworkAvailable = IsNetworkAvailable()
            };
        }

        /// <summary>
        /// Indicates whether any network connection is available
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNetworkAvailable()
        {
            return true;
            //return IsNetworkAvailable(0);
        }

        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (string.Equals(ni.Description, "Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }
    }
}
