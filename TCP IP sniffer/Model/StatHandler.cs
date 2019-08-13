using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_IP_sniffer.Model
{
    public static class StatHandler
    {
        #region [ Constructor ]

        static StatHandler()
        {
            StopWatch = new Stopwatch();
            PacketCount = 0;
            ByteCount = 0;
            ProtocolStats = new ObservableCollection<IpV4ProtocolStats>()
            {
                new IpV4ProtocolStats( IpV4Protocol.Tcp),
                new IpV4ProtocolStats( IpV4Protocol.Udp),
                new IpV4ProtocolStats( IpV4Protocol.InternetControlMessageProtocol),
                new IpV4ProtocolStats( IpV4Protocol.InternetGroupManagementProtocol),
            };
            ConnectionStats = new ObservableCollection<IpV4ConnectionStats>();

        }

        #endregion

        #region [ Properties ]

        public static Stopwatch StopWatch { get; private set; }

        public static long PacketCount { get; private set; }

        public static long ByteCount { get; private set; }

        public static ObservableCollection<IpV4ProtocolStats> ProtocolStats { get; private set; }

        public static ObservableCollection<IpV4ConnectionStats> ConnectionStats { get; private set; }

        #endregion

        #region [ Methods ]

        public static void Start()
        {
            StopWatch.Start();
            PacketCount = 0;
            ByteCount = 0;
            ConnectionStats.Clear();

            foreach (IpV4ProtocolStats stat in ProtocolStats)
            {
                stat.ByteCount = 0;
                stat.PacketCount = 0;
            }
        }

        public static void Stop()
        {
            StopWatch.Stop();
        }

        public static void UpdateStats(Packet newPacket)
        {
            ByteCount += newPacket.Length;
            PacketCount++;

            /* Update IpV4ProtocolStats */
            if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.Tcp)
            {
                ProtocolStats[0].PacketCount++;
                ProtocolStats[0].ByteCount += newPacket.Length;
            }
            else if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.Udp)
            {
                ProtocolStats[1].PacketCount++;
                ProtocolStats[1].ByteCount += newPacket.Length;
            }
            else if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.InternetControlMessageProtocol)
            {
                ProtocolStats[2].PacketCount++;
                ProtocolStats[2].ByteCount += newPacket.Length;
            }
            else if (newPacket.Ethernet.IpV4.Protocol == IpV4Protocol.InternetGroupManagementProtocol)
            {
                ProtocolStats[3].PacketCount++;
                ProtocolStats[3].ByteCount += newPacket.Length;
            }

            /* Update IpV4ConnectionStats */
            IpV4ConnectionStats connStats = ConnectionStats.Where(c =>
            (c.AddressA == newPacket.Ethernet.IpV4.Source || c.AddressA == newPacket.Ethernet.IpV4.Destination) &&
            (c.AddressB == newPacket.Ethernet.IpV4.Source || c.AddressB == newPacket.Ethernet.IpV4.Destination)).FirstOrDefault();
            if (connStats == null)
            {
                connStats = new IpV4ConnectionStats(newPacket.Ethernet.IpV4.Source, newPacket.Ethernet.IpV4.Destination);
                connStats.ByteCountAToB = newPacket.Length;
                connStats.PacketCountAToB++;
                ConnectionStats.Add(connStats);
            }
            else
            {
                if (connStats.AddressA == newPacket.Ethernet.IpV4.Source)
                {
                    connStats.PacketCountAToB++;
                    connStats.ByteCountAToB += newPacket.Length;
                }
                else
                {
                    connStats.PacketCountBToA++;
                    connStats.ByteCountBToA += newPacket.Length;
                }
            }
        }

        #endregion
    }
}
