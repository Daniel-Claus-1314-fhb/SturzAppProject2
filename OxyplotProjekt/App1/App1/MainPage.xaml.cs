using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Series;
using Windows.Storage;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=391641 dokumentiert.

namespace App1
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            



            oxyplot.Model = new PlotModel { Title = "Testdaten" };
            oxyplot.Model.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
           

        }

        public PlotModel MyModel { get; private set; }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Rahmen angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Seite vorbereiten, um sie hier anzuzeigen.

            // TODO: Wenn Ihre Anwendung mehrere Seiten enthält, stellen Sie sicher, dass
            // die Hardware-Zurück-Taste behandelt wird, indem Sie das
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed-Ereignis registrieren.
            // Wenn Sie den NavigationHelper verwenden, der bei einigen Vorlagen zur Verfügung steht,
            // wird dieses Ereignis für Sie behandelt.
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            LineSeries x = new LineSeries();
            LineSeries y = new LineSeries();
            LineSeries z = new LineSeries();
            double equal = 0;

            x.Title = "X";
            y.Title = "Y";
            z.Title = "Z";

            string fileContent = "";
            int counter = 0;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///testData.csv"));
            StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync());
            fileContent = await sRead.ReadLineAsync();
            while (fileContent != null)
            {
                if (counter != 0)
                {        
                    string[] help;
                    help = fileContent.Split(new Char[] { ',' });
                    if (counter == 1)
                    {
                       equal = Convert.ToDouble(help[3]); 
                    }
                    x.Points.Add(new DataPoint(Convert.ToDouble(help[3]) - equal, Convert.ToDouble(help[0])));
                    y.Points.Add(new DataPoint(Convert.ToDouble(help[3]) - equal, Convert.ToDouble(help[1])));
                    z.Points.Add(new DataPoint(Convert.ToDouble(help[3]) - equal, Convert.ToDouble(help[2])));
                }
                fileContent = await sRead.ReadLineAsync();
                counter++;
                }
            oxyplot.Model.Series.Clear();
            oxyplot.Model.Series.Add(x);
            oxyplot.Model.Series.Add(y);
            oxyplot.Model.Series.Add(z);
            oxyplot.Model.InvalidatePlot(true);

        }
    }
}
