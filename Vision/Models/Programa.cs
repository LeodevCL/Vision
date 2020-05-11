
using System.Collections.ObjectModel;
namespace Vision
{
    public class Programas : ObservableCollection<Programa>
    {

    }

    public class Programa : IGeneric
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            { 
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set 
            {
                _path = value;
                RaisePropertyChanged("Path");
            }
        }

        public Programa()
        {

        }

        public Programa(string id, string name, string path)
        {
            Id = id;
            Name = name;
            Path = path;
        }

        public override string ToString()
        {
            return System.IO.Path.GetFileNameWithoutExtension(Name);
        }
    }
}
