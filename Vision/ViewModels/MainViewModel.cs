﻿using MetadataExtractor;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
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
            get
            {
                return _pictures;
            }
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
            get
            {
                return _loadSuccessfully;
            }
            set
            {
                _loadSuccessfully = value;
            }
        }

        private Picture _currentPicture;
        public Picture CurrentPicture
        {
            get
            {
                return _currentPicture;
            }
            set
            {
                try
                {
                    ResetAllMods();
                    _currentPicture = value;
                    RaisePropertyChanged("CurrentPicture");
                    RaisePropertyChanged("LogoVision");
                    RaisePropertyChanged("CanRotate");
                    NormalizeExifRotation(value.ExifOrientation);
                    ReadMetadata();
                }
                catch (System.ArgumentException)
                {
                    Console.WriteLine("Excepción controlada por asignación nula (ignorar)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error por asignación de null a value u otra excepción: " + ex.ToString());
                }
            }
        }

        private ObservableCollection<string> _exifTags = new ObservableCollection<string>();
        public ObservableCollection<string> ExifTags
        {
            get
            {
                return _exifTags;
            }
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
            get
            {
                return _ctrlLeftVision;
            }
            set
            {
                _ctrlLeftVision = value;
                RaisePropertyChanged("CtrlLeftVision");
            }
        }

        private bool _ctrlRightVision = true;
        public bool CtrlRightVision
        {
            get
            {
                return _ctrlRightVision;
            }
            set
            {
                _ctrlRightVision = value;
                RaisePropertyChanged("CtrlRightVision");
            }
        }

        private bool _barVision = false;
        public bool BarVision
        {
            get
            {
                return _barVision;
            }
            set
            {
                _barVision = value;
                RaisePropertyChanged("BarVision");
            }
        }

        private bool _commandBarVision = true;
        public bool CommandBarVision
        {
            get
            {
                return _commandBarVision;
            }
            set
            {
                _commandBarVision = value;
                RaisePropertyChanged("CommandBarVision");
            }
        }

        private bool _imageOnlyVision = false;
        public bool ImageOnlyVision
        {
            get
            {
                return _imageOnlyVision;
            }
            set
            {
                _imageOnlyVision = value;
                RaisePropertyChanged("ImageOnlyVision");
            }
        }

        private bool _exifPanelVision = false;
        public bool ExifPanelVision
        {
            get
            {
                return _exifPanelVision;
            }
            set
            {
                _exifPanelVision = value;
                RaisePropertyChanged("ExifPanelVision");
            }
        }

        private bool _presentationRunning = false;
        public bool PresentationRunning
        {
            get
            {
                return _presentationRunning;
            }
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
            get
            {
                return _listOpacity;
            }
            set
            {
                _listOpacity = value;
                RaisePropertyChanged("ListOpacity");
            }
        }

        private ZoomBorder _border;
        public ZoomBorder Border
        {
            get
            {
                return _border;
            }
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
            get
            {
                return Border == null ? null : (System.Windows.Controls.Image)_border.Child;
            }
        }

        public RotateTransform Transform
        {
            get
            {
                return Border == null ? null : (RotateTransform)Border.RenderTransform;
            }
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

        private ICommand _printFile2Command;
        public ICommand PrintFile2Command
        {
            get
            {
                if (_printFile2Command == null)
                    _printFile2Command = new RelayCommand(new Action(PrintFile2), () => CanExecute);
                return _printFile2Command;
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

        //private ICommand _deleteFileCommand;
        //public ICommand DeleteFileCommand
        //{
        //    get
        //    {
        //        if (_deleteFileCommand == null)
        //            _deleteFileCommand = new RelayCommand(new Action(DeleteFile), () => CanExecute);
        //        return _deleteFileCommand;
        //    }
        //}

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

        private ICommand _setAsWallpaperCommand;
        public ICommand SetAsWallpaperCommand
        {
            get
            {
                if (_setAsWallpaperCommand == null)
                    _setAsWallpaperCommand = new RelayCommand(new Action(SetAsWallpaper));
                return _setAsWallpaperCommand;
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
            get
            {
                if (CurrentPicture != null)
                {
                    return Pictures.Count > 0 && CurrentPicture.Indice != 0;
                }
                return false;
            }
        }

        private bool CanMovePrev
        {
            get
            {
                if (CurrentPicture != null)
                {
                    return Pictures.Count > 0 && CurrentPicture.Indice > 0;
                }
                return false;
            }
        }

        private bool CanMoveNext
        {
            get
            {
                if (CurrentPicture != null)
                {
                    return Pictures.Count > 0 && CurrentPicture.Indice < Pictures.Count - 1;
                }
                return false;
            }
        }

        private bool CanMoveLast
        {
            get
            {
                if (CurrentPicture != null)
                {
                    return Pictures.Count > 0 && CurrentPicture.Indice != Pictures.Count - 1;
                }
                return false;
            }
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

        #region SetAsWallpaper

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, String pvParam, uint fWinIni);

        private const uint SPI_SETDESKWALLPAPER = 0x14;
        private const uint SPIF_UPDATEINIFILE = 0x1;
        private const uint SPIF_SENDWININICHANGE = 0x2;
        private void SetAsWallpaper()
        {
            //update_registry indica si el cambio es permanente
            //por ahora lo será siempre, más adelante pondré la opción para que se revierta al cerrar sesión
            bool update_registry = true;
            try
            {
                // Si debemos actualizar el registro, poner las flags apropiadas
                uint flags = 0;
                if (update_registry)
                    flags = SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE;

                if (!SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, CurrentPicture.Path, flags))
                {
                    MessageBox.Show("Fallo de SystemParametersInfo.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    MessageBox.Show("La imagen se aplicó!", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error desplegando imagen " + CurrentPicture.Path + ".\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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
        public void OpenFile()
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
                await Task.Run(() => LoadDotVision(Pictures.FirstImage));
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
                Pictures.FirstImage = null; //Limpio y evito que la imagen se bloquée al tratar de borrar
                return SettingsManager.Load("IgnoreHidden") == 1 ? files.Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden)).ToList() : files;
            }
            catch (Exception ex)
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

        private void PrintFile2()
        {
            ImprimirMetodo2();
        }


        private void ImprimirMetodo2()
        {
            //PrintDocument pd = new PrintDocument();
            //pd.PrintPage += pd_PrintPage;

            //PrintDialog printPreviewDialog1 = new PrintDialog();
            //printPreviewDialog1. = pd;
            //printPreviewDialog1.ShowDialog();

            //pd.Print(); 
        }

        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile("D:\\Foto.jpg");
            System.Drawing.Point loc = new System.Drawing.Point(100, 100);
            e.Graphics.DrawImage(img, loc);
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
        public void MoverPrimera()
        {
            CurrentPicture = Pictures.Primera;
            RotateNow(CurrentPicture);
        }

        public void MoverAnterior()
        {
            CurrentPicture = Pictures.Anterior(CurrentPicture);
            RotateNow(CurrentPicture);
        }

        public void MoverSiguiente()
        {
            CurrentPicture = Pictures.Siguiente(CurrentPicture);
            RotateNow(CurrentPicture);

        }

        public void MoverUltima()
        {
            CurrentPicture = Pictures.Ultima;
            RotateNow(CurrentPicture);
        }

        #endregion

        #region Presentación
        public void Presentacion()
        {
            CancelBlur();
            RestaurarRotacion();
            IniciarPresentacion();
        }

        private DispatcherTimer _slideTimer;
        public DispatcherTimer SlideTimer
        {
            get
            {
                return _slideTimer;
            }
            set
            {
                _slideTimer = value;
            }
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

        public void RotateLeft()
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

        public void RotateRight()
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

        private void UpdateDotVision()
        {

        }

        public void RotateNow(Picture picture)
        {
            double angle = 0;
            angle = Rotations.getAngle(picture);


            if (Transform.Angle % 90 == 0)
            {
                Storyboard storyboard = new Storyboard();
                storyboard.Completed += rotation_Completed;
                storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(0));
                //double angulo = angle > 0 ? Transform.Angle + angle : Transform.Angle - angle;
                DoubleAnimation rotateAnimation = new DoubleAnimation(Transform.Angle, Transform.Angle + angle, storyboard.Duration);
                Storyboard.SetTarget(rotateAnimation, Border);
                Storyboard.SetTargetProperty(rotateAnimation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
                storyboard.Children.Add(rotateAnimation);
                storyboard.Begin();
            }
        }

        private void LoadDotVision(Picture picture)
        {
            string fileName = picture.Directorio + @"\.vision";
            Rotations.Clear();
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(fileName))
            using (var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8, true, BufferSize))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] valores = line.Split('|');
                    double angulo = 0;
                    double.TryParse(valores[1], out angulo);
                    Rotations.Add(new Rotation(valores[0], angulo));
                }
            }
        }

        private RotationList Rotations = new RotationList();












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
            try
            {
                if (LoadSuccessfully)
                {
                    UInt16 valor = func.Invoke();
                    if (valor > 0)
                    {
                        Storyboard storyboard = new Storyboard();
                        storyboard.Duration = new Duration(TimeSpan.FromMilliseconds(0));
                        DoubleAnimation rotateAnimation = new DoubleAnimation(Transform.Angle, Transform.Angle, storyboard.Duration);
                        switch ((int)valor)
                        {
                            case 1:
                                rotateAnimation = new DoubleAnimation(0, storyboard.Duration);
                                break;
                            case 3:
                                rotateAnimation = new DoubleAnimation(180, storyboard.Duration);
                                break;
                            case 6:
                                rotateAnimation = new DoubleAnimation(90, storyboard.Duration);
                                break;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error por asignación nula en CurrentPicture: " + ex.ToString());
            }
            finally
            {
                RaisePropertyChanged("CurrentPicture");
            }
        }
        #endregion

        #region ReadMetadata
        private void ReadMetadata()
        {
            try
            {
                ExifTags.Clear();
                if (CurrentPicture != null)
                {
                    ExifTags.Add("=== METADATA ===");
                    IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(CurrentPicture.Path);
                    foreach (var directory in directories)
                        foreach (var tag in directory.Tags)
                            ExifTags.Add(directory.Name + " - " + tag.Name + " = " + tag.Description);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error por asginación nula en CurrentPicture: " + ex.ToString());
            }
            finally
            {
                RaisePropertyChanged("ExifTags");
            }
        }
        #endregion


        /* ===================== CÓDIGO EN CONSTRUCCIÓN ===================== */


        #region Eliminar (DEBUG PENDIENTE)
        //private void DeleteFile()
        //{
        //    try
        //    {
        //        //si es el unico
        //        //---limpiar la lista y que vuelva el logo
        //        //si no
        //        //---si hay una imagen antes
        //        //------moverse a ella
        //        //---si hay una imagen despues
        //        //------moverse a ella

        //        if (Pictures.Count > 1)
        //        {
        //            string ruta = CurrentPicture.Path;
        //            if (Pictures.PrevPicture(CurrentPicture))
        //            {
        //                CurrentPicture = Pictures.Anterior(CurrentPicture);
        //                Pictures.Remove(Pictures.Siguiente(CurrentPicture));
        //            }
        //            else
        //            {
        //                CurrentPicture = Pictures.Siguiente(CurrentPicture);
        //                Pictures.Remove(Pictures.Anterior(CurrentPicture));
        //            }

        //            Pictures.RemapIndexes();
        //            BorrarArchivo(ruta);
        //        }
        //        else
        //        {
        //            string ruta = string.Empty + CurrentPicture.Path;
        //            Pictures.Remove(CurrentPicture);
        //            CurrentPicture = null; //esto da error pero lo manejo en el SET 
        //            TryDelete(ruta);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("=>>>: " + ex.ToString());
        //        MessageBox.Show("=>>>: " + ex.Message);
        //    }
        //}


        //private void TryDelete(string path)
        //{
        //    CtrlLeftVision = false;
        //    CtrlRightVision = false;
        //    //Pictures.FirstImage = null;
        //    //Pictures.Clear();
        //    //CurrentPicture = null;
        //    ResetAllMods();

        //    _ = Task.Run(async () =>
        //    {
        //        await Task.Run(() => DummyMethod());
        //        GC.Collect();
        //        GC.WaitForPendingFinalizers();

        //        try
        //        {
        //            if (File.Exists(path))
        //            {
        //                File.Delete(path);
        //            }
        //        }
        //        catch (IOException ex)
        //        {
        //            Console.WriteLine("=========> Error IO el archivo: " + ex.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("=========> Error borrando el archivo: " + ex.ToString());
        //        }
        //    });

        //} 


        //private void DummyMethod()
        //{
        //    Application.Current.Dispatcher.Invoke(new Action(() =>
        //    {
        //        Pictures.RemapIndexes();
        //    }));
        //}

        //private void BorrarArchivo(string path)
        //{
        //    try
        //    {
        //        Task tsk = Task.Run(async () =>
        //        {
        //            await Task.Run(() => DummyMethod());
        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //            try
        //            {
        //                File.Delete(path);
        //                Console.WriteLine("=========> Borrando 5/5 Finishing");
        //            }
        //            catch (System.IO.IOException ex)
        //            {
        //                Console.WriteLine("=========> Error IO el archivo: " + ex.ToString());
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine("=========> Error borrando el archivo: " + ex.ToString());
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("=========> Error borrando el archivo: " + ex.ToString());
        //        MessageBox.Show("Imposible borrar el archivo, intente más tarde", "Información", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}


        //private void BorrarArchivoUnico(string path)
        //{
        //    try
        //    {
        //        Task tsk = Task.Run(async () =>
        //        {
        //            await Task.Run(() => LimpiarColecciones());
        //            GC.Collect();
        //            GC.WaitForPendingFinalizers();
        //            File.Delete(path);
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error borrando el archivo: " + ex.ToString());
        //        MessageBox.Show("Imposible borrar el archivo, intente más tarde", "Información", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        #endregion

    }
}
