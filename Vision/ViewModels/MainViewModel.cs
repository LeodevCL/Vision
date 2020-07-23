using MetadataExtractor;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Vision.Views;

namespace Vision.ViewModels
{
    public class MainViewModel : IGeneric
    {
        private PictureCollection _pictures = new PictureCollection();
        public PictureCollection Pictures
        {
            get { return _pictures; }
            set
            {
                _pictures = value;
                RaisePropertyChanged("Pictures");
                RaisePropertyChanged("CurrentPicture");
                RaisePropertyChanged("CanExecute");
                RaisePropertyChanged("CanMoveFirst");
                RaisePropertyChanged("CanMovePrev");
                RaisePropertyChanged("CanMoveNext");
                RaisePropertyChanged("CanMoveLast"); 
            }
        }

        private bool _loadSuccessfully = false;
        public bool LoadSuccessfully
        {
            get { return _loadSuccessfully; }
            set { _loadSuccessfully = value; }
        }

        private Picture _currentPicture;
        public Picture CurrentPicture
        {
            get { return _currentPicture; }
            set
            {
                ResetAllMods();
                _currentPicture = value;
                RaisePropertyChanged("CurrentPicture");
                RaisePropertyChanged("LogoVision");
                RaisePropertyChanged("CanRotate");
                NormalizeExifRotation(value.ExifOrientation);
                ReadMetadata();
            }
        }

        private ObservableCollection<string> _exifTags = new ObservableCollection<string>();
        public ObservableCollection<string> ExifTags
        {
            get { return _exifTags; }
            set
            {
                _exifTags = value;
                RaisePropertyChanged("ExifTags");
            }
        } 

        public bool LogoVision
        {
            get
            {
                return CurrentPicture != null ? false : true;
            }
        }

        private bool _ctrlLeftVision = false;
        public bool CtrlLeftVision
        {
            get { return _ctrlLeftVision; }
            set
            {
                _ctrlLeftVision = value;
                RaisePropertyChanged("CtrlLeftVision");
            }
        }

        private bool _ctrlRightVision = true;
        public bool CtrlRightVision
        {
            get { return _ctrlRightVision; }
            set
            {
                _ctrlRightVision = value;
                RaisePropertyChanged("CtrlRightVision");
            }
        }

        private bool _barVision = false;
        public bool BarVision
        {
            get { return _barVision; }
            set
            {
                _barVision = value;
                RaisePropertyChanged("BarVision");
            }
        }

        private bool _commandBarVision = true;
        public bool CommandBarVision
        {
            get { return _commandBarVision; }
            set
            {
                _commandBarVision = value;
                RaisePropertyChanged("CommandBarVision");
            }
        }

        private bool _imageOnlyVision = false;
        public bool ImageOnlyVision
        {
            get { return _imageOnlyVision; }
            set
            {
                _imageOnlyVision = value;
                RaisePropertyChanged("ImageOnlyVision");
            }
        }

        private bool _exifPanelVision = false;
        public bool ExifPanelVision
        {
            get { return _exifPanelVision; }
            set
            {
                _exifPanelVision = value;
                RaisePropertyChanged("ExifPanelVision");
            }
        }

        private bool _presentationRunning = false;
        public bool PresentationRunning
        {
            get { return _presentationRunning; }
            set
            { 
                _presentationRunning = value;
                RaisePropertyChanged("PresentationRunning");
                CommandBarVision = !value;
            }
        }

        private double _listOpacity = 100;
        public double ListOpacity
        {
            get { return _listOpacity; }
            set 
            { 
                _listOpacity = value;
                RaisePropertyChanged("ListOpacity");
            }
        }

        private ZoomBorder _border;
        public ZoomBorder Border
        {
            get { return _border; }
            set 
            {
                _border = value;
                RaisePropertyChanged("Border");
                RaisePropertyChanged("Imagen");
                RaisePropertyChanged("Transform");
                RaisePropertyChanged("CanExecute");
                RaisePropertyChanged("CanRotate");
            }
        }

