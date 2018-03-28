using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Autodesk.DesignScript.Runtime;

namespace dtRegularExpression
{
    public class RegEx
    {
        internal RegEx()
        {

        }

        #region CreateSchema
            /// <summary>
            /// Constructs a RegEx Schema
            /// </summary>
            /// <param name="classType">type of class to build</param>
            /// <param name="match">item to match</param>
            /// <param name="ignoreCase"></param>
            /// <param name="numbersAllowed">numbers allowed</param>
            /// <param name="specialCharAllowed">special characters allowed</param>
            /// <param name="minMatch">minimum amount of characters</param>
            /// <param name="maxMatch">maximum amount of characters</param>
            /// <param name="count">number of characters</param>
            /// <returns name="schema">string</returns>
            /// <search>
            /// logic, core, regex, regular, expression, search, class, type, build, match, string
            /// </search>
            public static object CreateSchema(string classType, [DefaultArgument("{}")] List<string> match, bool ignoreCase = true, bool numbersAllowed = true, bool specialCharAllowed = true, int minMatch = 0, int maxMatch = 10, int count = 3)
        {
            List<string> upperMatch = new List<string>();
            List<string> lowerMatch = new List<string>();

            if (match.Count >= 2)
            {
                foreach (string m in match)
                {
                    upperMatch.Add(m.ToUpper());
                    lowerMatch.Add(m.ToLower());
                }
            }
            else if (match.Count == 1)
            {
                upperMatch.Add(match[0].ToUpper());
                upperMatch.Add(match[0].ToLower());
            }
            else
            {
            }

            List<string> specCharList = new List<string> { " ", "!", "\"", "#", "£", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "@", "{", "\\", "}", "^", "_", "`", "\\[", "|", "\\]", "~" };
            string specChar = System.String.Join("", specCharList);

            List<string> classTypeList = new List<string>() { classType };
            List<int> minMatchList = new List<int>() { minMatch };
            List<int> maxMatchList = new List<int>() { maxMatch };
            List<int> countList = new List<int>() { count };

