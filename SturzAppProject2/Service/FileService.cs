using BackgroundTask.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BackgroundTask.Service
{
    public class FileService
    {
        private readonly string _fileName = "Measurements.json";

        internal async void SaveMeasurementsAsync(List<Measurement> measurements)
        {
            if (measurements != null)
            {
                string jsonString = JsonConvert.SerializeObject(measurements);
                await SaveJsonStringToFile(_fileName, jsonString);
            }
        }

        internal async Task<List<Measurement>> LoadMeasurementListAsync()
        {
            bool isFileCorrupted = false;
            List<Measurement> measurements = new List<Measurement>();
            string jsonString = await LoadJsonStringFromFile(_fileName);

            if (jsonString != null && jsonString.Length > 0)
            {
                try
                {
                    measurements = JsonConvert.DeserializeObject<List<Measurement>>(jsonString);
                }
                catch (JsonSerializationException)
                {
                    Debug.WriteLine("[BackgroundTask.Service.FileService] Sersialized list of measurements could not deserialized.");
                    // Delete corrupted file.
                    isFileCorrupted = true;
                }

                if (isFileCorrupted)
                {
                    await deleteFile(_fileName);
                }
            }
            return measurements;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// SaveToFile /////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private async Task SaveJsonStringToFile(string filename, string jsonString)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                if (localFolder != null)
                {
                    StorageFile file = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    using (IRandomAccessStream textStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (DataWriter textWriter = new DataWriter(textStream))
                        {
                            textWriter.WriteString(jsonString);
                            await textWriter.StoreAsync();
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.SaveJsonStringToFile] Datei: {0} konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.SaveJsonStringToFile] Datei: {0} konnte nicht zugegriffen werden.", filename);
            }
            return;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////// LoadFromFile ///////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private async Task<string> LoadJsonStringFromFile(string filename)
        {
            string jsonString = null;

            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                if (localFolder != null)
                {
                    IReadOnlyList<StorageFile> files = await localFolder.GetFilesAsync();
                    if (files != null)
                    {
                        IEnumerator<StorageFile> filesIterator = files.GetEnumerator();
                        while (filesIterator.MoveNext())
                        {
                            StorageFile file = filesIterator.Current;
                            if (file.Name.Equals(filename))
                            {
                                jsonString = await FileIO.ReadTextAsync(file);
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.LoadJsonStringFromFile] Datei: {0} konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.LoadJsonStringFromFile] Datei: {0} konnte nicht zugegriffen werden.", filename);
            }
            return jsonString;
        }

        private async Task deleteFile(string filename)
        {
            try 
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                if (localFolder != null)
                {
                    StorageFile file = await localFolder.GetFileAsync(filename);
                    if (file != null)
                    {
                        await file.DeleteAsync();
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("[MensaApp.FileService.SaveJsonStringToFile] Datei: {0} konnte nicht gefunden werden.", filename);
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("[MensaApp.FileService.SaveJsonStringToFile] Datei: {0} konnte nicht zugegriffen werden.", filename);
            }
            return;
        }
    }
}
