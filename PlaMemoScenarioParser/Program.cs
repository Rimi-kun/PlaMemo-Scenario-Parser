using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PlaMemoScenarioParser
{
    class Program
    {
        private static readonly string currentDirectory = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (!Directory.Exists(string.Concat(currentDirectory, "\\", "parsed")))
                    Directory.CreateDirectory(string.Concat(currentDirectory, "\\", "parsed"));

                foreach (string path in args)
                {
                    if (Path.GetExtension(path) == ".json")
                        Parse(path);
                }

                Console.WriteLine("Finished!");
            }
        }

        private static void Parse(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path).Replace(".txt", string.Empty);
            string completeName = string.Concat(currentDirectory, "\\", "parsed", "\\", fileName, ".txt");

            JObject data = JObject.Parse(File.ReadAllText(path));
            int scnLength = data["scenes"].Count();

            File.Create(completeName).Close();

            using (StreamWriter streamWriter = new StreamWriter(completeName))
            {
                for (int texts = 0; texts < scnLength; texts++)
                {
                    try
                    {
                        for (int i = 0; i < data["scenes"][texts]["texts"].Count(); i++)
                        {
                            if (string.IsNullOrEmpty((string)data["scenes"][texts]["texts"][i][0]))
                            {
                                string scns = (string)data["scenes"][texts]["texts"][i][2];
                                streamWriter.WriteLine(scns + "\n\n");
                            }
                            else
                            {
                                string scns = string.Concat(data["scenes"][texts]["texts"][i][0], ": ", data["scenes"][texts]["texts"][i][2]);
                                streamWriter.WriteLine(scns + "\n\n");
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            string scns = string.Concat("[", data["scenes"][texts]["selects"][0]["text"], "] OR [", data["scenes"][texts]["selects"][1]["text"], "]");
                            streamWriter.WriteLine(scns + "\n\n");
                        }
                        catch
                        { }
                    }
                }
            }

            Console.WriteLine(fileName + " parsed.");
        }
    }
}
