﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PclLocalizer.Console
{
    class ParameterExtractor
    {
        private readonly IList<string> _arguments;

        public ParameterExtractor(IEnumerable<string> arguments )
        {
            this._arguments = arguments.ToList();
        }


        //public string InputFile => this.GetValue(Constants.InputParam);
        public List<string> InputFile => this.GetValueList(Constants.InputParam); 

        public string DestinationFile => this.GetValue(Constants.DestinationParam);
        public string Separator => this.GetValue(Constants.SeparatorParam);
        public string NameSpace => this.GetValue(Constants.NamespaceParam);
        public string ClassName => this.GetValue(Constants.ClassNameParam);

        private string GetValue(string param)
        {
            var index = this._arguments.IndexOf(param);
            return this._arguments[index + 1];
        }

        private List<string> GetValueList(string param)
        {
            var index = this._arguments.IndexOf((param));
            return this._arguments[index + 1].Split(' ').ToList();
        }
        
    }
}
