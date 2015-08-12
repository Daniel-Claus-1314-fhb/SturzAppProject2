using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Sensors;
using Windows.UI.Popups;
using Windows.UI.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.ApplicationModel.Activation;

namespace accelometerDemo
{
    public sealed partial class MainPage : Page, IFileSavePickerContinuable, IFileOpenPickerContinuable
    {
        public static MainPage Current;    
        public int counter = 0;
        public int stepcounter = 0;
        StorageFile sampleFile;
        Schrittzähler sc = new Schrittzähler();
        Datenumwandlung dtu = new Datenumwandlung();
        public short[] Buffer1 = new short[Public.BUFFERSIZE];
        public short[] Buffer2 = new short[Public.BUFFERSIZE];
        public double[] dblBuffer = new double[Public.BUFFERSIZE / 3];
        public int Buffercounter = -1;
        public Windows.System.Display.DisplayRequest KeepScreenOnRequest = new Windows.System.Display.DisplayRequest();
        public Accelerometer accelerometer { get; set; }
        private uint desiredReportInterval { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            Current = this;       
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private async void ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {    
                AccelerometerReading reading = args.Reading;
                Buffercounter++;
                Buffer1[Buffercounter] = dtu.DoubleToShort(reading.AccelerationX);
                corX.Text = String.Format("{0,5:0.00}", reading.AccelerationX);
                Buffercounter++;
                Buffer1[Buffercounter] = dtu.DoubleToShort(reading.AccelerationY);
                corY.Text = String.Format("{0,5:0.00}", reading.AccelerationY);
                Buffercounter++;
                Buffer1[Buffercounter] = dtu.DoubleToShort(reading.AccelerationZ);
                corZ.Text = String.Format("{0,5:0.00}", reading.AccelerationZ);
                lbl_Datacount.Text = "#: " + counter;
                if (Buffercounter == Public.BUFFERSIZE - 1)
                {
                    Buffer2 = Buffer1;
                    BufferToFile(Buffer2);
                    stepcounter += sc.CountSteps(Buffer2);
                    lbl_steps.Text = "steps: " + stepcounter;
                    Buffercounter = -1;
                    counter++;
                    KeepScreenOnRequest.RequestActive();
                }
            }); 
        }
        private async void BufferToFile(short[] Buffer)
        {
            byte[] data = new byte[Public.BUFFERSIZE * 2];   
                for (int i = 0; i < Public.BUFFERSIZE; i++)
                {
                    data[2 * i] = BitConverter.GetBytes(Buffer[i])[0];
                    data[2 * i + 1] = BitConverter.GetBytes(Buffer[i])[1];
                }
                await Windows.Storage.FileIO.AppendTextAsync(sampleFile, Encoding.UTF8.GetString(data, 0, data.Length));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {  
        }
        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            btn_stop.Visibility = Visibility.Collapsed;
            btn_start.Visibility = Visibility.Visible;
            accelerometer.ReadingChanged -= new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
        }
        private async void btn_start_Click(object sender, RoutedEventArgs e)
        {
            btn_start.Visibility = Visibility.Collapsed;
            btn_stop.Visibility = Visibility.Visible;
            accelerometer = Accelerometer.GetDefault();
            if (accelerometer != null)
            {
                uint minReportInterval = accelerometer.MinimumReportInterval;
                accelerometer.ReportInterval = desiredReportInterval;
                lbl_ReportInterval.Text = "reportinterval: " + accelerometer.ReportInterval.ToString() + " = " + 1000 / accelerometer.ReportInterval + "Hz";
                accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
            else
            {
                MessageDialog ms = new MessageDialog("No accelerometer Found");
                await ms.ShowAsync();
            }   
        }
        private void btn_picker_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add(".dat", new List<string>() { ".dat" });
            savePicker.FileTypeChoices.Add(".txt", new List<string>() { ".txt" });
            savePicker.SuggestedFileName = "Aufnahme01";
            savePicker.PickSaveFileAndContinue();
        }
        public void ContinueFileSavePicker(FileSavePickerContinuationEventArgs args)
        {
            btn_start.Visibility = Visibility.Visible;
            if (args.File.DisplayType != null)
            {
                sampleFile = args.File;
                btn_start.Visibility = Visibility.Visible;
            }
            else
            {
                lbl_test.Text = "Status: Save abgebrochen";
            }
        }
        private void btn_pickeropen_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".dat");
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.PickSingleFileAndContinue();
        }
        public void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            btn_start.Visibility = Visibility.Visible;
            if (args.Files.Count > 0)
            {
                sampleFile = args.Files[0];
            }
            else
            {
                lbl_test.Text = "Status: keine oder zu viele Dateien ausgewählt";
            }
        }



    }
}
