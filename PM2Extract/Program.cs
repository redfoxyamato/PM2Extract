using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using LB4Extract;

namespace PM2Extract
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                string exe = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
                write(string.Format("Usage: {0} <input file> [option]",exe));
                write(string.Format("[Option]"));
                write(string.Format("/s ... turn off debug message"));
                write(string.Format("/o<output path> ... sets destination-directory."));
                write(string.Format("/f<name filter> ... filtering name by given filter."));
                waitKeyInput();
                return;
            }
            string file_name = Path.GetFullPath(args[0]);
            string dest = "";
            string filter = "*.*";
            if(!File.Exists(file_name))
            {
                write("No such a file:"+file_name);
                waitKeyInput();
                return;
            }
            LB4Extractor ext = new LB4Extractor(file_name);
            LB4ArchiveInfo info = ext.getArchiveInfo();
            string[] options = getArgumentsFrom(args, 1);
            if(isThereOption(options,"s"))
            {
                info.DisableDebug();
            }
            if(isThereOption(options,"o",false))
            {
                dest = getOptionalString(options, "o");
                if(!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }
            }
            if(isThereOption(options,"f",false))
            {
                filter = getOptionalString(options, "f");
            }
            int counter = 0;
            foreach(LB4Entry entry in info.GetMatchedEntries(filter))
            {
                string path = MakeDirStr(dest) + entry.Name;
                File.WriteAllBytes(path, entry.Buffer);
                counter++;
            }
            write(string.Format("Extraction has done! file count:{0}",counter));
            waitKeyInput();
        }
        static void write(string str)
        {
            Console.WriteLine(str);
        }
        [Conditional("DEBUG")]
        static void waitKeyInput()
        {
            write("");
            write("Press any key to continue...");
            Console.ReadKey();
        }
        static string[] getArgumentsFrom(string[] strs,int start)
        {
            List<string> list = new List<string>();
            for(int i = start;i < strs.Length;i++)
            {
                list.Add(strs[i]);
            }
            return list.ToArray();
        }
        static bool isThereOption(string[] option,string name,bool strict = true)
        {
            foreach(string str in option)
            {
                if(strict)
                {
                    if(str == "/"+name)
                    {
                        return true;
                    }
                }
                else
                {
                    if(str.Contains("/"+name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        static string getOptionalString(string[] options,string name)
        {
            if(!isThereOption(options,name,false))
            {
                return "";
            }
            string ret = "";
            foreach(string opt in options)
            {
                if(opt.Contains("/"+name))
                {
                    ret = opt.Replace("/" + name, "");
                    break;
                }
            }
            return ret;
        }

        static string MakeDirStr(string path)
        {
            return path.Trim() == "" ? MakeDirStr(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])) : (path.EndsWith(@"\") ? path : path + "\\");
        }
    }
}
