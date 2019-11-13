using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using FluentAssertions.Json;
using PlayerSpace;
using CustomExceptions;

namespace UnitTests
{
    [TestClass]
    public class PlayerProxyUnitTests
    {
        #region Peer Tests
        private List<string> _test_files = new List<string>();
        int _port = 8080;
        
        [TestMethod]
        //Test all files found in TestFiles/7.1 in the build folder
        public void PeerTests()
        {
            List<string> inputs = new List<string>();
            List<string> outputs = new List<string>();
            DirectorySearch("TestFiles/7.1");
            foreach (string file in _test_files)
            {
                if (file.Length - file.LastIndexOf('i') == 6 || file.Length - file.LastIndexOf('i') == 7)
                    inputs.Add(file);
                else
                    outputs.Add(file);
            }

            for (int i = 0; i < inputs.Count; i++)
                JToken.Parse(TestJson(inputs[i])).Should().BeEquivalentTo(
                    JToken.Parse(ExtractJson(outputs[i])));
        }

        //Helper function for peer tests
        private void DirectorySearch(string sDir)
        {
            foreach (string d in Directory.GetDirectories(sDir))
            {
                foreach (string f in Directory.GetFiles(d))
                {
                    _test_files.Add(f);
                }
                DirectorySearch(d);
            }
        }

        //Parse json from a file
        private string ExtractJson(string filePath)
        {
            string json;

            //read from file
            using (StreamReader r = new StreamReader(filePath))
            {
                json = r.ReadToEnd();
            }

            return json;
        }

        //Parse json from a file and run it through Player
        //Returns output of Player.JsonCommand
        private string TestJson(string filePath)
        {
            string json = ExtractJson(filePath);

            List<JToken> finalList = new List<JToken>();

            PlayerAdapter aiPlayer = new PlayerAdapter(true, _port);
            PlayerClient client = new PlayerClient("localhost", _port);

            _port++;

            //Parse console input while testing
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            reader.SupportMultipleContent = true;
            JsonSerializer serializer = new JsonSerializer();

            JToken toAdd;
            while (true)
            {
                //Parse console input while testing
                if (!reader.Read())
                    break;
                JToken jtoken = serializer.Deserialize<JToken>(reader);


                try
                {
                    toAdd = aiPlayer.JsonCommand(jtoken, "no name", "less dumb", 1);
                    if (toAdd.Type != JTokenType.Null)
                        finalList.Add(toAdd);
                }
                catch (InvalidJsonInputException)
                {
                    finalList.Add("GO has gone crazy!");
                    break;
                }
            }

            return JsonConvert.SerializeObject(finalList);
        }

        #endregion

        [TestMethod]
        public void UnitTest1()
        {

        }
    }

}
