﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using FluentAssertions.Json;
using BoardSpace;


namespace UnitTests
{
    [TestClass]
    public class BoardUnitTests
    {
        private List<string> _test_files = new List<string>();

        [TestMethod]
        public void ParsePointTests()
        {
            Assert.AreEqual(ParsingHelper.ParsePoint("1-1")[0], 0);
            Assert.AreEqual(ParsingHelper.ParsePoint("1-1")[1], 0);
            Assert.AreEqual(ParsingHelper.ParsePoint("19-19")[0], 18);
            Assert.AreEqual(ParsingHelper.ParsePoint("19-19")[1], 18);
            Assert.AreEqual(ParsingHelper.ParsePoint("15-19")[0], 14);
            Assert.AreEqual(ParsingHelper.ParsePoint("15-19")[1], 18);
            Assert.AreEqual(ParsingHelper.ParsePoint("15-5")[0], 14);
            Assert.AreEqual(ParsingHelper.ParsePoint("15-5")[1], 4);
        }

        [TestMethod]
        //Test all files found in TestFiles/ in the build folder
        public void PeerTests()
        {
            List<string> inputs = new List<string>();
            List<string> outputs = new List<string>();
            DirectorySearch("TestFiles/3");
            foreach (string file in _test_files)
            {
                if (file.Length - file.LastIndexOf('i') == 6)
                    inputs.Add(file);
                else
                    outputs.Add(file);
            }

            for (int i = 0; i < inputs.Count; i++)
                JToken.Parse(TestJson(inputs[i])).Should().BeEquivalentTo(
                    JToken.Parse(ParseJson(outputs[i])));
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
        private string ParseJson(string filePath)
        {
            string json;

            //read from file
            using (StreamReader r = new StreamReader(filePath))
            {
                json = r.ReadToEnd();
            }

            return json;
        }

        //Parse json from a file and run it through BoardWrapper
        //Returns output of BoardWrapper.JsonCommand
        private string TestJson(string filePath)
        {
            List<JToken> jTokenList = new List<JToken>();
            string brokenObject = "";

            string json = ParseJson(filePath);

            //Parse console input
            foreach (char character in json)
            {
                brokenObject += character;
                try
                {
                    jTokenList.Add(JToken.Parse(brokenObject));
                    brokenObject = "";
                }
                catch { }
            }

            List<JToken> finalList = new List<JToken>();
            foreach (JToken jtoken in jTokenList)
            {
                BoardAdapter boardWrapper = new BoardAdapter();
                finalList.Add(boardWrapper.JsonCommand(jtoken));
            }

            return JsonConvert.SerializeObject(finalList);
        }
    }
}
