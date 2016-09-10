﻿using System.Collections.Generic;
using System.Linq;
using PclLocalizer.ResMan;

namespace Test
{
    public static class TestRes
    {
        private static Dictionary<string,Dictionary<string,string>> values = new Dictionary<string, Dictionary<string, string>>();

        static TestRes()
        {
            var enDic = new Dictionary<string, string> {{"saluta", "hello!"}};
            var itDic = new Dictionary<string, string> {{"saluta", "ciao!"}};

            values.Add("en", enDic);
            values.Add("it", itDic);
        }

        public static string Value1 => GetValue("saluta");

        private static string GetValue(string key)
        {
            if (values.ContainsKey(PclResMan.Lang))
                return values[ResourceManager.Lang][key];
            else
            {
                return ResourceManager.Default == null ? values[values.First().Key][key] : values[ResourceManager.Default][key];
            }
        }
    }
}
