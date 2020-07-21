using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace Vision
{
    public class PictureCollection : ObservableCollection<Picture>
    {
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

        public System.UInt16 ExifOrientation()
        {
            #region Nota sobre metadatos EXIF
            /**
            The 8 EXIF orientation values are numbered 1 to 8.

            1 = 0 degrees: the correct orientation, no adjustment is required.
            2 = 0 degrees, mirrored: image has been flipped back-to-front.
            3 = 180 degrees: image is upside down.
            4 = 180 degrees, mirrored: image is upside down and flipped back-to-front.
            5 = 90 degrees: image is on its side.
            6 = 90 degrees, mirrored: image is on its side and flipped back-to-front.
            7 = 270 degrees: image is on its far side.
            8 = 270 degrees, mirrored: image is on its far side and flipped back-to-front. || RotatedRightAndMirroredVertically
            */
            #endregion
            try
            {
                System.Windows.Media.Imaging.BitmapFrame frame = System.Windows.Media.Imaging.BitmapFrame.Create(new System.Uri(Path), System.Windows.Media.Imaging.BitmapCreateOptions.DelayCreation, System.Windows.Media.Imaging.BitmapCacheOption.Default);
                var bmData = (System.Windows.Media.Imaging.BitmapMetadata)frame.Metadata;
                if (bmData != null)
                {
                    object val = bmData.GetQuery("/app1/ifd/exif:{uint=274}");
                    if (val != null)
                    {
                        System.UInt16 exifrotation = 0;
                        if (System.UInt16.TryParse(val.ToString(), out exifrotation))
                        {
                            return exifrotation;
                        }
                    }
                }

                return 0;
            }
            catch(System.Exception)
            {
                return 0;
            }
        }
      
    }
}
