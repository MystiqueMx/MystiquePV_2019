using System.ComponentModel;

namespace MystiqueNative.Helpers
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public bool IsBusy { get => _isBusy; set { _isBusy = value; OnPropertyChanged("IsBusy"); } }
        private bool _isBusy;
        public bool ErrorStatus { get => _errorStatus; set { _errorStatus = value; OnPropertyChanged("ErrorStatus"); } }
        private bool _errorStatus;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged("ErrorMessage"); } }
        private string _errorMessage;
    }
}
