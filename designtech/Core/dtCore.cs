using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using Autodesk.DesignScript.Runtime;

namespace dtCore
{
    public class Logic
    {
        internal Logic()
        {

        }

        #region If
        /// <summary>
        /// Retains the old status of the previous built-in IF node.
        /// Depending on if the test input is set to true or false it will pass through whats in the respective inputs.
        /// </summary>
        /// <param name="test">boolean test</param>
        /// <param name="ifTrue">result if true</param>
        /// <param name="ifFalse">result if false</param>
        /// <param name="result">output</param>
        /// <search>
        /// logic, core, if, true, false, test
        /// </search>
        public static object If(bool test, object ifTrue, object ifFalse)
        {
            if (test)
            {
                return ifTrue;
            }
            else
            {
                return ifFalse;
            }
        }
        #endregion

        #region IfEqualReturnIndex
        /// <summary>
        /// Returns the index of items in the list/s that match
        /// </summary>
        /// <param name="item1">first items</param>
        /// <param name="item2">second items</param>
        /// <param name="index">matching indicies</param>
        /// <search>
        /// logic, core, if, equal, return, index, indicies, item
        /// </search>
        public static object IfEqualReturnIndex(List<object> item1, List<object> item2)
        {
            List<object> indexList = new List<object>();
            foreach (object i in item2)
            {
                indexList.Add(item1.IndexOf(i));
            }
            return indexList;

        }
        #endregion

    }

    public class String
    {
        internal String()
        {

        }

        #region AddLeadingZeros
        /// <summary>
        /// Takes an input of a series of numbers, finds the length of the maximum number and then adds leading zero’s to any numbers of less digits till all numbers in the list have the same digits.
        /// 
        /// As an example if you feed in the numbers 1, 10 and 1000, it will give you the results 0001, 0010, 1000. Lists can be in any order as it searches for the maximum item.
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns name="numbers">string</returns>
        /// <search>string,add,leading,zeros,0</search>
        public static List<string> AddLeadingZeros(List<int> numbers)
        {
            List<int> lengths = new List<int>();
            foreach (int i in numbers)
            {
                string str = (System.Math.Abs(i)).ToString();
                int len = str.Length;
                lengths.Add(len);
            }
            int max = lengths.Max();

            List<string> output = new List<string>();
            if (max <= 1)
            {
                foreach (int i in numbers)
                {
                    output.Add("0" + i.ToString());
                }
            }
            else
            {
                foreach (int i in numbers)
                {
                    string format = "D" + max.ToString();
                    output.Add(i.ToString(format));
                }
            }
        
            return output;
        }
        #endregion

        #region Alphabet
        /// <summary>
        /// Gives the Alphabet as a string and list.
        /// </summary>
        /// <param name></param>
        /// <returns name="strLower">alphabet as string in lower case</returns>
        /// <returns name="strUpper">alphabet as string in upper case</returns>
        /// <returns name="listLower">alphabet as list in lower case</returns>
        /// /// <returns name="listUpper">alphabet as list in upper case</returns>
        /// <search>string,alphabet,list,upper,lower</search>
        [MultiReturn(new[] { "strLower", "strUpper", "listLower", "listUpper" })]
        public static IDictionary<string, object> Alphabet()
        {
            string strLower = "abcdefghijklmnopqrstuvwxyz";
            string strUpper = DSCore.String.ToUpper(strLower);

            List<string> strHolderLower = new List<string>();
            foreach (char ch in strLower)
            {
                strHolderLower.Add(ch.ToString());
            }

            List<string> strHolderUpper = new List<string>();
            foreach (char ch in strUpper)
            {
                strHolderUpper.Add(ch.ToString());
            }

            return new Dictionary<string, object>()
            {
                {"strLower",strLower},
                {"strUpper",strUpper},
                {"listLower",strHolderLower},
                {"listUpper",strHolderUpper}
            };
        }
        #endregion

        #region ChangeCharacterAtIndexToLower
        /// <summary>
        /// Changes the character at the given index to lower case
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <returns name="str">string</returns>
        /// <search>string,change,index,lower,case,character</search>
        public static object ChangeCharacterAtIndexToLower(string str, int index)
        {
            string item = String.ReturnCharacterAtIndex(str, index);
            string lowerStr = item.ToLower();
            char lowerChar = lowerStr[0];
            StringBuilder builder = new StringBuilder(str);
            builder[index] = lowerChar;
            return builder.ToString();
        }
        #endregion

