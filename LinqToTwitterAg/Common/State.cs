#if !NETFX_CORE
using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace LinqToTwitter
{
    public class State
    {
        public static void Save(string data, string fileName)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.Write(data);
                    }
                }
            }
        }

        public static string Load(string fileName)
        {
            string data = String.Empty;
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.FileExists(fileName))
                {
                    return "<state><consumer_key></consumer_key><consumer_secret></consumer_secret><code></code><access_token></access_token><authorizing></authorizing></state>";
                }

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Open, isf))
                {
                    using (StreamReader sr = new StreamReader(isfs))
                    {
                        string lineOfData = String.Empty;
                        while ((lineOfData = sr.ReadLine()) != null) data += lineOfData;
                    }
                }
            }
            return data;
        }
    }
}

#endif