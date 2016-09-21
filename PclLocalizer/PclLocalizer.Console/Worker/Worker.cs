using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PclLocalizer.Console.Properties;

namespace PclLocalizer.Console.Worker
{
    internal class Worker
    {
        private readonly IEnumerable<string> _args;
        private readonly Checker _checker;
        private ParameterExtractor _extractor;

        public Worker(string[] args)
        {
            this._args = args;
            this._checker = new Checker(args);
            this._extractor = new ParameterExtractor(args);
        }

        public void Run()
        {
            //Help mode
            if (this._checker.IsHelpRequest)
            {
                this.RunHelp();
                return;
            }

            //Run check
            this._checker.CheckArgs();

            this.RunLocalizer();
        }

        private void RunLocalizer()
        {
            var input = this._extractor.InputFile;
            var destination = this._extractor.DestinationFile;
            var className = this._extractor.ClassName;
            var separator = this._extractor.Separator;
            var nameSpace = this._extractor.NameSpace;

            var magic = Resources.MagicFile;// File.ReadAllText("File/MagicFile.txt");

            //Add destination name
            magic = magic.Replace(Constants.ClassNamePlaceHolder, className);
            //Add namespace
            magic = magic.Replace(Constants.NamespacePlaceHolder, nameSpace);

            var resourceContainerList = input.Select(item => new ResourceContainer(item, separator)).ToList();

            //check files
            if (!this._checker.CheckListInputFile(resourceContainerList)) return;

            //create dictionary
            var dictionarySection = new StringBuilder();
            var dictionaryCounter = 0;
            foreach (var resource in resourceContainerList)
            {
                dictionaryCounter++;
                var varname = $"d{dictionaryCounter}";
                System.Console.WriteLine($"Language {resource.Culture} found.");
                dictionarySection.Append($"\t\t\tvar {varname} = new Dictionary<string, string> {{");

                foreach (var res in resource.Resource)
                {
                    var key = Regex.Replace(res.Key, @"\s+", ""); //trim
                    dictionarySection.Append($"{{\"{key}\",\"{res.Value}\"}},");
                }

                dictionarySection.Remove(dictionarySection.Length - 1, 1);
                dictionarySection.Append($"}};{Environment.NewLine}");
                dictionarySection.Append($"\t\t\tValues.Add(\"{resource.Culture}\", {varname});{Environment.NewLine}");
            }
            magic = magic.Replace(Constants.DictionariesPlaceHolder, dictionarySection.ToString());
            //end

            var propertiesSection = new StringBuilder();
            var defaultCulture = resourceContainerList.First();

            foreach (var res in defaultCulture.Resource)
            {
                var key = Regex.Replace(res.Key, @"\s+", ""); //trim
                propertiesSection.AppendLine($"\t\tpublic static string {key} => GetValue(\"{key}\");");
            }

            magic = magic.Replace(Constants.PropertiesPlaceHolder, propertiesSection.ToString());

            File.WriteAllText(destination, magic);

            System.Console.WriteLine("All done!");

        }

        private void RunHelp()
        {
            System.Console.WriteLine("Welcome to PclLocalizer!");
            System.Console.WriteLine("*** A big thanks to Mark Jack Milian for having created me! ***");
            System.Console.WriteLine("I need those args:");
            System.Console.WriteLine("-f INPUTFILE => the input file with a separator");
            System.Console.WriteLine("-s SEPARATOR => the separator for input file");
            System.Console.WriteLine("-d DESTINATIONFILE => the destination file");
            System.Console.WriteLine("-c CLASSNAME => the destination classname file");
            System.Console.WriteLine("-n NAMESPACE => the namespace for generated class");
            System.Console.WriteLine("The first line of input file must be:");
            System.Console.WriteLine("Column[0]: key");
            System.Console.WriteLine("Column[X]: languageCode");
            System.Console.WriteLine("");
            System.Console.WriteLine("Have Fun!");
        }
    }
}
