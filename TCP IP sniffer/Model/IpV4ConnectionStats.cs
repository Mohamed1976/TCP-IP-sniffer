using PcapDotNet.Packets.IpV4;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCP_IP_sniffer.Model
{
    public class IpV4ConnectionStats
    {
        #region [ Constructor ]

        public IpV4ConnectionStats(IpV4Address addressA, IpV4Address addressB)
        {
            AddressA = addressA;
            AddressB = addressB;
            PacketCountAToB = 0;
            PacketCountBToA = 0;
            ByteCountAToB = 0;
            ByteCountBToA = 0;
        }

        #endregion

        #region [ Properties ]

        public IpV4Address AddressA { get; private set; }
        public IpV4Address AddressB { get; private set; }

        private long packetCountAToB;
        public long PacketCountAToB
        {
            get
            {
                return packetCountAToB;
            }
            set
            {
                packetCountAToB = value;
                NotifyPropertyChanged("PacketCountAToB");
            }
        }

        private long packetCountBToA;
        public long PacketCountBToA
        {
            get
            {
                return packetCountBToA;
            }
            set
            {
                packetCountBToA = value;
                NotifyPropertyChanged("PacketCountBToA");
            }
        }

        private long byteCountAToB;
        public long ByteCountAToB
        {
            get
            {
                return byteCountAToB;
            }
            set
            {
                byteCountAToB = value;
                NotifyPropertyChanged("ByteCountAToB");
            }
        }

        private long byteCountBToA;
        public long ByteCountBToA
        {
            get
            {
                return byteCountBToA;
            }
            set
            {
                byteCountBToA = value;
                NotifyPropertyChanged("ByteCountBToA");
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
