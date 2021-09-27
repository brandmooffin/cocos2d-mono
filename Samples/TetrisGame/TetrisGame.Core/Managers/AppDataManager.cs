using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using TetrisGame.Core.Settings;

namespace TetrisGame.Core.Managers
{
    /// <summary>
    /// Manage app data for the game. Helps maintain state for AppSettings
    /// </summary>
    public class AppDataManager
    {
        private const string AppDataFilename = "app_data.json";

        public AppSettings AppSettings;
        private static AppDataManager appDataManager;

        public static AppDataManager Instance
        {
            get
            {
                if (appDataManager == null)
                {
                    appDataManager = new AppDataManager();
                }
                return appDataManager;
            }
        }

        public AppDataManager()
        {
            AppSettings = new AppSettings();
            LoadData();
        }

        public void LoadData()
        {
            LoadFromIsolatedStorage();
        }

        public void SaveData()
        {
            SaveToIsolatedStorage();
        }

        private void SaveToIsolatedStorage()
        {
            // Get the place to store data
            using (IsolatedStorageFile isf = GetIsolatedStore())
            {
                if (isf.FileExists(AppDataFilename))
                {
                    isf.DeleteFile(AppDataFilename);
                }
                // Create a file to save the highscore data
                using (IsolatedStorageFileStream isfs =
                    isf.CreateFile(AppDataFilename))
                {
                    using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(AppSettings));
                    }
                }
            }
        }

        private IsolatedStorageFile GetIsolatedStore()
        {
            return IsolatedStorageFile.GetUserStoreForApplication();
        }

        /// <summary>
        /// Loads the high scores from a text file.  
        /// </summary>
        private void LoadFromIsolatedStorage()
        {
            // Get the place where data is stored
            using (IsolatedStorageFile isf =
                GetIsolatedStore())
            {
                // Try to open the file
                if (isf.FileExists(AppDataFilename))
                {
                    using (IsolatedStorageFileStream isfs =
                        isf.OpenFile(AppDataFilename, FileMode.Open))
                    {
                        AppSettings = JObject.Parse(new StreamReader(isfs).ReadToEnd()).ToObject<AppSettings>();
                    }
                }
            }
        }
    }
}
