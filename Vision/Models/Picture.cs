using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace Vision
{
    public class PictureCollection : ObservableCollection<Picture>
    {
        //private FileInfo _firstImage;
        //public FileInfo FirstImage
        //{
        //    get { return _firstImage; }
        //    set { _firstImage = value; }
        //}

        private Picture _firstImage;
        public Picture FirstImage
        {
            get { return _firstImage; }
            set { _firstImage = value; }
        }

        public Picture Primera
        {
            get { return this[0]; }
        }

        public Picture Anterior(Picture actual)
        {
            if (actual.Indice == Primera.Indice)
            {
                return actual;
            }
            return this[actual.Indice - 1];
        }

        public Picture Siguiente(Picture actual)
        {
            if(actual.Indice == Ultima.Indice)
            {
                return actual;
            }
            return this[actual.Indice + 1];
        }

        public Picture Ultima
        {
            get { return this[this.Count - 1]; }
        }
    }

    public class Picture : IGeneric
    {
        // Borde - Image - Resolución - Ruta - Directorio - Peso - Indice

        
        private FileInfo _info;
        public FileInfo Info
        {
            get { return _info; }
            set
            {
                _info = value;
                RaisePropertyChanged("Info");
                RaisePropertyChanged("Directorio");
                RaisePropertyChanged("Path");
                RaisePropertyChanged("Nombre");
                RaisePropertyChanged("Imagen");
                RaisePropertyChanged("Ancho");
                RaisePropertyChanged("Alto");
            }
        }

        /// <summary>
        /// Devuelve la ruta de acceso completa al archivo incluyendo directorio, nombre y extensión
        /// </summary>
        public string Path
        {
            get { return Info.FullName; }
        }

        /// <summary>
        /// Devuelve la ruta de acceso al directorio que contiene el archivo
        /// </summary>
        public string Directorio
        {
            get { return Info.DirectoryName; }
        }

        /// <summary>
        /// Devuelve el nombre completo del archivo incluyendo extensión
        /// </summary>
        public string Nombre
        {
            get { return Info.Name; }
        }
        
        /// <summary>
        /// Devuelve la extensión del archivo
        /// </summary>
        public string Extension
        {
            get { return System.IO.Path.GetExtension(Path); }
        }

        /// <summary>
        /// Devuelve el peso del archivo en bytes
        /// </summary>
        public long Peso
        {
            get { return Info.Length; }
        }

        public System.Drawing.Image Imagen
        {
            get { return System.Drawing.Image.FromFile(Path); }
        }

        /// <summary>
        /// Devuelve el ancho en pixeles del archivo de imagen
        /// </summary>
        public int Ancho
        {
            get { return Imagen.Width; }
        }

        /// <summary>
        /// Devuelve el alto en pixeles del archivo de imagen
        /// </summary>
        public int Alto
        {
            get { return Imagen.Height; }
        }

        /// <summary>
        /// Devuelve el index del archivo dentro de la colección
        /// </summary>
        private int _indice;
        public int Indice
        {
            get { return _indice; }
            set 
            {
                _indice = value;
                RaisePropertyChanged("Indice");
            }
        }

        public bool PrimeraImagen
        {
            get { return Indice == 0; }
        }

        public Picture()
        {

        }

        public Picture(FileInfo info, int indice)
        {
            Info = info;
            Indice = indice;
        }

      
    }
}
