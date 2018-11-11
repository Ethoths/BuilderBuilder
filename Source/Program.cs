using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuilderBuilder
{   
    public class Program
    {
        private readonly List<string> _nameSpaces = new List<string>();

        public void Run(string assembly)
        {
            var asm = Assembly.LoadFile(assembly);
            var classes = asm.GetTypes();

            foreach (var classe in classes)
            {
                var boilerPlate = CreateBoilerPlate(classe);                                                
                var propertyInfos = classe.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var fields = new List<string>();
                var properties = new List<string>();
                var initialisers = new List<string>();
                var constructors = new List<string>();

                foreach (var propertyInfo in propertyInfos)
                {
                    var field = string.Empty;
                    AddNameSpace(propertyInfo.PropertyType.ToString());
                    var fieldName = GetTypeName(propertyInfo.PropertyType.ToString());

                    // Is it a simple type or does it need a builder?
                    


                    if (fieldName.EndsWith("[]"))
                    {
                        fieldName = fieldName.Substring(0, fieldName.Length - 2);
                        field = string.Format("private List<{0}Builder> _{1}Builders = new List<{0}Builder>();", DeAlias(fieldName), DeCapitalise(propertyInfo.Name));
                        initialisers.Add(propertyInfo.Name);
                        fields.Add(field);
                        constructors.Add(string.Format("{1}Builder.Add(new {0}Builder())", DeAlias(fieldName), DeCapitalise(propertyInfo.Name)));
                    }
                    else
                    {
                        field = $"private {DeAlias(fieldName)} _{DeCapitalise(propertyInfo.Name)} = Randomiser.{fieldName}();";
                        initialisers.Add(propertyInfo.Name);
                        fields.Add(field);
                    }

                    Console.WriteLine(field);
                    var property = string.Format("public {0}Builder {1}({2} {3}){4}{5}{{{4}{5}{5}_{3} = {3};{4}{4}{5}{5}return this;{4}{5}}}", classe.Name, propertyInfo.Name, DeAlias(GetTypeName(propertyInfo.PropertyType.ToString())), DeCapitalise(propertyInfo.Name), Environment.NewLine, "\t");
                    Console.WriteLine(property);
                    properties.Add(property);
                }

                string g = null;

                foreach (var initialiser in initialisers)
                {
                    g += initialiser + " = _" + DeCapitalise(initialiser) + ",";
                }

                g = g.Substring(0, g.Length - 1);

                boilerPlate = boilerPlate.Replace("[FIELDS]", string.Join(Environment.NewLine, fields));
                boilerPlate = boilerPlate.Replace("[PROPERTIES]", string.Join(Environment.NewLine, properties));
                boilerPlate = boilerPlate.Replace("[INITIALISERS]", g);
                boilerPlate = boilerPlate.Replace("[CONSTRUCTOR]", string.Join(Environment.NewLine, constructors));

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine(boilerPlate.ToString());
            }
        }

        private StringBuilder CreateBoilerPlate(Type classe)
        {            
            var boilerPlate = new StringBuilder(System.IO.File.ReadAllText(@"C:\Projects\BuilderBuilder\BuilderBuilder\Source\Template.txt"));
            boilerPlate.Replace("[CLASSNAME]", classe.Name);

            return boilerPlate;
        }

        private void AddNameSpace(string fullName)
        {
            var nameSpace = fullName.Substring(0, fullName.LastIndexOf("."));

            if (!_nameSpaces.Contains(nameSpace))
            {
                _nameSpaces.Add(nameSpace);
            }
        }

        private string GetTypeName(string fullName)
        {
            var nameSpace = fullName.Substring(0, fullName.LastIndexOf("."));

            if (!_nameSpaces.Contains(nameSpace))
            {
                _nameSpaces.Add(nameSpace);
            }

            return fullName.Substring(nameSpace.Length + 1);
        }

        private string DeCapitalise(string text)
        {
            var letters = text.ToArray();

            if (letters[0].ToString().ToLower() != letters[0].ToString())
            {
                letters[0] = letters[0].ToString().ToLower().ToCharArray()[0];            
            }

            return new string(letters);
        }

        private string DeAlias(string text)
        {
            if (text == "String")
            {
                text = DeCapitalise(text);
            }

            return text;
        }

        private string DeCapitaliseMany(string text)
        {
            var letters = text.ToArray();

            for (var i = 0; i<letters.Length; i++)
            {
                if (letters[i].ToString().ToLower() != letters[i].ToString())
                {
                    letters[i] = letters[i].ToString().ToLower().ToCharArray()[0];
                }
                else
                {
                    break;
                }
            }

            return new string(letters);
        }
    }
}
