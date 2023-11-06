using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altunbilekler.Service
{
    public class ErrorHelper
        {
            public void ErrorWriteFile(Exception error, string path, string fileName, string projectName)
            {

                try
                {
                    StreamWriter Dosya = File.CreateText(path + fileName);
                    Dosya.Close();

                    TextWriter txtwriter = new StreamWriter(path + fileName);
                    txtwriter.WriteLine(projectName);
                    txtwriter.WriteLine(error.Message);
                    txtwriter.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
