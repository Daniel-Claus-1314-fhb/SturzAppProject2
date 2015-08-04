using BackgroundTask.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BackgroundTask.Service
{
    class FileService
    {
        private string folderName = "Measurements";

        public async void AppendPassivReadingsToFileAsync(AccelerometerData accelerometerData)
        {
            Debug.WriteLine("############## Save Passiv Readings ##################");
            await SaveToEndOfFileAsync(accelerometerData.AccelerometerDataId, accelerometerData.GetPassivReadingsList());
        }

        public async void AppendActivReadingsToFileAsync(AccelerometerData accelerometerData)
        {
            Debug.WriteLine("############## Save Activ Readings ##################");
            await SaveToEndOfFileAsync(accelerometerData.AccelerometerDataId, accelerometerData.GetActivReadingsList());
        }

        private async Task SaveToEndOfFileAsync(string filename, IList<AccelerometerReading> acceleroReadingsList)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                if (localFolder != null)
                {
                    StorageFolder measurementFolder = await localFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

                    if (measurementFolder != null)
                    {
                        StorageFile file = await measurementFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                        using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            using (DataWriter textWriter = new DataWriter(textStream.GetOutputStreamAt(textStream.Size)))
                            {
                                foreach (AccelerometerReading accelerometerReading in acceleroReadingsList)
                                {
                                    String text = String.Format(new CultureInfo("en-US") ,"{0:f4},{1:f4},{2:f4},{3}\n", 
                                        accelerometerReading.AccelerationX, accelerometerReading.AccelerationY, accelerometerReading.AccelerationZ, accelerometerReading.Timestamp.UtcTicks);
                                    textWriter.WriteString(text);
                                }
                                await textWriter.StoreAsync();
                            }
                            Debug.WriteLine("############## Current file size: '{0}' ################", textStream.Size);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[BackgroundTask.Service.FileService.SaveStringToFile] Datei: {0} konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[BackgroundTask.Service.FileService.SaveStringToFile] Datei: {0} konnte nicht zugegriffen werden.", filename);
            }
            return;
        }
    }
}
