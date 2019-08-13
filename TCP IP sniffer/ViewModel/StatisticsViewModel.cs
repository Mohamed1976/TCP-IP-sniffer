using GalaSoft.MvvmLight;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TCP_IP_sniffer.Model;

namespace TCP_IP_sniffer.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        #region [ Consts ]

        private const int MaxNrOfDataPoints = 150;
        private const int UpdateInterval = 400;

        #endregion

        #region [ Constructor ]

        public StatisticsViewModel()
        {

            SetupPlotModel();
            timer.Interval = TimeSpan.FromMilliseconds(UpdateInterval);
            timer.Tick += UpdatePlotModel;
            timer.Start();
            this.watch.Start();
            ProtocolStats = StatHandler.ProtocolStats;
            ConnectionStats = StatHandler.ConnectionStats;
            prevTimeElapsed = 0;
            prevByteCount = 0;
        }

        #endregion

        #region [ Properties ]

        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Stopwatch watch = new Stopwatch();
        private long prevTimeElapsed;
        private long prevByteCount;

        public ObservableCollection<IpV4ProtocolStats> ProtocolStats { get; private set; }

        public ObservableCollection<IpV4ConnectionStats> ConnectionStats { get; private set; }

        public PlotModel RealTimePlot { get; private set; }


        private string capturingTime;
        public string CapturingTime
        {
            get
            {
                return capturingTime;
            }
            private set
            {
                capturingTime = value;
                RaisePropertyChanged("CapturingTime");
            }
        }

        private long packetCount;
        public long PacketCount
        {
            get
            {
                return packetCount;
            }
            private set
            {
                packetCount = value;
                RaisePropertyChanged("PacketCount");
            }
        }

        private long byteCount;
        public long ByteCount
        {
            get
            {
                return byteCount;
            }
            private set
            {
                byteCount = value;
                RaisePropertyChanged("ByteCount");
            }
        }

        #endregion

        #region [ Methods ]

        private void SetupPlotModel()
        {
            PlotModel plotModel = new PlotModel { Title = "tcp/ip traffic (kB/s)" };
            plotModel.Axes.Add(new LinearAxis { Title = "time (s)", Position = AxisPosition.Bottom });
            plotModel.Series.Add(new LineSeries { Title = "kilobytes", Smooth = false, LineStyle = LineStyle.Solid });
            this.RealTimePlot = plotModel;
        }

        private void UpdatePlotModel(object sender, EventArgs e)
        {
            long timeElapsed = StatHandler.StopWatch.ElapsedMilliseconds;

            if (timeElapsed - prevTimeElapsed > 0)
            {
                TimeSpan ts = StatHandler.StopWatch.Elapsed;
                CapturingTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                PacketCount = StatHandler.PacketCount;
                ByteCount = StatHandler.ByteCount;

                long diffByteCount = byteCount - prevByteCount > 0 ? byteCount - prevByteCount : 0;
                double kiloBytesPerSec = diffByteCount / (timeElapsed - prevTimeElapsed) * 0.001 * 1024;

                lock (this.RealTimePlot.SyncRoot)
                {
                    LineSeries bytesPerSecLine = (LineSeries)RealTimePlot.Series[0];
                    bytesPerSecLine.Points.Add(new DataPoint(timeElapsed * 0.001, kiloBytesPerSec));
                    if (bytesPerSecLine.Points.Count >= MaxNrOfDataPoints)
                        bytesPerSecLine.Points.RemoveAt(0);

                }
                this.RealTimePlot.InvalidatePlot(true);
                prevTimeElapsed = timeElapsed;
                prevByteCount = byteCount;
            }
        }

        #endregion
    }
}