            if (classType == "ClassType.Fixed")
            {
                if (classTypeList.Count != match.Count)
                {
                    string output = "Error - Ensure input structures match!";
                    return output;
                }
                else
                {
                    if (ignoreCase)
                    {
                        string output = "(?:" + match[0].ToLower() + "|" + match[0].ToUpper() + ")$";
                        return output;
                    }
                    else
                    {
                        string output = "(?:" + match[0] + ")$";
                        return output;
                    }
                }
            }
            else if (classType == "ClassType.List")
            {
                if (match.Count <= 1)
                {
                    string output = "Error - Ensure list input of matches is correct!";
                    return output;
                }
                else
                {
                    if (ignoreCase)
                    {
                        string output = "(?:" + System.String.Join("|", lowerMatch) + "|" + System.String.Join("|", upperMatch) + ")$";
                        return output;
                    }
                    else
                    {
                        string output = "(?:" + System.String.Join("|", match) + ")$";
                        return output;
                    }
                }
            }
            else if (classType == "ClassType.Varies")
            {
                if (numbersAllowed && specialCharAllowed)
                {
                    string output = ".";
                    return output;
                }
                else if (numbersAllowed && specialCharAllowed == false)
                {
                    string output = "^([^" + specChar + "]*)$";
                    return output;
                }
                else if (numbersAllowed == false && specialCharAllowed)
                {
                    string output = "^([^0-9]*)$";
                    return output;
                }
                else
                {
                    string output = "^([^0-9" + specChar + "]*)$";
                    return output;
                }
            }
            else if (classType == "ClassType.Minimum")
            {
                if (classTypeList.Count != minMatchList.Count)
                {
                    string output = "Error - Ensure minimum value is correct!";
                    return output;
                }
                else
                {
                    if (numbersAllowed && specialCharAllowed)
                    {
                        string output = "^(.{" + minMatch.ToString() + ",})$";
                        return output;
                    }
                    else if (numbersAllowed && specialCharAllowed == false)
                    {
                        string output = "^([^" + specChar + "]{" + minMatch.ToString() + ",})$";
                        return output;
                    }
                    else if (numbersAllowed == false && specialCharAllowed)
                    {
                        string output = "^([^0-9]{" + minMatch.ToString() + ",})$";
                        return output;
                    }
                    else
                    {
                        string output = "^([^0-9" + specChar + "]{" + minMatch.ToString() + ",})$";
                        return output;
                    }
                }
            }
            else if (classType == "ClassType.Maximum")
            {
                if (classTypeList.Count != maxMatchList.Count)
                {
                    string output = "Error - Ensure maximum value is correct!";
                    return output;
                }
                else
                {
                    if (numbersAllowed && specialCharAllowed)
                    {
                        string output = "^(.{0," + maxMatch.ToString() + "})$";
                        return output;
                    }
                    else if (numbersAllowed && specialCharAllowed == false)
                    {
                        string output = "^([^" + specChar + "]{0," + maxMatch.ToString() + "})$";
                        return output;
                    }
                    else if (numbersAllowed == false && specialCharAllowed)
                    {
                        string output = "^([^0-9]{0," + maxMatch.ToString() + "})$";
                        return output;
                    }
                    else
                    {
                        string output = "^([^0-9" + specChar + "]{0," + maxMatch.ToString() + "})$";
                        return output;
                    }
                }
            }
            else if (classType == "ClassType.Range")
            {
                if ((classTypeList.Count != minMatchList.Count) && (minMatchList.Count != maxMatchList.Count))
                {
                    string output = "Error - Ensure minimum and maximum values are correct!";
                    return output;
                }
                else
                {
                    if (numbersAllowed && specialCharAllowed)
                    {
                        string output = "^(.{" + minMatch.ToString() + "," + maxMatch.ToString() + "})$";
                        return output;
                    }
                    else if (numbersAllowed && specialCharAllowed == false)
                    {
                        string output = "^([^" + specChar + "]{" + minMatch.ToString() + "," + maxMatch.ToString() + "})$";
                        return output;
                    }
                    else if (numbersAllowed == false && specialCharAllowed)
                    {
                        string output = "^([^0-9]{" + minMatch.ToString() + "," + maxMatch.ToString() + "})$";
                        return output;
                    }
                    else
                    {
                        string output = "^([^0-9" + specChar + "]{" + minMatch.ToString() + "," + maxMatch.ToString() + "})$";
                        return output;
                    }
                }
            }
            else if (classType == "ClassType.Count")
            {
                if (classTypeList.Count != countList.Count)
                {
                    string output = "Error - Ensure count value is correct!";
                    return output;
                }
                else
                {
                    if (numbersAllowed && specialCharAllowed)
                    {
                        string output = "^(.{" + count.ToString() + "})$";
                        return output;
                    }
                    else if (numbersAllowed && specialCharAllowed == false)
                    {
                        string output = "^([^" + specChar + "]{" + count.ToString() + "})$";
                        return output;
                    }
                    else if (numbersAllowed == false && specialCharAllowed)
                    {
                        string output = "^([^0-9]{" + count.ToString() + "})$";
                        return output;
                    }
                    else
                    {
                        string output = "^([^0-9" + specChar + "]{" + count.ToString() + "})$";
                        return output;
                    }
                }
            }

            else
            {
                string output = "Error - Select valid class type";
                return output;
            }
        }
        #endregion

        #region PasswordSchema
        /// <summary>
        /// Constructs a RegEx Password Schema
        /// </summary>
        /// <param name="oneNumberRequired">bool</param>
        /// <param name="oneLowercaseLetterRequired">bool</param>
        /// <param name="oneUppercaseLetterRequired">bool</param>
        /// <param name="oneSpecialCharacterRequired">bool</param>
        /// <param name="minMatch">minimum amount of characters</param>
        /// <param name="maxMatch">maximum amount of characters</param>
        /// <returns name="schema">string</returns>
        /// <search>
        /// logic, core, regex, regular, expression, search, class, type, build, match, string, password
        /// </search>
        public static object PasswordSchema(bool oneNumberRequired = false, bool oneLowercaseLetterRequired = false, bool oneUppercaseLetterRequired = false, bool oneSpecialCharacterRequired = false, int minMatch = 0, int maxMatch = 100)
        {
            List<string> specCharList = new List<string> { " ", "!", "\"", "#", "$", "%", "&", "'", "(", ")", "*", "+", ",", "-", ".", "/", ":", ";", "<", "=", ">", "?", "@", "{", "\\", "}", "^", "_", "`", "\\[", "|", "\\]", "~" };
            string specChar = System.String.Join("", specCharList);

            object nums = dtCore.Logic.If(oneNumberRequired, "(?=.*[0-9])", "");
            object lcase = dtCore.Logic.If(oneLowercaseLetterRequired, "(?=.*[a-z])", "");
            object ucase = dtCore.Logic.If(oneUppercaseLetterRequired, "(?=.*[A-Z])", "");
            object schar = dtCore.Logic.If(oneSpecialCharacterRequired, "(?=.*[" + specChar + "])", "");
            string range = ".{" + minMatch.ToString() + "," + maxMatch.ToString() + "}";

