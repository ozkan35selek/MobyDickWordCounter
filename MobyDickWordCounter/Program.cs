using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace MobyDickWordCounter
{
    class Program
    {
        //ana fonksiyon
        static void Main(string[] args)
        {
            string mobyDickBookFullText = ReadMobyDickText();
            ReadMobyDickBook(mobyDickBookFullText);
        }

        //kitap içinden kelimeleri sayar
        public static void ReadMobyDickBook(string text)
        {
            Regex RegExp = new Regex("[^a-zA-Z0-9]");
            text = RegExp.Replace(text, " ");
            string[] words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var wordQuery = (from string word in words orderby word select word);
            string[] result = wordQuery.ToArray();
            if (result != null && result.Length > 0)
            {
                Dictionary<string, int> dict = new Dictionary<string, int>();
                for (int i = 0; i < result.Length; i++)
                {
                    if (dict.ContainsKey(result[i]))
                    {
                        dict[result[i]]++;
                    }
                    else
                    {
                        dict.Add(result[i], 1);
                    }
                }
                var sortedDict = from entry in dict orderby entry.Value descending select entry;
                SerializeDictToXml(sortedDict.ToDictionary(r => r.Key, r => r.Value));
            }
        }

        //ilgili kitabı text olarak url üzerinden okur.
        public static string ReadMobyDickText()
        {
            WebClient client = new WebClient();
            return client.DownloadString("http://www.gutenberg.org/files/15/text/moby-041.txt");
        }

        //Dictionaryden xmle çeviren metod
        public static void SerializeDictToXml(Dictionary<string, int> dict)
        {

            XElement root = new XElement("words");

            foreach (var pair in dict)
            {
                XElement cElement = new XElement("word");
                cElement.SetAttributeValue("text", pair.Key);
                cElement.SetAttributeValue("count", pair.Value);
                root.Add(cElement);
            }
            root.Save("mobydickwords.xml");
            Console.WriteLine("Dosya proje bin klasörüne mobydickwords.xml adıyla oluşturulmuştur");
        }

    }
}
