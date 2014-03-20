using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;


namespace HexStrategy
{
    /// <summary>
    /// Non static class allows easy save file IO
    /// </summary>
    public class PersistentStorage
    {
        StorageDevice device;
        string containerName = "HexStrategy";
        string filename = "World1.xml";


        public void InitiateSave()
        {
            device = null;
            StorageDevice.BeginShowSelector(PlayerIndex.One, this.SaveToDevice, null);
        }
        void SaveToDevice(IAsyncResult result)
        {

            device = StorageDevice.EndShowSelector(result);
            if (device != null && device.IsConnected)
            {
                WorldData worldData = new WorldData();

                IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = device.EndOpenContainer(r);
                if (container.FileExists(filename))
                    container.DeleteFile(filename);
                Stream stream = container.CreateFile(filename);

                XmlSerializer serializer = new XmlSerializer(worldData.GetType());
                serializer.Serialize(stream, worldData);
                stream.Close();
                container.Dispose();
                result.AsyncWaitHandle.Close();
                Logger.AddMessage("Saved to " + containerName + "/" + filename);
            }
        }
        public void InitiateLoad()
        {

            device = null;
            StorageDevice.BeginShowSelector(PlayerIndex.One, this.LoadFromDevice, null);

        }
        void LoadFromDevice(IAsyncResult result)
        {

            device = StorageDevice.EndShowSelector(result);
            IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(r);
            result.AsyncWaitHandle.Close();
            if (container.FileExists(filename))
            {
                Stream stream = container.OpenFile(filename, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(WorldData));
                WorldData worldData = (WorldData)serializer.Deserialize(stream);
                stream.Close();
                container.Dispose();
                Core.LoadFrom(worldData);
                Core.Reconstruct();
            }

        }



    }
}
