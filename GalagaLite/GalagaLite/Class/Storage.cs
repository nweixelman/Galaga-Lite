﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace GalagaLite.Class
{
    class Storage
    {
        public static StorageFolder StorageFolder = ApplicationData.Current.LocalFolder;
        public static async void CreateFile()
        {
            try
            {
                await StorageFolder.CreateFileAsync("SpaceGame.txt", CreationCollisionOption.OpenIfExists);
            }
            catch
            {

            }

        }
        public static async void ReadFile()
        {
            try
            {
                StorageFile DataFile = await StorageFolder.GetFileAsync("SpaceGame.txt");
                MainPage.STRHighScore = await FileIO.ReadTextAsync(DataFile);
            }
            catch
            {

            }
        }
        public static async void UpdateScore()
        {
            if (MainPage.MyScore > Convert.ToInt16(MainPage.STRHighScore))
            {
                try
                {
                    StorageFile DataFile = await StorageFolder.GetFileAsync("SpaceGame.txt");
                    await FileIO.WriteTextAsync(DataFile, MainPage.MyScore.ToString());
                    ReadFile();
                }
                catch
                {

                }

            }

        }
    }
}
