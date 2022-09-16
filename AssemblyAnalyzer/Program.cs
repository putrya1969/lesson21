using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssemblyAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path to file for analyze");
            var filePath = Console.ReadLine();
            if (!IsValidFilename(filePath))
            {
                Console.WriteLine("Entered invalid file path");
                return;
            }
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File is not exist!");
                return;
            }
            //string selectedFile = string.Empty;
            //using (OpenFileDialog ofd = new OpenFileDialog())
            //{
            //    ofd.Title = "Select dll or exe file for analysis";
            //    ofd.Filter = "dll or exe files (*.dll)|*.dll|exe files (*.exe)|*.exe";
            //    if (ofd.ShowDialog() == DialogResult.OK)
            //    {
            //        selectedFile = ofd.FileName;
            //    }
            //}
            var assembly = Assembly.LoadFile(filePath);
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //Assembly assembly = null;
            //StringBuilder sb = new StringBuilder();
            //foreach (var item in assemblies)
            //{
            //    if (item.GetName().Name.Equals("Delegates"))
            //        assembly = Assembly.Load(item.GetName().Name);
            //}
            if (assembly == null)
            {
                Console.WriteLine("assembly 'Delegates' not find");
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var type in assembly.GetTypes().OrderBy(t => t.Name))
            {
                sb.AppendLine();
                var methodsInfo = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_") && !m.Name.StartsWith("add_") && !m.Name.StartsWith("remove_"))
                    .OrderBy(m => m.Name)
                    .ToArray();
                sb.AppendLine(methodsInfo.Length == 0 ?
                    $"Class name {type.Name} has no methods" :
                    $"Class '{type.Name}' has is following methods:");
                foreach (var methodInfo in methodsInfo)
                {
                    //sb.AppendLine(new string('-', 50));
                    if (methodInfo.IsConstructor) sb.Append(" ctor");
                    if (methodInfo.IsAbstract) sb.Append(" abstract");
                    if (methodInfo.IsPublic) sb.Append(" public");
                    if (methodInfo.IsPrivate) sb.Append(" private");
                    if (methodInfo.IsFamily) sb.Append(" protected");
                    if (methodInfo.IsAssembly) sb.Append(" internal");
                    sb.Append(" " + methodInfo.ReturnType.ToString());
                    sb.Append(" " + methodInfo.Name);
                    sb.Append(" (");
                    foreach (var parameter in methodInfo.GetParameters())
                    {
                        sb.Append(parameter.ToString());
                    }
                    sb.Append(")\n");
                }
            }
            Console.WriteLine(sb);
            Console.ReadKey();
        }
        static bool IsValidFilename(string testName)
        {
            Regex containsABadCharacter = new Regex("["
                  + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");
            if (containsABadCharacter.IsMatch(testName)) { return false; };

            // other checks for UNC, drive-path format, etc

            return true;
        }
    }
}
