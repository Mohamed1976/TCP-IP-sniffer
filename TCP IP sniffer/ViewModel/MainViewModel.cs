using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Input;

namespace TCP_IP_sniffer.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region [ Constructor ]

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            HomeViewCmd = new RelayCommand(() => HomeViewCmdExecute());
            StatsViewCmd = new RelayCommand(() => StatsViewCmdExecute());
            AboutViewCmd = new RelayCommand(() => AboutViewCmdExecute());
            CurrentViewModel = snifferViewModel;
        }

        #endregion

        #region [ Fields ]

        private SnifferViewModel snifferViewModel = new SnifferViewModel();
        private StatisticsViewModel statisticsViewModel = new StatisticsViewModel();
        private AboutViewModel aboutViewModel = new AboutViewModel();

        #endregion


        #region [ Properties ]

        private ViewModelBase currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }
            set
            {
                currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        #endregion

        #region [ Commands ]

        public ICommand HomeViewCmd { get; private set; }

        private void HomeViewCmdExecute()
        {
            CurrentViewModel = snifferViewModel;
        }

        public ICommand StatsViewCmd { get; private set; }

        private void StatsViewCmdExecute()
        {
            CurrentViewModel = statisticsViewModel;
        }

        public ICommand AboutViewCmd { get; private set; }

        private void AboutViewCmdExecute()
        {
            CurrentViewModel = aboutViewModel;
        }

        #endregion
    }
}