        #region ChangeCharacterAtIndexToUpper
        /// <summary>
        /// Changes the character at the given index to upper case
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <returns name="str">string</returns>
        /// <search>string,change,index,upper,case,character</search>
        public static object ChangeCharacterAtIndexToUpper(string str, int index)
        {
            string item = String.ReturnCharacterAtIndex(str, index);
            string upperStr = item.ToUpper();
            char upperChar = upperStr[0];
            StringBuilder builder = new StringBuilder(str);
            builder[index] = upperChar;
            return builder.ToString();
        }
        #endregion

        #region CreateGUID
        /// <summary>
        /// Creates a randomised GUID (globally unique indentifier). A GUID is a 128-bit integer number used to identify resources.
        /// </summary>
        /// <returns name="guid">string</returns>
        /// <search>string,create,guid,global,unique,indentifier,globally,128-bit,integer,indentify</search>
        public static object CreateGUID()
        {
            Guid g;
            g = Guid.NewGuid();
            return g;
        }
        #endregion

        #region CreateRandom
        /// <summary>
        /// Creates a randomised string.
        /// </summary>
        /// <param name="length"></param>
        /// <returns name="str">string</returns>
        /// <search>string,create,random,randomise,unique</search>
        public static object CreateRandom(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        #endregion

        #region CreateListBasedOnReturns
        /// <summary>
        /// Creates a list based on the returns within the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns name="str">string</returns>
        /// <search>string,list,index,return</search>
        public static object CreateListBasedOnReturns(string str)
        {
            var split = DSCore.String.Split(str, "\r\n");
            return split;
        }
        #endregion

        #region ContainsThisAndThat
        /// <summary>
        /// Returns true or false based on whether the string contains all the inputs (mulitple input as a list)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="searchFor"></param>
        /// <param name="ignoreCase"></param>
        /// <returns name="str">string</returns>
        /// <search>string,contains,this,and,that,case,search</search>
        public static object ContainsThisAndThat(string str, List<string> searchFor, bool ignoreCase = false)
        {
            int count = searchFor.Count();
            var boolList = new List<bool>();
            for (int i = 0; i < count; i++)
            {
                //bool contains = str.Contains(searchFor[i]);
                bool contains = DSCore.String.Contains(str, searchFor[i], ignoreCase);
                boolList.Add(contains);
            }
            bool alltrue = boolList.TrueForAll(b => b);
            return alltrue;
        }
        #endregion

        #region ContainsThisOrThat
        /// <summary>
        /// Returns true or false based on whether the string contains any of the inputs (mulitple input as a list)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="searchFor"></param>
        /// <param name="ignoreCase"></param>
        /// <returns name="str">string</returns>
        /// <search>string,contains,this,or,that,case,search</search>
        public static object ContainsThisOrThat(string str, List<string> searchFor, bool ignoreCase = false)
        {
            int count = searchFor.Count();
            var boolList = new List<bool>();
            for (int i = 0; i < count; i++)
            {
                //bool contains = str.Contains(searchFor[i]);
                bool contains = DSCore.String.Contains(str, searchFor[i], ignoreCase);
                boolList.Add(contains);
            }
            bool anyTrue = boolList.Any(b => b);
            return anyTrue;
        }
        #endregion

        #region DropFirstWord
        /// <summary>
        /// Removes the set of characters upto the first whitespace in the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns name="str">string</returns>
        /// <search>string,word,remove,drop,first</search>
        public static object DropFirstWord(string str)
        {
            string trim = str.TrimStart(" ".ToCharArray());
            int Cnt = trim.IndexOf(" ");
            var rem = trim.Remove(0, (Cnt + 1));
            return rem;
        }
        #endregion

        #region DropLastWord
        /// <summary>
        /// Removes the set of characters beyond the last whitespace in the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns name="str">string</returns>
        /// <search>string,word,remove,drop,last</search>
        public static object DropLastWord(string str)
        {
            string trim = str.TrimEnd(" ".ToCharArray());
            int cnt = trim.LastIndexOf(" ");
            int len = trim.Length;
            var rem = trim.Remove(cnt, (len - cnt));
            return rem;
        }
        #endregion

        #region FindIndicesOfPhrase
        /// <summary>
        /// Returns the indices of the entire phrase the first time it is listed
        /// </summary>
        /// <param name="str"></param>
        /// <param name="searchFor"></param>
        /// <param name="ignoreCase"></param>
        /// <returns name="str">string</returns>
        /// <search>string,find,index,phrase,word,indices</search>
        public static object FindIndicesOfPhrase(string str, string searchFor, bool ignoreCase = false)
        {
            int Cnt = DSCore.String.IndexOf(str, searchFor, ignoreCase);
            int len = DSCore.String.Length(searchFor);
            List<int> ran = Enumerable.Range(Cnt, len).ToList();
            return ran;
        }
        #endregion

        #region FindFirstIndexOfPhrase
        /// <summary>
        /// Returns the indices of the entire phrase
        /// </summary>
        /// <param name="str"></param>
        /// <param name="searchFor"></param>
        /// <param name="ignoreCase"></param>
        /// <returns name="str">string</returns>
        /// <search>string,find,index,phrase,word,indices</search>
        [MultiReturn(new[] { "ind", "len" })]
        public static Dictionary<string, object> FindFirstIndexOfPhrase(string str, string searchFor, bool ignoreCase = false)
        {
            int Cnt = DSCore.String.IndexOf(str, searchFor, ignoreCase);
            int len = DSCore.String.Length(searchFor);

            Dictionary<string, object> newOutput;
            newOutput = new Dictionary<string, object>
                {
                    {"ind",Cnt},
                    {"len",len}
                };
            return newOutput;
        }
        #endregion

        #region FromObject
        /// <summary>
        /// Converts the object to a string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns name="str">string</returns>
        /// <search>string,word,convert,var,obj,change</search>
        public static object FromObject(object obj)
        {
            string output = Convert.ToString(obj);
            return output;
        }
        #endregion

        #region Number
        /// <summary>
        /// Gives the numbers 0-9 as a string and list.
        /// </summary>
        /// <param name></param>
        /// <returns name="str0-9">numbers 0-9 as string</returns>
        /// <returns name="list0-9">numbers 0-9 as list</returns>
        /// <search>string,numbers,list,integer,0-9</search>
        [MultiReturn(new[] { "str0-9", "list0-9" })]
        public static IDictionary<string, object> Numbers()
        {
            string strNum = "0123456789";

            List<string> strHolderNum = new List<string>();
            foreach (char ch in strNum)
            {
                strHolderNum.Add(ch.ToString());
            }

            return new Dictionary<string, object>()
            {
                {"str0-9",strNum},
                {"list0-9",strHolderNum},
            };
        }
        #endregion

        #region RemoveAfterCharacters
        /// <summary>
        /// Removes the part of the string after to the characters
        /// </summary>
        /// <param name="str"></param>
        /// <param name="characters"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="includeChar"></param>
        /// <returns name="str">string</returns>
        /// <search>string,remove,characters,after,end,char</search>
        public static string RemoveAfterCharacters(string str, string characters, bool ignoreCase = false, bool includeChar = false)
        {
            int x = DSCore.String.IndexOf(str, characters, ignoreCase);
            if (x < 0) return "error - characters could not be found in the string";
            int startInd = x + characters.Length;
            string rem = DSCore.String.Remove(str, startInd, (str.Length - startInd));
            if (includeChar)
            {
                int ind = DSCore.String.IndexOf(rem, characters, ignoreCase);
                string remLen = DSCore.String.Remove(rem, ind, characters.Length);
                return remLen;
            }
            else
                return rem;
        }
        #endregion

        #region RemoveAllWhitespace
        /// <summary>
        /// Removes all the whitespace from the input string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns name="str">string</returns>
        /// <search>string,remove,whitespace,space,input</search>
        public static object RemoveAllWhitespace(string str)
        {
            string trim = str.Replace(" ", "");
            return trim;
        }
        #endregion

        #region RemoveBeforeCharacters
        /// <summary>
        /// Removes the the part of the string prior to the characters
        /// </summary>
        /// <param name="str"></param>
        /// <param name="characters"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="includeChar"></param>
        /// <returns name="str">string</returns>
        /// <search>string,remove,characters,before,start,char</search>
        public static string RemoveBeforeCharacters(string str, string characters, bool ignoreCase = false, bool includeChar = false)
        {
            int x = DSCore.String.IndexOf(str, characters, ignoreCase);
            if (x < 0) return "error - characters could not be found in the string";
            string rem = DSCore.String.Remove(str, 0, x);
            if (includeChar)
            {
                string remLen = DSCore.String.Remove(rem, 0, characters.Length);
                return remLen;
            }
            else
                return rem;
        }
        #endregion

        #region RemoveCharacterAtIndex
        /// <summary>
        /// Removes the character at the given index
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <returns name="str">string</returns>
        /// <search>string,remove,index,character</search>
        public static object RemoveCharacterAtIndex(string str, int index)
        {
            string item = DSCore.String.Remove(str, index, 1);
            return item;
        }
        #endregion

        #region RemoveCharacterAtMultipleIndices
        /// <summary>
        /// Deletes multiple indices of a given string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <returns name="str">string</returns>
        /// <search>string,remove,index,multiple,character</search>
        public static object RemoveCharacterAtMultipleIndices(string str, int[] index)
        {
            string inputString = str;
            List<int> deleteIndices = new List<int>(index);
            try
            {
                for (int x = 0; x < deleteIndices.Count; x++)
                {
                    inputString = inputString.Remove(deleteIndices[x]-x, 1);
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return inputString;
        }
        #endregion

        #region RemoveCharactersBetweenInputs
        /// <summary>
        /// Removes the characters between the given inputs
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="input1">string</param>
        /// <param name="input2">string</param>
        /// <param name="ignoreCase">bool</param>
        /// <returns name="string">string</returns>
        /// <search>string,find,between,inputs,remove,character</search>
        public static string RemoveCharactersBetweenInputs(string str, string input1, string input2, bool ignoreCase = false)
        {
            int findInd1 = DSCore.String.IndexOf(str, input1, ignoreCase);
            int len = input1.Length;
            int startIndex = findInd1 + len;
            int findInd2 = DSCore.String.IndexOf(str, input2, ignoreCase);
            int count = findInd2 - startIndex;

            if (findInd1 == -1)
            {
                return "Cannot find input1 in string";
            }
            else if (findInd2 == -1)
            {
                return "Cannot find input2 in string";
            }
            else
            {
                string removeString = str.Remove(startIndex, count);
                return removeString;
            }
        }
        #endregion

        /*#region RemoveWordsOfMinimumLength
        /// <summary>
        /// Removes any words which length are lower than the minimum
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns name="str">string</returns>
        /// <search>string,list,Add,list,remove,string</search>
        public static string[] RemoveWordsOfMinimumLength(List<string> str, int length)
        {
            //Initialise
            List<string> inputList = new List<string>(str);
            List<string> deleteWordsList = new List<string>();
            int minWordLength = length;

            try
            {
                for (int x = 0; x < inputList.Count; x++)
                {
                    if (inputList[x].Length < minWordLength) //If the word in the input list is less than the minimum
                    {
                        deleteWordsList.Add(inputList[x]); //Add it to the delete words list
                    }
                }
                for (int x = 0; x < deleteWordsList.Count; x++)
                {
                    inputList.Remove(deleteWordsList[x]); // This removes any strings which are found from the delete list
                }
            }
            catch (Exception ex)
            {
                inputList[0] = ex.ToString();
                return inputList.ToArray();
            }
            return inputList.ToArray();
        }
        #endregion*/

        #region ReturnCharacterAtIndex
        /// <summary>
        /// Returns the character at the given index
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <returns name="str">string</returns>
        /// <search>string,find,index,return,character</search>
        public static string ReturnCharacterAtIndex(string str, int index)
        {
            string myChar = str[index].ToString();
            return myChar;
        }
        #endregion

        #region ReturnCharactersAtIndices
        /// <summary>
        /// Returns the characters at the given indices
        /// </summary>
        /// <param name="str"></param>
        /// <param name="indices"></param>
        /// <returns name="str">string</returns>
        /// <search>string,find,index,return,character</search>
        public static string ReturnCharactersAtIndices(string str, int indices)
        {
            string myChar = str[indices].ToString();
            string concat = System.String.Concat(myChar);
            return concat;
        }
        #endregion

        #region ReturnCharactersBetweenInputs
        /// <summary>
        /// Returns the characters between the given inputs
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="input1">string</param>
        /// <param name="input2">string</param>
        /// <param name="ignoreCase">bool</param>
        /// <returns name="string">string</returns>
        /// <search>string,find,between,inputs,return,character</search>
        public static string ReturnCharactersBetweenInputs(string str, string input1, string input2, bool ignoreCase = false)
        {
            int findInd1 = DSCore.String.IndexOf(str, input1, ignoreCase);
            int len = input1.Length;
            int startIndex = findInd1 + len;
            int findInd2 = DSCore.String.IndexOf(str, input2, ignoreCase);
            int count = findInd2 - startIndex;

            if (findInd1 == -1)
            {
                return "Cannot find input1 in string";
            }
            else if (findInd2 == -1)
            {
                return "Cannot find input2 in string";
            }
            else
            {
                string removeString = str.Substring(startIndex, count);
                return removeString;
            }
        }
        #endregion

        #region ToCamelCase
        /// <summary>
        /// Converts the given string to camel case
        /// </summary>
        /// <param name="str"></param>
        /// <returns name="str">string</returns>
        /// <search>string,convert,camel,case,to</search>
        public static object ToCamelCase(string str)
        {
            string lowerStr = str.ToLower();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            lowerStr = textInfo.ToTitleCase(lowerStr);
            string str1 = RemoveAllWhitespace(lowerStr).ToString();
            return str1;
        }
        #endregion

        #region ToList
        /// <summary>
        /// Converts the given string to a list
        /// </summary>
        /// <param name="str"></param>
        /// <returns name="list">string</returns>
        /// <search>string,convert,list,to</search>
        public static object ToList(string str)
        {
            string trim = DSCore.String.TrimTrailingWhitespace(str);
            var split = DSCore.String.Split(str, "");
            return split;
        }
        #endregion

        #region ToTitleCase
        /// <summary>
        /// Converts the given string to title case
        /// </summary>
        /// <param name="str"></param>
        /// <returns name="str">string</returns>
        /// <search>string,convert,title,case,to</search>
        public static object ToTitleCase(string str)
        {
            string lowerStr = str.ToLower();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            lowerStr = textInfo.ToTitleCase(lowerStr);
            return lowerStr;
        }
        #endregion

    }

    public class Validation
    {
        internal Validation()
        {

        }

        #region isBoolean
        /// <summary>
        /// Returns a boolean to say whether the item is a boolean type or not. 
        /// </summary>
        /// <param name="item">var</param>
        /// <returns name="bool">output</returns>
        /// <search>
        /// validation, core, is, true, false, bool, boolean
        /// </search>
        public static object isBoolean(object item)
        {
            if (item.GetType().ToString() == "System.Boolean")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region isDateTime
        /// <summary>
        /// Returns a boolean to say whether the item is a datetime type or not. 
        /// </summary>
        /// <param name="item">var</param>
        /// <returns name="bool">output</returns>
        /// <search>
        /// validation, core, is, true, false, bool, datetime, date, time
        /// </search>
        public static object isDateTime(object item)
        {
            if (item.GetType().ToString() == "System.DateTime")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region isDouble
        /// <summary>
        /// Returns a boolean to say whether the item is a double type or not. 
        /// </summary>
        /// <param name="item">var</param>
        /// <returns name="bool">output</returns>
        /// <search>
        /// validation, core, is, true, false, bool, double
        /// </search>
        public static object isDouble(object item)
        {
            if (item.GetType().ToString() == "System.Double")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region isInteger
        /// <summary>
        /// Returns a boolean to say whether the item is an integer type or not. 
        /// </summary>
        /// <param name="item">var</param>
        /// <returns name="bool">output</returns>
        /// <search>
        /// validation, core, is, integer, true, false, bool, int32
        /// </search>
        public static object isInteger(object item)
        {
            if (item.GetType().ToString() == "System.Int32")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region isString
        /// <summary>
        /// Returns a boolean to say whether the item is a string type or not. 
        /// </summary>
        /// <param name="item">var</param>
        /// <returns name="bool">output</returns>
        /// <search>
        /// validation, core, is, string, true, false, bool
        /// </search>
        public static object isString(object item)
        {
            if (item.GetType().ToString() == "System.String")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

    }
}