        public System.Windows.Controls.Image Imagen
        {
            get { return Border == null? null : (System.Windows.Controls.Image)_border.Child; }
        }

        public RotateTransform Transform
        {
            get { return Border == null ? null : (RotateTransform)Border.RenderTransform;  }
        }
         

        #region ICommands

        private ICommand _loadBorderCommand;
        public ICommand LoadBorderCommand
        {
            get
            {
                if (_loadBorderCommand == null)
                    _loadBorderCommand = new ParamCommand(new Action<object>(LoadBorder));
                return _loadBorderCommand;
            }
        }

        private ICommand _openFileCommand;
        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                    _openFileCommand = new RelayCommand(new Action(OpenFile));
                return _openFileCommand;
            }
        }

        private ICommand _printFileCommand;
        public ICommand PrintFileCommand
        {
            get
            {
                if (_printFileCommand == null)
                    _printFileCommand = new RelayCommand(new Action(PrintFile), () => CanExecute);
                return _printFileCommand;
            }
        }

        private ICommand _saveAsCommand;
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                    _saveAsCommand = new RelayCommand(new Action(SaveAs), () => CanExecute);
                return _saveAsCommand;
            }
        }

        private ICommand _openFileInFolderCommand;
        public ICommand OpenFileInFolderCommand
        {
            get
            {
                if (_openFileInFolderCommand == null)
                    _openFileInFolderCommand = new RelayCommand(new Action(OpenFileInFolder), () => CanExecute);
                return _openFileInFolderCommand;
            }
        }

        private ICommand _moverPrimeraCommand;
        public ICommand MoverPrimeraCommand
        {
            get
            {
                if (_moverPrimeraCommand == null)
                    _moverPrimeraCommand = new RelayCommand(new Action(MoverPrimera), () => CanMoveFirst); //Can? es la 1ra esta?
                return _moverPrimeraCommand;
            }
        }

        private ICommand _moverAnteriorCommand;
        public ICommand MoverAnteriorCommand
        {
            get
            {
                if (_moverAnteriorCommand == null)
                    _moverAnteriorCommand = new RelayCommand(new Action(MoverAnterior), () => CanMovePrev); //Can? hay anterior?
                return _moverAnteriorCommand;
            }
        }

        private ICommand _presentacionCommand;
        public ICommand PresentacionCommand
        {
            get
            {
                if (_presentacionCommand == null)
                    _presentacionCommand = new RelayCommand(new Action(Presentacion), () => CanExecute);
                return _presentacionCommand;
            }
        }

        private ICommand _moverSiguienteCommand;
        public ICommand MoverSiguienteCommand
        {
            get
            {
                if (_moverSiguienteCommand == null)
                    _moverSiguienteCommand = new RelayCommand(new Action(MoverSiguiente), () => CanMoveNext); //hay sgte?
                return _moverSiguienteCommand;
            }
        }

        private ICommand _moverUltimaCommand;
        public ICommand MoverUltimaCommand
        {
            get
            {
                if (_moverUltimaCommand == null)
                    _moverUltimaCommand = new RelayCommand(new Action(MoverUltima), () => CanMoveLast); //noe s esta la ultima?
                return _moverUltimaCommand;
            }
        }

        private ICommand _zoomOutCommand;
        public ICommand ZoomOutCommand
        {
            get
            {
                if (_zoomOutCommand == null)
                    _zoomOutCommand = new RelayCommand(new Action(ZoomOut), () => CanExecute);
                return _zoomOutCommand;
            }
        }

        private ICommand _zoomInCommand;
        public ICommand ZoomInCommand
        {
            get
            {
                if (_zoomInCommand == null)
                    _zoomInCommand = new RelayCommand(new Action(ZoomIn), () => CanExecute);
                return _zoomInCommand;
            }
        }

        private ICommand _rotateLeftCommand;
        public ICommand RotateLeftCommand
        {
            get
            {
                if (_rotateLeftCommand == null)
                    _rotateLeftCommand = new RelayCommand(new Action(RotateLeft), () => CanRotate);
                return _rotateLeftCommand;
            }
        }

        private ICommand _rotateRightCommand;
        public ICommand RotateRightCommand
        {
            get
            {
                if (_rotateRightCommand == null)
                    _rotateRightCommand = new RelayCommand(new Action(RotateRight), () => CanRotate);
                return _rotateRightCommand;
            }
        }

        private ICommand _applyBlurCommand;
        public ICommand ApplyBlurCommand
        {
            get
            {
                if (_applyBlurCommand == null)
                    _applyBlurCommand = new RelayCommand(new Action(ApplyBlur), () => CanExecute);
                return _applyBlurCommand;
            }
        }

        private ICommand _openAndSelectCommand;
        public ICommand OpenAndSelectCommand
        {
            get
            {
                if (_openAndSelectCommand == null)
                    _openAndSelectCommand = new RelayCommand(new Action(OpenAndSelect), () => CanExecute);
                return _openAndSelectCommand;
            }
        }

        private ICommand _showSettingsCommand;
        public ICommand ShowSettingsCommand
        {
            get
            {
                if (_showSettingsCommand == null)
                    _showSettingsCommand = new RelayCommand(new Action(ShowSettings));
                return _showSettingsCommand;
            }
        }

        private ICommand _listProgramsCommand;
        public ICommand ListProgramsCommand
        {
            get
            {
                if (_listProgramsCommand == null)
                    _listProgramsCommand = new RelayCommand(new Action(ListPrograms), () => CanExecute);
                return _listProgramsCommand;
            }
        }

        private ICommand _showDebugInfoCommand;
        public ICommand ShowDebugInfoCommand
        {
            get
            {
                if (_showDebugInfoCommand == null)
                    _showDebugInfoCommand = new RelayCommand(new Action(ShowDebugInfo));
                return _showDebugInfoCommand;
            }
        }

        #endregion

        #region Condicionantes de ICommands
        private bool CanExecute
        {
            get 
            {  
                if (ImageOnlyVision)
                {
                    if (Pictures.FirstImage != null)
                    {
                        return true;
                    }
                }
                else
                {
                    if (Pictures.Count > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool CanMoveFirst
        {
            get { return Pictures.Count > 0 && CurrentPicture.Indice != 0; }
        }

        private bool CanMovePrev
        {
            get { return Pictures.Count > 0 && CurrentPicture.Indice > 0; }
        }

        private bool CanMoveNext
        {
            get { return Pictures.Count > 0 && CurrentPicture.Indice < Pictures.Count - 1; }
        }

        private bool CanMoveLast
        {
            get { return Pictures.Count > 0 && CurrentPicture.Indice != Pictures.Count - 1; }
        }

        private bool CanRotate
        {
            get 
            {
                if (ImageOnlyVision)
                {
                    if (Pictures.FirstImage != null && Transform != null)
                    {
                        return true;
                    }
                }
                else
                {
                    if (Pictures.Count > 0 && Transform != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region ShowDebugInfo
        private void ShowDebugInfo()
        {
            string mensaje = Pictures.Count > 0 ? "Hay " + Pictures.Count + " items en la colección, y el primer item apunta a " + Pictures[0].Path : "No hay imagenes cargadas en la colección Pictures";
            MessageBox.Show(mensaje, "Información de depuración", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region LoadBorder
        private void LoadBorder(object obj)
        {
            if (obj != null)
            {
                Border = (ZoomBorder)obj;
            }
        }

        #endregion

        #region OpenFile
        private void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.gif;*.jfif;*.png;*.bmp;*.emf;*.wmf;*.ico;*.exif;*.tiff;*.tif;*.webp|Todos los archivos (*.*)|*.*";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IniciarCarga(dialog.FileName);                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        public void IniciarCarga(string filename)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (File.Exists(filename))
                {
                    CtrlLeftVision = true;
                    Pictures.FirstImage = new Picture(new FileInfo(filename), -1);
                    CargarArchivosCompatibles();
                }
            }));
        }

        private void CargarArchivosCompatibles()
        {
            Task tsk = Task.Run(async () =>
            {
                await Task.Run(() => LimpiarColecciones());
                await Task.Run(() => CargarPrimeraImagen());
                if (SettingsManager.Load("OnlySelected") != 1)
                {
                    await Task.Run(() => CargarImagenesCompatibles());
                    ImageOnlyVision = false;
                }
                else
                {
                    ImageOnlyVision = true;
                }
            });
        }

        private void LimpiarColecciones()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Pictures.Clear();
            }));
        }

        private void CargarPrimeraImagen()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Border.Reset();
                        CurrentPicture = Pictures.FirstImage;
                    }));
        }

        private void CargarImagenesCompatibles()
        {
            try
            {
                Task tsk = Task.Run(async () =>
                {
                    List<FileInfo> lista = await Task.Run(() => ListarImagenesCompatibles());
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        for (int i = 0; i < lista.Count; i++)
                        {
                            Pictures.Add(new Picture(lista[i], i));
                            if (CurrentPicture.Path.Equals(lista[i].FullName))
                            {
                                CurrentPicture = Pictures[i];
                            }
                        }
                    }));
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } 
        }

        private List<FileInfo> ListarImagenesCompatibles()
        {
            try
            {
                string folder = Pictures.FirstImage.Info.Directory.FullName;
                List<FileInfo> files = new DirectoryInfo(Pictures.FirstImage.Info.Directory.FullName)
                    .GetFiles("*.*", SearchOption.TopDirectoryOnly)
                    .Where(s => s.Extension.MatchesWith(".jpg", ".jpeg", ".gif", ".jfif", ".png", ".bmp", ".emf", ".wmf", ".ico", ".exif", ".tiff", ".tif", ".webp"))
                    .ToList<FileInfo>();

                files.Sort(new WindowsFileNameComparer());
                return SettingsManager.Load("IgnoreHidden") == 1 ? files.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)).ToList() : files;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error cargando archivos", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return new List<FileInfo>();
        }

        #endregion

        #region PrintFile
        private void PrintFile()
        {
            ImprimirMetodo1();
        }

        private void ImprimirMetodo1()
        {
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(CurrentPicture.Path);
            info.Verb = "Print";
            info.CreateNoWindow = true;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process.Start(info);
        }

        #endregion

        #region SaveAs

        private void SaveAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = CurrentPicture.Extension;
            dialog.FileName = CurrentPicture.Info.Name;
            dialog.Title = "Seleccione ruta de guardado";
            dialog.Filter = "Portable Network Graphics PNG |*.png|Joint Photographic Group JPG |*.jpg|Joint Photographic Experts Group JPEG |*.jpeg|Tagged Image File Format TIFF |*.tiff|Graphics Interchange Format GIF |*.gif|Bits Maps Protocole BMP |*.bmp|Enhanced Windows Metafile Picture EMF |*.emf|Exchangeable Image File Format  EXIF |*.exif";
            UseDefaultExtAsFilterIndex(dialog);
            if (dialog.ShowDialog() == true)
            {
                //Si la imagen original y la que quiero guardar son del mismo formato, simplemente copio
                if (CurrentPicture.Extension.Equals(Path.GetExtension(dialog.FileName)))
                {
                    CopiarArchivo(CurrentPicture.Path, dialog.FileName);
                }
                else //si no eran del mismo formato entonces lo convierto normalmente.
                {
                    switch (Path.GetExtension(dialog.FileName).ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            GuardarImagenComo(CurrentPicture.Path, dialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case ".png":
                            GuardarImagenComo(CurrentPicture.Path, dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case ".tiff":
                            GuardarImagenComo(CurrentPicture.Path, dialog.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                            break;
                        case ".gif":
                            GuardarImagenComo(CurrentPicture.Path, dialog.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        case ".bmp":
                            GuardarImagenComo(CurrentPicture.Path, dialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        case ".emf":
                            GuardarImagenComo(CurrentPicture.Path, dialog.FileName, System.Drawing.Imaging.ImageFormat.Emf);
                            break;
                        case ".exif":
                            GuardarImagenComo(CurrentPicture.Path, dialog.FileName, System.Drawing.Imaging.ImageFormat.Exif);
                            break;
                        //case ".ico":
                        //    if (img.Width > 48 || img.Height > 48) // 256 si uso el método mejorado
                        //    {
                        //        MessageBox.Show("Los íconos no pueden tener una resolución mayor a 48x48 píxeles", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        //    }
                        //    else
                        //    {
                        //        GuardarImagenComo(info.FullName, dialog.FileName, System.Drawing.Imaging.ImageFormat.Icon);
                        //    }
                        //    break;
                        default:
                            throw new ArgumentOutOfRangeException(Path.GetExtension(dialog.FileName));
                    }
                }
            }
        }

        private void UseDefaultExtAsFilterIndex(FileDialog dialog)
        {
            var ext = "*." + dialog.DefaultExt;
            var filter = dialog.Filter;
            var filters = filter.Split('|');
            for (int i = 1; i < filters.Length; i += 2)
            {
                if (filters[i] == ext)
                {
                    dialog.FilterIndex = 1 + (i - 1) / 2;
                    return;
                }
            }
        }

        private void CopiarArchivo(string from_file, string to_file)
        {
            try
            {
                System.IO.File.Copy(from_file, to_file, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GuardarImagenComo(string from_file, string to_file, System.Drawing.Imaging.ImageFormat format)
        {
            System.Drawing.Image bmpImageToConvert = System.Drawing.Image.FromFile(from_file);
            System.Drawing.Image bmpNewImage = new System.Drawing.Bitmap(bmpImageToConvert.Width, bmpImageToConvert.Height);
            System.Drawing.Graphics gfxNewImage = System.Drawing.Graphics.FromImage(bmpNewImage);
            gfxNewImage.DrawImage(bmpImageToConvert, new System.Drawing.Rectangle(0, 0, bmpNewImage.Width, bmpNewImage.Height), 0, 0, bmpImageToConvert.Width, bmpImageToConvert.Height, System.Drawing.GraphicsUnit.Pixel);
            gfxNewImage.Dispose();
            bmpImageToConvert.Dispose();
            bmpNewImage.Save(to_file, format);
        }

        public void SwitchExifMetadata()
        {
            ExifPanelVision = !ExifPanelVision;
        }

        #endregion

        #region OpenFileInFolder
        private void OpenFileInFolder()
        {
            Magic.OpenAndSelect(CurrentPicture.Path);
        }
        #endregion

        #region Mover
        private void MoverPrimera() 
        {
            CurrentPicture = Pictures.Primera;
        }

        public void MoverAnterior()
        {
            CurrentPicture = Pictures.Anterior(CurrentPicture);
        }

        public void MoverSiguiente()
        {
            CurrentPicture = Pictures.Siguiente(CurrentPicture);
        }

        private void MoverUltima()
        {            
            CurrentPicture = Pictures.Ultima;
        }

        #endregion

        #region Presentación
        private void Presentacion()
        {
            CancelBlur();
            RestaurarRotacion();
            IniciarPresentacion();
        }

        private DispatcherTimer _slideTimer;
        public DispatcherTimer SlideTimer
        {
            get { return _slideTimer; }
            set { _slideTimer = value; }
        }

        private void IniciarPresentacion()
        {
            PresentationRunning = true;
            CtrlLeftVision = false;
            CtrlRightVision = false;
            SlideTimer = new DispatcherTimer();
            SlideTimer.Interval = TimeSpan.FromSeconds(3);
            SlideTimer.Tick += SlideTimer_Tick;
            SlideTimer.Start();
        }

        void SlideTimer_Tick(object sender, EventArgs e)
        {
            if (CurrentPicture.Indice < Pictures.Count - 1)
            {
                MoverSiguiente();
            }
            else
            {
                DetenerPresentacion();
            }
        }

        public void DetenerPresentacion()
        {
            CtrlLeftVision = true;
            CtrlRightVision = true;
            SlideTimer.Stop();
            SlideTimer = null;
            PresentationRunning = false;
        }

        #endregion

        #region ZoomIn
        public void ZoomIn()
        {
            Point p = new Point(Imagen.ActualWidth / 2, Imagen.ActualHeight / 2);
            Border.ApplyZoom(1, p);            
        }
        #endregion

        #region ZoomOut
        public void ZoomOut()
        {
            Point p = new Point(Imagen.ActualWidth / 2, Imagen.ActualHeight / 2);
            Border.ApplyZoom(-1, p);
        }
        #endregion

        #region RotateLeft y RotateRight

        //Posible solución para giros en ángulos erróneos:
        //https://stackoverflow.com/questions/29585378/wpf-rotating-image-with-arbitrary-angle

        private void RotateLeft()
        {
            if (Transform.Angle % 90 == 0)
            {
                Storyboard storyboard = new Storyboard();
                storyboard.Completed += rotation_Completed;
                storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(150));
                DoubleAnimation rotateAnimation = new DoubleAnimation(Transform.Angle, Transform.Angle - 90, storyboard.Duration); 
                Storyboard.SetTarget(rotateAnimation, Border);
                Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
                storyboard.Children.Add(rotateAnimation);
                storyboard.Begin();
            }
        }
                
        private void RotateRight()
        {
            if (Transform.Angle % 90 == 0)
            {
                Storyboard storyboard = new Storyboard();
                storyboard.Completed += rotation_Completed;
                storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(150));
                DoubleAnimation rotateAnimation = new DoubleAnimation(Transform.Angle, Transform.Angle + 90, storyboard.Duration); 
                Storyboard.SetTarget(rotateAnimation, Border);
                Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
                storyboard.Children.Add(rotateAnimation);
                storyboard.Begin();
            }
        }

        private void RotateRightORIGINAL()
        {
            if (Transform.Angle % 90 == 0)
            {
                Storyboard storyboard = new Storyboard();
                storyboard.Completed += rotation_Completed;
                storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(150));
                DoubleAnimation rotateAnimation = new DoubleAnimation();

                switch ((int)Transform.Angle)
                {
                    case 0:
                    case 360:
                        rotateAnimation = new DoubleAnimation(0, 90, storyboard.Duration);
                        break;
                    case 90:
                    case -270:
                        rotateAnimation = new DoubleAnimation(90, 180, storyboard.Duration);
                        break;
                    case 180:
                    case -180:
                        rotateAnimation = new DoubleAnimation(180, 270, storyboard.Duration);
                        break;
                    case 270:
                    case -90:
                        rotateAnimation = new DoubleAnimation(270, 360, storyboard.Duration);
                        break;
                }

                Storyboard.SetTarget(rotateAnimation, Border);
                Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

                storyboard.Children.Add(rotateAnimation);
                storyboard.Begin();
            }
        }

        void rotation_Completed(object sender, EventArgs e)
        {
            
        }

        private void RestaurarRotacion()
        {
            RotateTransform ct = (RotateTransform)Border.RenderTransform;
            Storyboard storyboard = new Storyboard();
            storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(0));
            DoubleAnimation rotateAnimation = new DoubleAnimation();
            rotateAnimation = new DoubleAnimation(0, storyboard.Duration);
            Storyboard.SetTarget(rotateAnimation, Border);
            Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));

            storyboard.Children.Add(rotateAnimation);
            storyboard.Begin();
        } 
        #endregion

        #region ApplyBlur
        private void ApplyBlur()
        {
            if (Border.Effect == null)
            {
                System.Windows.Media.Effects.BlurEffect efecto = new System.Windows.Media.Effects.BlurEffect();
                efecto.Radius = 25;
                Border.Effect = efecto;
            }
            else
            {
                CancelBlur();
            }
        }

        private void CancelBlur()
        {
            Border.Effect = null;            
        }

        #endregion

        #region ResetAllMods
        private void ResetAllMods()
        {
            Border.Reset();
            CancelBlur();
            RestaurarRotacion();
        }
        #endregion

        #region Configuración

        private void ShowSettings()
        {
            SettingsDialog dialog = new SettingsDialog();
            dialog.ShowDialog();
        }

        #endregion

        #region ListPrograms
        private void ListPrograms()
        {
            ShowOpenWithDialog();
        }

        /// <summary>
        /// Abre el cuadro de diálogo para seleccionar un programa compatible que abra este tipo de archivos
        /// Fuente: https://stackoverflow.com/questions/4726441/how-can-i-show-the-open-with-file-dialog
        /// </summary>
        private void ShowOpenWithDialog()
        {
            var args = Path.Combine(Environment.SystemDirectory, "shell32.dll");
            args += ",OpenAs_RunDLL " + CurrentPicture.Path;
            System.Diagnostics.Process.Start("rundll32.exe", args);
        }


        #endregion

        #region OpenAndSelect
        private void OpenAndSelect()
        {
            Magic.OpenAndSelect(CurrentPicture.Path);
        }
        #endregion

        #region NormalizeExifRotation
        private void NormalizeExifRotation(Func<ushort> func)
        {
            if (LoadSuccessfully)
            {
                UInt16 valor = func.Invoke();
                if (valor > 0)
                {
                    Storyboard storyboard = new Storyboard();
                    //storyboard.Completed += rotation_Completed;
                    storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(0));
                    DoubleAnimation rotateAnimation = new DoubleAnimation(Transform.Angle, Transform.Angle, storyboard.Duration);
                    switch ((int)valor)
                    {
                        case 1:
                            rotateAnimation = new DoubleAnimation(0, storyboard.Duration);
                            break;
                        //case 2: //
                        //    rotateAnimation = new DoubleAnimation(90, storyboard.Duration);
                        //    break;
                        case 3:
                            rotateAnimation = new DoubleAnimation(180, storyboard.Duration);
                            break;
                        //case 4: //
                        //    rotateAnimation = new DoubleAnimation(270, storyboard.Duration);
                        //    break;
                        //case 5: //
                        //    rotateAnimation = new DoubleAnimation(0, storyboard.Duration);
                        //    break;
                        case 6:
                            rotateAnimation = new DoubleAnimation(90, storyboard.Duration);
                            break;
                        //case 7: //
                        //    rotateAnimation = new DoubleAnimation(180, storyboard.Duration);
                        //    break;
                        case 8:
                            rotateAnimation = new DoubleAnimation(270, storyboard.Duration);
                            break;
                    }

                    Storyboard.SetTarget(rotateAnimation, Border);
                    Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
                    storyboard.Children.Add(rotateAnimation);
                    storyboard.Begin();
                }
            }
            else
            {
                LoadSuccessfully = true;
            }

            RaisePropertyChanged("CurrentPicture");
        }
        #endregion
        
        #region ReadMetadata
        private void ReadMetadata()
        {
            ExifTags.Clear();
            ExifTags.Add("=== METADATA ===");
            IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(CurrentPicture.Path);
            foreach (var directory in directories)
                foreach (var tag in directory.Tags)
                ExifTags.Add(directory.Name + " - " + tag.Name + " = " + tag.Description);

            RaisePropertyChanged("ExifTags");
        }
        #endregion
    }
}
