using UnityEngine;
using System.Collections;
using System.IO;

namespace mytest2.Utils
{
    public class LocalizationManager
    {
        public enum Languages : int { english = 0, max = 1 };//, russian = 1, max = 2 };

        private static Hashtable[] textTables;
        private static Hashtable[] texturesTables;

        public static Languages defaultLanguage = Languages.english;
        public static Languages language;

        static LocalizationManager()
        {
            textTables = new Hashtable[(int)Languages.max];
            texturesTables = new Hashtable[(int)Languages.max];

            for (int i = (int)Languages.english; i < (int)Languages.max; ++i)
            {
                textTables[i] = new Hashtable();
                texturesTables[i] = new Hashtable();
            }

            SetLocalLanguage();
        }

        static void SetLocalLanguage()
        {
            int cL = PlayerPrefs.GetInt("currentLanguage", -1);
            if (cL != -1)
            {
                CurrentLanguage = cL;
            }
            else
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.Russian:
                    case SystemLanguage.Ukrainian:
                    default:
                        CurrentLanguage = (int)Languages.english;
                        break;
                }
            }
        }

        public static int CurrentLanguage
        {
            get { return (int)language; }
            set
            {
                language = (Languages)value;
                SetupLanguage(language);
            }
        }
        public static int DefaultLanguage
        {
            get { return (int)defaultLanguage; }
        }

        static void SetupLanguage(Languages language)
        {
            LoadTextResources(language);
            LoadTextureResources(language);
        }

        private static bool LoadTextResources(Languages language)
        {
            string fullpath = "Localization/" + language.ToString() + ".po";

            TextAsset textAsset = (TextAsset)Resources.Load(fullpath);
            if (textAsset == null)
            {
                Debug.LogError("\"[LocalizationManager] \" + fullpath + \" file not found.");
                return false;
            }

            StringReader reader = new StringReader(textAsset.text);
            string key = null;
            string val = null;
            string line;

            textTables[(int)language].Clear();

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("msgid \""))
                    key = line.Substring(7, line.Length - 8);
                else if (line.StartsWith("msgstr \""))
                    val = line.Substring(8, line.Length - 9);
                else
                {
                    if (key != null && val != null)
                    {
                        if (!textTables[(int)language].Contains(key))
                        {
                            textTables[(int)language].Add(key, val);
                            key = val = null;
                        }
                    }
                }
            }

            reader.Close();
            return true;
        }

        private static bool LoadTextureResources(Languages language)
        {
            string fullpath = "Localization/" + language.ToString();

            Object[] textures = Resources.LoadAll(fullpath) as Object[];

            texturesTables[(int)language].Clear();
            for (int i = 0; i < textures.Length; ++i)
            {
                if (!texturesTables[(int)language].ContainsValue(textures[i]))
                    texturesTables[(int)language].Add(textures[i].name, textures[i]);
            }

            return true;
        }

        public static string GetText(string key)
        {
            if (key != null)
            {
                if (!textTables[CurrentLanguage].ContainsKey(key))
                {
                    if (textTables[DefaultLanguage].ContainsKey(key))
                        return ((string)textTables[DefaultLanguage][key]).Replace("\\n", "\n");

                    Debug.LogError(string.Format("[LocalizationManager]->GetText() : key \"{0}\" not found in any localization", key));
                    return string.Empty;
                }

                return ((string)textTables[CurrentLanguage][key]).Replace("\\n", "\n");
            }

            Debug.LogError("[LocalizationManager]->GetText() Key == null");
            return string.Empty;
        }
    }
}

