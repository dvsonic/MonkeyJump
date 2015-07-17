﻿using UnityEngine;
using System.Collections;
using System.Security;
using UnityEngine.UI;
using System.Text;

public class GameData 
{
    public static int score = 0;
    public static int hp;
    public const int MAX_HP = 3;
    public static int blockNum;
    public static bool isStart;

    /*private static XmlNode _language;
    public static XmlNode getLanguage()
    {
        if(null == _language)
        {
            string language = "config/language_en";
            if(Application.systemLanguage == SystemLanguage.Chinese)
                language = "config/language_cn";
            string data = Resources.Load(language).ToString();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            _language = doc.SelectSingleNode("language");
        }
        return _language;
    }*/

    private static Hashtable _language;
    public static string getLanguage(string key)
    {
        if (null == _language)
        {
            string language = "config/language_en";
            if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
               language = "config/language_cn";
            TextAsset asset = Resources.Load(language) as TextAsset;
            string data = Encoding.UTF8.GetString(asset.bytes);
            string[] ary = data.Split('\n');
            _language = new Hashtable();
            for (int i = 0; i < ary.Length; i++)
            {
                string str = ary[i];
                string[] ary2 = str.Split('\t');
                _language.Add(ary2[0], ary2[1]);
            }
        }
        return _language[key].ToString().Replace("<br>", "\n");
    }
}
