using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FluentAssertions;
using FluentAssertions.Json;
using RuleCheckerSpace;
using CustomExceptions;


namespace UnitTests
{
    [TestClass]
    public class RuleCheckerUnitTests
    {
        /*
         * Scoring with an empty board*/
        [TestMethod]
        public void EmptyBoard()
        {
            //one way to create an empty board
            string[][] board = new string[19][];
            for (int i = 0; i < 19; i++)
                board[i] = new string[19] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };

            //another way to create an empty board
            string[][] aboard = new string[19][]
            {
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "},
                new string[19] {" ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " "}
            };

            JObject jObject = new JObject(
                new JProperty("B", 0),
                new JProperty("W", 0));

            RuleCheckerWrapper.Score(board).Should().BeEquivalentTo(jObject);
        }

        [TestMethod]
        public void SmallBoards()
        {
            string[][] board1 = new string[5][]
            {
                new string[5] {" ", " ", " ", " ", " " },
                new string[5] {" ", " ", " ", " ", " " },
                new string[5] {" ", " ", " ", " ", " " },
                new string[5] {" ", " ", " ", " ", " " },
                new string[5] {" ", " ", " ", " ", " " }
            };

            JObject jObject = new JObject(
                new JProperty("B", 0),
                new JProperty("W", 0));

            RuleCheckerWrapper.Score(board1).Should().BeEquivalentTo(jObject);

            string[][][] boards1 = new string[1][][]
            {
                new string[5][]
                {
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " }
                }
            };

            RuleCheckerWrapper.Play("B", "1-1", boards1).Should().BeTrue();

            string[][][] boards2 = new string[2][][]
            {
                new string[5][]
                {
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " }
                }
                ,
                new string[5][]
                {
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " }
                }
            };

            RuleCheckerWrapper.Play("W", "1-1", boards2).Should().BeTrue();

            string[][][] boards3 = new string[3][][]
            {
                new string[5][]
                {
                    new string[5] {" ", "W", " ", " ", " " },
                    new string[5] {"W", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " }
                },
                new string[5][]
                {
                    new string[5] {" ", "W", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " }
                },
                new string[5][]
                {
                    new string[5] {" ", "W", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " },
                    new string[5] {" ", " ", " ", " ", " " }
                }
            };

            Action act = () => RuleCheckerWrapper.Play("B", "1-1", boards3);
            act.Should().Throw<RuleCheckerException>()
                .WithMessage("Rule 7a violated in RuleChecker: self sacrifice is not allowed");
        }

        #region PeerTests
        private List<string> _test_files = new List<string>();

        [TestMethod]
        //Test all files found in TestFiles/ in the build folder
        public void PeerTests()
        {
            List<string> inputs = new List<string>();
            List<string> outputs = new List<string>();
            DirectorySearch("TestFiles/4");
            foreach (string file in _test_files)
            {
                if (file.Length - file.LastIndexOf('i') == 6)
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

        //Parse json from a file and run it through RuleChecker
        //Returns output of RuleChecker.JsonCommand
        private string TestJson(string filePath)
        {
            string json = ExtractJson(filePath);

            //Parse console input
            List<JToken> jTokenList = ParsingHelper.ParseJson(json);

            List<JToken> finalList = new List<JToken>();
            foreach (JToken jtoken in jTokenList)
            {
                finalList.Add(RuleCheckerAdapter.JsonCommand(jtoken));
            }

            return JsonConvert.SerializeObject(finalList);
        }
        #endregion
    }
}
