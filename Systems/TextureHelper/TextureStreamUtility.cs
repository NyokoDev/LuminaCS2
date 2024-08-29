using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lumina.Systems.TextureHelper
{
    class TextureStreamUtility
    {
        public static Texture2D LoadTextureFromFile(string path, bool readOnly = false)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            try
            {
                using (var textureStream = File.OpenRead(path))
                {
                    return LoadTextureFromStream(readOnly, textureStream);
                }
            }
            catch (Exception e)
            {
                Lumina.Mod.Log.Error(e);
                return null;
            }
        }

        private static Texture2D LoadTextureFromStream(bool readOnly, Stream textureStream)
        {
            var buf = new byte[textureStream.Length]; //declare arraysize
            textureStream.Read(buf, 0, buf.Length); // read from stream to byte array
            textureStream.Close();
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            tex.LoadImage(buf);
            tex.name = Guid.NewGuid().ToString();
            tex.filterMode = FilterMode.Trilinear;
            tex.anisoLevel = 9;
            tex.Apply(false, readOnly);
            return tex;
        }

    }
}
