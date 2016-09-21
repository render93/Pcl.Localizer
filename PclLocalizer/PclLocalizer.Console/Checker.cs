using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using PclLocalizer.Console.Exceptions;

namespace PclLocalizer.Console
{
    class Checker
    {
        private readonly IList<string> _arguments;

        public Checker(IEnumerable<string> arguments)
        {
            this._arguments = arguments.ToList();
        }


        public void CheckArgs()
        {
            if (this.NoArguments)
                throw new ParameterMissingException("I need some parameter! Try -h");

            if (!this.InputFileArgumentExist)
                throw new FileNotFoundException("Input file not exist! -f");

            if (!this.DestFileArgumentsExist)
                throw new ParameterMissingException("I need destination file parameter! -d");

            if (!this.SeparatorArgumentsExist)
                throw new ParameterMissingException("I need separator for input file! -s");

            if (!this.NamespaceArgumentsExist)
                throw new ParameterMissingException("I need namespace for input file! -n");

            if (!this.ClassArgumentsExist)
                throw new ParameterMissingException("I need a destination classname! -c");
        }

        /// <summary>
        /// Is help request
        /// </summary>
        public bool IsHelpRequest => this._arguments.Contains("-h");

        /// <summary>
        /// There are no arguments
        /// </summary>
        public bool NoArguments => !this._arguments.Any();

        /// <summary>
        /// Check if input file exist
        /// </summary>
        public bool InputFileArgumentExist
        {
            get
            {
                if (!this._arguments.Contains(Constants.InputParam)) return false;

                var index = this._arguments.IndexOf(Constants.InputParam);
                if (index >= this._arguments.Count - 1) return false;

                var listFile = this._arguments[index + 1].Split(' ');
                return listFile.All(File.Exists);
            }
        }

        /// <summary>
        /// Exist arguments -d and is passed
        /// </summary>
        public bool DestFileArgumentsExist
        {
            get
            {
                var paramexist = this._arguments.Contains(Constants.DestinationParam);

                if (!paramexist) return false;

                var index = this._arguments.IndexOf(Constants.DestinationParam);
                if (index >= this._arguments.Count - 1) return false;

                return true;
            }
        }

        /// <summary>
        /// Exist arguments -s and is passed
        /// </summary>
        public bool SeparatorArgumentsExist
        {
            get
            {
                var paramexist = this._arguments.Contains(Constants.SeparatorParam);

                if (!paramexist) return false;

                var index = this._arguments.IndexOf(Constants.SeparatorParam);
                if (index >= this._arguments.Count - 1) return false;

                return true;
            }
        }

        /// <summary>
        /// Exist arguments -n and is passed
        /// </summary>
        public bool NamespaceArgumentsExist
        {
            get
            {
                var paramexist = this._arguments.Contains(Constants.NamespaceParam);

                if (!paramexist) return false;

                var index = this._arguments.IndexOf(Constants.NamespaceParam);
                if (index >= this._arguments.Count - 1) return false;

                return true;
            }
        }

        /// <summary>
        /// Exist arguments -c and is passed
        /// </summary>
        public bool ClassArgumentsExist
        {
            get
            {
                var paramexist = this._arguments.Contains(Constants.ClassNameParam);

                if (!paramexist) return false;

                var index = this._arguments.IndexOf(Constants.ClassNameParam);
                if (index >= this._arguments.Count - 1) return false;

                return true;
            }
        }

        public bool CheckListInputFile(List<ResourceContainer> fileList)
        {
            if (fileList.Count <= 1) return true;

            for (var i = 1; i < fileList.Count; i++)
            {
                //verifico che i due dictionary abbiano lo stesso numero di elementi
                if (fileList[0].Resource.Count != fileList[i].Resource.Count)
                {
                    System.Console.WriteLine("CSV files has different number of rows.");
                    return false;
                }
            }

            //verifico che ogni oggetto abbia la culture differente
            var allCulture = fileList.Select(x => x.Culture).ToList();
            if (allCulture.GroupBy(x => x).Any(g => g.Count() > 1))
            {
                System.Console.WriteLine("Two or more csv file contains the same culture.");
                return false;
            }

            //ordino le key di ogni dictionary in ordine crescente
            fileList.ForEach(
                f => f.Resource = f.Resource.OrderBy(o => o.Key).ToDictionary(pair => pair.Key, pair => pair.Value));

            //verifico che tutti i file contengano le stesse chiavi
            var defaultKeyList = fileList[0].Resource.Select(x => x.Key).ToList();
            for (var i = 1; i < fileList.Count; i++)
            {
                var keyList = fileList[i].Resource.Select(x => x.Key).ToList();
                if (!keyList.SequenceEqual(defaultKeyList))
                {
                    System.Console.WriteLine("CSV files not contains same key.");
                    return false;
                }
            }

            return true;
        }
    }
}