            return "^" + nums + lcase + ucase + schar + range + "$";
        }

        #endregion

        #region Validate
        /// <summary>
        /// Use a RegEx schema to validate data 
        /// </summary>
        /// <param name="separator">the separator between the data sections</param>
        /// <param name="data">data to check</param>
        /// <param name="schema">RegEx Schema, either a single schema or a list of schema's</param>
        /// <search>
        /// logic, core, regex, regular, expression, search, match, schema, match, string, validated. unvalidated, data, validate
        /// </search>
        [MultiReturn(new[] { "validatedData", "unvalidatedData" })]
        public static Dictionary<string, object> Validate(List<string> data, List<string> schema, string separator = "")
        {
            List<string> validatedData = new List<string>();
            List<string> unvalidatedData = new List<string>();

            if (separator == "")
            {
                if (schema.Count == 0)
                {
                    Dictionary<string, object> newOutput;
                    newOutput = new Dictionary<string, object>
                    {
                        {"validatedData","Error - Use Correct Amount of Schema's!"},
                        {"unvalidatedData","Error - Use Correct Amount of Schema's!"}
                    };
                    return newOutput;
                }
                else if (schema.Count == 1)
                {
                    var repSchema = Enumerable.Repeat(schema[0], data.Count).ToList();
                    var match = data.Zip(repSchema, (d, r) => Regex.IsMatch(d, r));
                    List<string> vd = data.Zip(match, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter == true).Select(item => item.name).ToList();
                    List<string> unvd = data.Zip(match, (name, filter) => new { name = name, filter = filter, }).Where(item => item.filter ==  false).Select(item => item.name).ToList();

                    Dictionary<string, object> newOutput;
                    newOutput = new Dictionary<string, object>
                    {
                        {"validatedData",vd},
                        {"unvalidatedData",unvd}
                    };
                    return newOutput;
                }
                else
                {
                    var match = data.Zip(schema, (d, s) => Regex.IsMatch(d, s));
                    Dictionary<string, object> newOutput;
                    newOutput = new Dictionary<string, object>
                    {
                        {"validatedData","Error - Use Correct Schema"},
                        {"unvalidatedData","Error - Use Correct Schema"}
                    };
                    return newOutput;
                }
            }
            else
            {
                char[] newSep = separator.ToCharArray();
                var repSep = Enumerable.Repeat(newSep[0], data.Count).ToList();

                var splitList = data.Zip(repSep, (d, s) => d.Split(s));
                List<int> totalCount = new List<int>();
                foreach (var s in splitList)
                {
                    totalCount.Add(s.Count());
                }

                List<string> countPass = new List<string>();
                for (int i = 0; i < totalCount.Count; i++)
                {
                    if (totalCount[i] == schema.Count)
                    {
                        countPass.Add(data[i]);
                    }
                    else
                    {
                        unvalidatedData.Add(data[i]);
                    }
                }

                var repSep2 = Enumerable.Repeat(newSep[0], countPass.Count).ToList();
                var splitList2 = countPass.Zip(repSep2, (d, s) => d.Split(s));

                var repSch = Enumerable.Repeat(schema, splitList2.Count()).ToList();
                var flatRepSch = repSch.SelectMany(i => i);
                var flatSplitList2 = splitList2.SelectMany(i => i);

                var boolList = flatSplitList2.Zip(flatRepSch, (d, r) => Regex.IsMatch(d, r));

                List<IEnumerable<bool>> listofBools = new List<IEnumerable<bool>>();
                for (int i = 0; i < boolList.Count(); i += schema.Count)
                {
                    listofBools.Add(boolList.Skip(i).Take(schema.Count));
                }

                List<string> regexPass = new List<string>();
                for (int i = 0; i < listofBools.Count; i++)
                {
                    if (listofBools[i].All(b => b))
                    {
                        validatedData.Add(countPass[i]);
                    }
                    else
                    {
                        unvalidatedData.Add(countPass[i]);
                    }
                }

                Dictionary<string, object> newOutput;
                newOutput = new Dictionary<string, object>
                {
                    {"validatedData",validatedData},
                    {"unvalidatedData",unvalidatedData}
                };
                return newOutput;

            }
            #endregion

        }
    }
}
