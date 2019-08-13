using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.Ethernet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TCP_IP_sniffer.Model;

namespace TCP_IP_sniffer.ViewModel
{
    public class SnifferViewModel : ViewModelBase
    {
        #region [ Constructors ]

        public SnifferViewModel()
        {
            Packets = new ObservableCollection<Packet>();
            InterfaceList = new ObservableCollection<IPacketDevice>();
            StartCaptureCmd = new RelayCommand(() => StartCaptureCmdExecute());
            StopCaptureCmd = new RelayCommand(() => StopCaptureCmdExecute());
            RefreshInterfaceList();
            SelectedPacket = null;
            SelectedInterface = null;
            TrafficFilter = string.Empty;
            IsBusy = false;
            packetReceiveThread = null;
            cts = null;
            communicator = null;
        }

        #endregion

        #region [ Properties ]

        private Thread packetReceiveThread;
        private CancellationTokenSource cts;
        private PacketCommunicator communicator;

        private IPacketDevice selectedInterface;
        public IPacketDevice SelectedInterface
        {
            get
            {
                return selectedInterface;
            }
            set
            {
                selectedInterface = value;
                RaisePropertyChanged("SelectedInterface");
            }
        }

        private string trafficFilter;
        public string TrafficFilter
        {
            get
            {
                return trafficFilter;
            }
            set
            {
                trafficFilter = value;
                RaisePropertyChanged("TrafficFilter");
            }
        }

        public ObservableCollection<IPacketDevice> InterfaceList { get; private set; }

        private Packet selectedPacket;
        public Packet SelectedPacket
        {
            get
            {
                return selectedPacket;
            }
            set
            {
                selectedPacket = value;
                RaisePropertyChanged("SelectedPacket");
            }
        }

        ObservableCollection<Packet> packets;
        public ObservableCollection<Packet> Packets
        {
            get
            {
                return packets;
            }

            private set
            {
                packets = value;
            }
        }

        bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            private set
            {
                isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        #endregion

        #region [ Methods ]

        private void RefreshInterfaceList()
        {
            // Retrieve the interfaces list
            IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;

            // Scan the list printing every entry
            for (int i = 0; i < allDevices.Count(); i++)
                InterfaceList.Add(allDevices[i]);
        }

        #endregion

        #region [ Commands ]

        public ICommand StartCaptureCmd { get; private set; }

        private void StartCaptureCmdExecute()
        {
            if (packetReceiveThread != null)
            {
                packetReceiveThread.Abort();
                packetReceiveThread = null;
            }

            Packets.Clear();
            packetReceiveThread = new Thread(new ThreadStart(PacketHandler));
            packetReceiveThread.Priority = ThreadPriority.AboveNormal;
            packetReceiveThread.Start();
            StatHandler.Start();
        }

        public void PacketHandler()
        {
            Packet packet;

            try
            {
                IsBusy = true;
                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;

                // Open the device
                communicator = SelectedInterface.Open(65536,        // 65536 guarantees that the whole packet will be captured on all the link layers
                    PacketDeviceOpenAttributes.Promiscuous,         // promiscuous mode
                    1000);                                          // read timeout

                if (!string.IsNullOrEmpty(trafficFilter))
                {
                    communicator.SetFilter(trafficFilter);
                }

                do
                {
                    PacketCommunicatorReceiveResult result = communicator.ReceivePacket(out packet);
                    switch (result)
                    {
                        case PacketCommunicatorReceiveResult.Timeout:
                            // Timeout elapsed
                            continue;
                        case PacketCommunicatorReceiveResult.Ok:
                            if (packet.Ethernet.EtherType == EthernetType.IpV4)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    packets.Add(packet);
                                    StatHandler.UpdateStats(packet);
                                });
                            }
                            break;
                        default:
                            throw new InvalidOperationException("PacketCommunicator InvalidOperationException");
                    }
                } while (!token.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                MessageBox.Show("A handled exception occurred: " + ex.Message, "Tcp/Ip sniffer",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                communicator.Break();
                communicator.Dispose();
                communicator = null;

                cts.Dispose();
                cts = null;
                IsBusy = false;
            }
        }

        public ICommand StopCaptureCmd { get; private set; }

        private void StopCaptureCmdExecute()
        {
            if (cts != null)
                cts.Cancel();

            StatHandler.Stop();
        }

        #endregion
    }
}
