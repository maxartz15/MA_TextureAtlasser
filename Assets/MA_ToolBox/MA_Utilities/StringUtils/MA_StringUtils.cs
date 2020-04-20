//-

using UnityEditor;
using UnityEngine;

namespace MA_Toolbox.Utils
{
    public static class MA_StringUtils
    {
        private const string ALPHABET = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ";

        public static string RandomAlphabetString(int length)
        {
            string s = "";
            for (int i = 0; i < length; i++)
            {
                s += RandomAlphabetChar();
            }

            return s;
        }

        public static char RandomAlphabetChar()
        {
            return ALPHABET[Random.Range(0, ALPHABET.Length)];
        }
    }
}