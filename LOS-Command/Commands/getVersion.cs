// 2017, 1, 22
// LOS-Command:getVersion.cs
// Copyright (c) 2017 PopulationX

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LOS_Command.Models;

namespace LOS_Command.Commands
{
    public class GetVersion
    {


        public static VersionInfo[] Version(string filename = "")
        {
            if (filename == String.Empty)
            {
                VersionInfo[] versions = new VersionInfo[1];
                VersionInfo vi = new VersionInfo();
                vi.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                vi.Filename = Assembly.GetExecutingAssembly().FullName;
                versions[0] = vi;
                return versions;
            }

            return AssemblyVersion(filename);
        }


        private static VersionInfo[] AssemblyVersion(string filename)
        {
            // Search the LOS Directory for the file(s) with that filename and return appropriate version information.
            string[] files = Directory.GetFiles("C:\\LOS\\", filename + ".*", SearchOption.AllDirectories);

            VersionInfo[] versions = new VersionInfo[1];

            if (files.Length == 0)
            {
                VersionInfo vi = new VersionInfo
                {
                    Filename = filename,
                    VersionNumber = "N/A"
                };
                versions[0] = vi;
                return versions;
            }


            versions = new VersionInfo[files.Length];
            int idx = 0;
            foreach (string file in files)
            {
                var ErrorMessage = "";

                VersionInfo vi = new VersionInfo();

                try
                {
                    string assemblyVersion = Assembly.LoadFile(file).GetName().Version.ToString();
                    vi.Filename = file;
                    vi.VersionNumber = assemblyVersion;
                }
                
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    if (ex.Message.Contains("The module was expected to contain an assembly manifest."))
                        ErrorMessage = "Not Applicable";
                    vi.Filename = file;
                    vi.VersionNumber = ErrorMessage;
                }
                finally
                {
                    versions[idx] = vi;

                    idx++;
                }


            }

            return versions;
        }
    }


}
