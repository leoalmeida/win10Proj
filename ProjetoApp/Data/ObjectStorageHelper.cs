﻿using Microsoft.Toolkit.Uwp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ProjetoApp
{
    public class ObjectStorageHelper : IObjectStorageHelper
    {
        private JsonSerializer serializer = new JsonSerializer();

        /// <summary>
        /// Gets or sets settings container
        /// </summary>
        protected ApplicationDataContainer Settings { get; set; }

        /// <summary>
        /// Gets or sets storage folder
        /// </summary>
        protected StorageFolder Folder { get; set; }

        /// <summary>
        /// Detect if a setting already exists
        /// </summary>
        /// <param name="key">Key of the setting (that contains object)</param>
        /// <returns>True if a value exists</returns>
        public bool KeyExists(string key)
        {
            return Settings.Values.ContainsKey(key);
        }

        /// <summary>
        /// Retrieve single item by its key.
        /// </summary>
        /// <typeparam name="T">Type of object retrieved</typeparam>
        /// <param name="key">Key of the object</param>
        /// <param name="default">Default value of the object</param>
        /// <returns>The T object</returns>
        public T Read<T>(string key, T @default = default(T))
        {
            string value = (string)Settings.Values[key];
            if (value != null)
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            return @default;
        }

        /// <summary>
        /// Save single item by its key.
        /// This method should be considered for objects that do not exceed 8k bytes during the lifetime of the application
        /// (refers to <see cref="SaveFileAsync{T}(string, T)"/> for complex/large objects).
        /// </summary>
        /// <typeparam name="T">Type of object saved</typeparam>
        /// <param name="key">Key of the value saved</param>
        /// <param name="value">Object to save</param>
        public void Save<T>(string key, T value)
        {
            Settings.Values[key] = JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Detect if a file already exists
        /// </summary>
        /// <param name="filePath">Key of the file (that contains object)</param>
        /// <returns>True if a value exists</returns>
        public async Task<bool> FileExistsAsync(string filePath)
        {
            if (Folder == null)
            {
                Folder = await GetDataSaveFolder();
            }

            return await Folder.IsFileExistsAsync(filePath);
        }

        /// <summary>
        /// Retrieve object from file.
        /// </summary>
        /// <typeparam name="T">Type of object retrieved</typeparam>
        /// <param name="filePath">Path to the file that contains the object</param>
        /// <param name="default">Default value of the object</param>
        /// <returns>Waiting task until completion with the object in the file</returns>
        public async Task<T> ReadFileAsync<T>(string filePath, T @default = default(T))
        {
            if (Folder == null)
            {
                Folder = await GetDataSaveFolder();
            }

            string value = await StorageFileHelper.ReadTextFromFileAsync(Folder, filePath);
            return (value != null) ? JsonConvert.DeserializeObject<T>(value) : @default;
        }

        /// <summary>
        /// Save object inside file.
        /// There is no limitation to use this method (refers to <see cref="Save{T}(string, T)"/> method for simple objects).
        /// </summary>
        /// <typeparam name="T">Type of object saved</typeparam>
        /// <param name="filePath">Path to the file that will contain the object</param>
        /// <param name="value">Object to save</param>
        /// <returns>Waiting task until completion</returns>
        public async Task SaveFileAsync<T>(string filePath, T value)
        {
            if (Folder == null)
            {
                Folder = await GetDataSaveFolder();
            }

            await StorageFileHelper.WriteTextToFileAsync(Folder, JsonConvert.SerializeObject(value), filePath, CreationCollisionOption.ReplaceExisting);
        }

        public static async Task<StorageFolder> GetDataSaveFolder()
        {
            StorageFolder folder;

            try
            {
                var pictureLibraryFolder = (await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures)).SaveFolder;

                try
                {
                    folder = await pictureLibraryFolder.GetFolderAsync("OptionWorks");
                }
                catch (Exception ex)
                {
                    folder = await pictureLibraryFolder.CreateFolderAsync("OptionWorks");
                }
            }
            catch (Exception ex)
            {
                folder = ApplicationData.Current.LocalFolder;
            }

            return folder;
        }

        Task<StorageFile> IObjectStorageHelper.SaveFileAsync<T>(string filePath, T value)
        {
            throw new NotImplementedException();
        }
    }
}