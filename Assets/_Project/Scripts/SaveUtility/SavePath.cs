using System.IO;
using UnityEngine;

namespace SaveUtility
{
    public static class SavePath
    {
        private static readonly string SAVE_FILE_NAME = "game_save.json";
        private static readonly string SAVE_FILE_SUCCESS_NAME = "success.json";
        private static readonly string FOLDER_SPRITE = "LevelsSprite";
        private static readonly string SAVE_FOLDER_BASE = Application.persistentDataPath;
        private static readonly string SAVE_FOLDER_SPRITES = Path.Combine(SAVE_FOLDER_BASE, FOLDER_SPRITE);
        
        public static string FullPathSaveFile => Path.Combine(SAVE_FOLDER_BASE, SAVE_FILE_NAME);
        public static string FullPathSuccessSaveFile => Path.Combine(SAVE_FOLDER_BASE, SAVE_FILE_SUCCESS_NAME);
        public static string SaveFileName => SaveFileName;
        public static string SaveFolder => SAVE_FOLDER_BASE;
        public static string SaveFolderSprites
        {
            get
            {
                if (!Directory.Exists(SAVE_FOLDER_SPRITES))
                {
                    Directory.CreateDirectory(SAVE_FOLDER_SPRITES);
                }
                return SAVE_FOLDER_SPRITES;
            }
        }

        public static string GetLevelID(string author, string title) => $"{author}_{title}";
        public static bool SaveSuccessExists => File.Exists(FullPathSuccessSaveFile);
    }
}
