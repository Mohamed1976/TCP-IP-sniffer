using PcapDotNet.Packets.IpV4;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_IP_sniffer.Model
{
    public class IpV4ProtocolStats : INotifyPropertyChanged
    {
        #region [ Constructor ]

        public IpV4ProtocolStats(IpV4Protocol protocol)
        {
            Protocol = protocol;
            PacketCount = 0;
            ByteCount = 0;
        }

        #endregion

        #region [ Properties ]

        public IpV4Protocol Protocol { get; private set; }

        private long packetCount;
        public long PacketCount
        {
            get
            {
                return packetCount;
            }
            set
            {
                packetCount = value;
                NotifyPropertyChanged("PacketCount");
            }
        }

        private long byteCount;
        public long ByteCount
        {
            get
            {
                return byteCount;
            }
            set
            {
                byteCount = value;
                NotifyPropertyChanged("ByteCount");
            }
        }

        #endregion

        #region [ Event handlers ]

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
