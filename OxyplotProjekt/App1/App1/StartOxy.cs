namespace App1
{
    using System;

    using OxyPlot;
    using OxyPlot.Series;

    public class MainViewModel
    {
        public MainViewModel()
        {
            this.MyModel = new PlotModel { Title = "Testdaten" };
            //this.MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            LineSeries x = new LineSeries();
            LineSeries y = new LineSeries();
            LineSeries z = new LineSeries();

            
            //x.Points.Add(new DataPoint(0, 0));
            //y.Points.Add(new DataPoint(2, 2));
            z.Points.Add(new DataPoint(5, 10));

            this.MyModel.Series.Add(x);
            this.MyModel.Series.Add(y);
            this.MyModel.Series.Add(z);
        }

        public PlotModel MyModel { get; private set; }
    }
}