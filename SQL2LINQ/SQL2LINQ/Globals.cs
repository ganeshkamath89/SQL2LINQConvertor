using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQL2LINQ {
    internal class Globals {
        public static string EDMXworkingDirectory = @"C:\ERachana\EDMX\EDMXFiles\EDMXParts";
        public static bool isEDMXAlreadyGenerated = false;

        public static string Server = "",Database = "", UserName = "",Password = "";
        public static string ProjectName = "", UserDefinedObjectName = "_appdb", TemporaryDirectoryPath="";

        public static string GetSubstringBetweenStrings(string Full, string startMatch, string endMatch) {
            int pFrom = Full.IndexOf(startMatch) + startMatch.Length;
            int pTo = Full.LastIndexOf(endMatch);
            if (pTo > pFrom)
                return Full.Substring(pFrom, pTo - pFrom);
            else
                return "";
        }

        public static void GenerateORMFiles() {
            string workingDirectory = EDMXworkingDirectory;
            if (!isEDMXAlreadyGenerated) {
                // Show Progress Bar here
                try {
                    isEDMXAlreadyGenerated = true;
                    Directory.CreateDirectory(@"C:\ERachana");
                    Directory.CreateDirectory(@"C:\ERachana\EDMX");
                    Directory.CreateDirectory(@"C:\ERachana\EDMX\EDMXFiles");
                    Directory.CreateDirectory(workingDirectory);
                    
                    string CommandToCreateEDMXOnCommandLine = "\"%windir%\\Microsoft.NET\\Framework\\v4.0.30319\\edmgen.exe\" /mode:fullgeneration /c:\"data source = "
                                        + Server + "; initial catalog = "
                                        + Database + "; user id = "
                                        + UserName + "; password = "
                                        + Password + "; MultipleActiveResultSets = True; persist security info = True; App = EntityFramework\" /project:DataModel /entitycontainer:DBContext /namespace:Models /language:CSharp & exit";

                    string ResourcesDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Resources\";
                    string EDMXFileName = "DataModel.edmx";
                    string ContextFileName = "DataModel.Context.tt";
                    string TablesFileName = "DataModel.tt";

                    string EdmxLocation = workingDirectory + @"\" + EDMXFileName;
                    File.Copy(Path.Combine(ResourcesDirectory, EDMXFileName), EdmxLocation, true);
                    File.Copy(Path.Combine(ResourcesDirectory, ContextFileName), workingDirectory + @"\" + ContextFileName, true);
                    File.Copy(Path.Combine(ResourcesDirectory, TablesFileName), workingDirectory + @"\" + TablesFileName, true);
                    using (var process = new Process()) {
                        var startInfo = new ProcessStartInfo {
                            WorkingDirectory = workingDirectory,
                            WindowStyle = ProcessWindowStyle.Minimized,
                            CreateNoWindow = true,
                            RedirectStandardInput = true,
                            UseShellExecute = false,
                            FileName = "cmd.exe",
                            Verb = "runas"
                        };

                        process.StartInfo = startInfo;
                        process.Start();
                        process.StandardInput.WriteLine(CommandToCreateEDMXOnCommandLine);
                        process.WaitForExit();
                        process.Close();
                        process.Dispose();
                    }
                    string text = File.ReadAllText(EdmxLocation);

                    string c = "";
                    c = parseSCMDLFiles(workingDirectory + @"\DataModel.ssdl", "Schema");
                    text = text.Replace("###StorageModelsSchema", c);

                    c = parseSCMDLFiles(workingDirectory + @"\DataModel.csdl", "Schema");
                    text = text.Replace("###ConceptualModelsSchema", c);

                    c = parseSCMDLFiles(workingDirectory + @"\DataModel.msl", "Mapping");
                    text = text.Replace("###Mappings", c);

                    File.WriteAllText(EdmxLocation, text);

                    string[] fileToBeDeleted = Directory.GetFiles(workingDirectory);
                    foreach (string filePath in fileToBeDeleted) {
                        if (filePath.Contains("DataModel.ObjectLayer.cs") || filePath.Contains("DataModel.Views.cs")) {
                            File.Delete(filePath);
                        } else {
                            if (filePath.ToLower().Contains(".edmx") || filePath.ToLower().Contains(".tt") || filePath.ToLower().Contains(".cs"))
                                continue;
                            File.Delete(filePath);
                        }
                    }
                    string location = @"C:\ERachana\EDMX";
                    string TransformFileName = "transform_all.bat";
                    File.Copy(Path.Combine(ResourcesDirectory, TransformFileName), location + @"\" + TransformFileName, true);
                    string batFileCommand = "/C " + location + @"\" + TransformFileName;

                    using (var process = new Process()) {
                        var startInfo = new ProcessStartInfo() {
                            WorkingDirectory = location,
                            WindowStyle = ProcessWindowStyle.Minimized,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            FileName = @"cmd.exe",
                            Verb = "runas",
                            Arguments = batFileCommand
                        };

                        process.StartInfo = startInfo;
                        process.Start();
                        process.WaitForExit();
                        process.Close();
                        process.Dispose();
                    }
                } catch {
                    MessageBox.Show("Only Projects with MSSQL may be converted to Web Projects");
                } finally {
                    // Close Progressbar here
                }
            }
        }

        public static string parseSCMDLFiles(string EDMXDirectoryFile, string tag) {
            List<string> lines = File.ReadLines(EDMXDirectoryFile).ToList();
            string content = "";
            bool flagEnable = false;
            foreach (string line in lines) {
                if (line.Contains("</" + tag + ">"))
                    flagEnable = false;
                if (flagEnable == true)
                    content += line + Environment.NewLine;
                if (line.Contains("<" + tag))
                    flagEnable = true;
            }
            return content;
        }
    }
}
