using BackgroundTask.Common;
using BackgroundTask.DataModel;
using BackgroundTask.ViewModel;
using BackgroundTask.ViewModel.Page;
using BackgroundTask.ViewModel.Setting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace BackgroundTask
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private SettingPageViewModel _pageViewModel;
        private string _measurementId;
        private MainPage _mainPage;

        public SettingPage()
        {
            this._mainPage = MainPage.Current;
            this._pageViewModel = new SettingPageViewModel();

            this.InitializeComponent();
        }

        public SettingPageViewModel PageViewModel
        {
            get { return this._pageViewModel; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                // wenn eine id verwendet wird, öffne die Settings der jeweiligen messung
                _measurementId = e.Parameter as string;
                MeasurementModel measurement = _mainPage.GlobalMeasurementModel.GetMeasurementById(_measurementId);
                _pageViewModel.SettingViewModel = new SettingViewModel(measurement.Setting);
                _pageViewModel.isGlobalSetting = false;
            }
            else
            {
                // wenn keine id verwendet wird, öffne die globalen settings
                _pageViewModel.SettingViewModel = new SettingViewModel(_mainPage.GlobalSettingModel);
                _pageViewModel.isGlobalSetting = true;
            }
        }

        private void SaveAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pageViewModel.isGlobalSetting)
            {
                _mainPage.GlobalMeasurementModel.UpdateGlobalSetting(_pageViewModel.SettingViewModel);
                _mainPage.ShowNotifyMessage("Allgemeine Einstellungen wurde gespeichert.", NotifyLevel.Info);
            }
            else if (_measurementId != null && _measurementId != String.Empty)
            {
                _mainPage.GlobalMeasurementModel.UpdateMeasurementSettingById(_measurementId, _pageViewModel.SettingViewModel);
                _mainPage.ShowNotifyMessage("Einstellung der Messung wurde gespeichert.", NotifyLevel.Info);
            }
            else
            {
                _mainPage.ShowNotifyMessage("Einstellung konnten nicht gespeichert werden.", NotifyLevel.Error);
            }
        }
    }
}
