using System.ComponentModel;

namespace Vision
{
    public class IEnlace
    {

    }

    public class IGeneric : IEnlace, INotifyPropertyChanged
    {

        #region Implementación de INotifyPropertyChanged
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
