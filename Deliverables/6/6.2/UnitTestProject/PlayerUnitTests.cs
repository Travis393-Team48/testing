using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using FluentAssertions.Json;
using PlayerSpace;
using RuleCheckerSpace;


namespace UnitTests
{
    [TestClass]
    public class PlayerUnitTests
    {
        private List<string> _test_files = new List<string>();

        [TestMethod]
        //Test all files found in TestFiles/5.1 in the build folder
        public void PeerTestsOne()
        {
            List<string> inputs = new List<string>();
            List<string> outputs = new List<string>();
            DirectorySearch("TestFiles/5.1");
            foreach (string file in _test_files)
            {
                if (file.Length - file.LastIndexOf('i') == 6)
                    inputs.Add(file);
                else
                    outputs.Add(file);
            }

            for (int i = 0; i < inputs.Count; i++)
                JToken.Parse(TestJson(inputs[i], "dumb")).Should().BeEquivalentTo(
                    JToken.Parse(ExtractJson(outputs[i])));
        }

        [TestMethod]
        //Test all files found in TestFiles/5.2 in the build folder
        public void PeerTestsTwo()
        {
            List<string> inputs = new List<string>();
            List<string> outputs = new List<string>();
            DirectorySearch("TestFiles/5.2");
            foreach (string file in _test_files)
            {
                if (file.Length - file.LastIndexOf('i') == 6)
                    inputs.Add(file);
                else
                    outputs.Add(file);
            }

            for (int i = 0; i < inputs.Count; i++)
                JToken.Parse(TestJson(inputs[i], "less dumb")).Should().BeEquivalentTo(
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
        private string TestJson(string filePath, string AIType)
        {
            string json = ExtractJson(filePath);

            //Parse console input
            List<JToken> jTokenList = ParsingHelper.ParseJson(json);
            List<JToken> finalList = new List<JToken>();

            PlayerAdapter aiPlayer = new PlayerAdapter();
            JToken toAdd;
            foreach (JToken jtoken in jTokenList)
            {
                toAdd = aiPlayer.JsonCommand(jtoken, "no name", AIType);
                if (toAdd.Type != JTokenType.Null)
                    finalList.Add(toAdd);
            }

            return JsonConvert.SerializeObject(finalList);
        }
    }
}
