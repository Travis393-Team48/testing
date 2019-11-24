using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CustomExceptions;

namespace RuleCheckerSpace
{
    /* Interfaces between the RuleChecker and Json Inputs
     * Provides a method called JsonCommand which is how Json Inputs interact with the RuleChecker
     * Json Commands must be in the format Board or [Stone, Move]
     *      Move can be "pass" or a Play
     *      Play is in the form [Point, Boards]
     *          where Boards is 1-3 board states, representing the current and past 2 game states, ordered from the most recent to the oldest
     * Returns JSON data as a JToken (if input is valid)
     */
    public static class RuleCheckerAdapter
    {
        public static JToken JsonCommand(JToken jtoken)
        {
            JsonValidation.ValidateJTokenRuleChecker(jtoken);

            //If object is a Board...
            if (jtoken.Count() > 2)
                return JToken.Parse(RuleChecker.Score(jtoken.ToObject<string[][]>()).ToString());

            //If object is [Stone, Move] and Move is "pass"
            try
            {
                if (jtoken.ElementAt(1).ToObject<string>() == "pass") //Pass this to RuleChecker
                    return JToken.Parse(JsonConvert.SerializeObject(
                        RuleChecker.Pass()));
            }
            catch { }

            //If object is [Stone, Move] and Move is a Play
            JsonValidation.ValidatePlay(jtoken.ElementAt(1));
            try
            {
                return JToken.Parse(JsonConvert.SerializeObject(
                    RuleChecker.Play(
                        jtoken.ElementAt(0).ToObject<string>(), //Stone
                        jtoken.ElementAt(1).ElementAt(0).ToObject<string>(), //Point
                        jtoken.ElementAt(1).ElementAt(1).ToObject<string[][][]>()))); //Boards
            }
            catch (RuleCheckerException)
            {
                return JToken.Parse(JsonConvert.SerializeObject(false));
            }

            throw new InvalidJsonInputException("Unrecognized JSONCommand passed to RuleCheckerAdapter");
        }
    }
}
