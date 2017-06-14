using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Fclp;
using static System.Environment;

namespace TryPasses
{
    class Program
    {
        class AppSettings
        {
            public string GethDir { get; set; }
            public string PasswordsFile { get; set; }
            public List<string> Passwords { get; set; }
            public string AccountId { get; set; }
            public bool SkipRussian { get; set; }
            public bool GoDeeper { get; set; }
        }

        static void Main(string[] args)
        {
            var parser = new FluentCommandLineParser<AppSettings>();

            parser.Setup(arg => arg.AccountId)
                .As('a', "account")
                .WithDescription("Id of account you are trying to bruteforce")
                .Required();

            parser.Setup(arg => arg.GethDir)
                .As('g', "gethdir")
                .WithDescription("Pass here your geth location if it different from default. By default it is searched in " + GetFolderPath(SpecialFolder.ApplicationData) + "\\Mist\\binaries\\Geth\\unpacked")
                .SetDefault(GetFolderPath(SpecialFolder.ApplicationData) + "\\Mist\\binaries\\Geth\\unpacked");

            parser.Setup(arg => arg.PasswordsFile)
                .As('f', "pfile")
                .WithDescription("Path to file where you have all possible password you could use in some form.")
                .SetDefault("");

            parser.Setup(arg => arg.SkipRussian)
                .As('s', "skiprussian")
                .WithDescription("Pass this if you don't have russian keyboard layout.")
                .SetDefault(false);

            parser.Setup(arg => arg.Passwords)
                .As('p', "passes")
                .WithDescription("You can pass here your passwords directly, but remember that it is less secure!")
                .SetDefault(new List<string>());

            parser.Setup(arg => arg.GoDeeper)
                .As('d', "deeper")
                .WithDescription("Add this option if you want to test combinations of 3 passwords")
                .SetDefault(false);
            string h = "Welcome to TryPasses. This software has been developed for unlocking ethereum accounts, whose passwords you can not remember.\n" +
                       "You pass here all passwords you could use either by file or directly to console, and program combines them in pairs. \n" +
                       "It also uses CAPSLOCKED versions of passwords and for russian users also uses incorrect(Russian) layout version.\n" +
                       "It is working with geth used by Mist, so you should have Mist installed. \n" +
                       "This software is free to use, but any donations are welcome: 0xBfD86078A4581c92302E87Cd6687B09CB1663A85 (ETH) 1EVRYSRL9CARRBv6NZhSPiLv29eua33iSN (BTC)";
            parser.SetupHelp("?", "help")
                .WithHeader(h)
                .UseForEmptyArgs()
                .Callback(text => Console.WriteLine(text));
           
            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                Console.WriteLine(result.ErrorText);
                parser.HelpOption.ShowHelp(parser.Options);
                return;
            }

            parser.Object.GethDir = parser.Object.GethDir.TrimEnd('\\');
            
            if (!File.Exists(parser.Object.GethDir + "\\geth.exe"))
            {
                Console.WriteLine("Geth not found on given location: " + parser.Object.GethDir + "\nPlease enter correct geth location.");
                return;
            }
            if (parser.Object.PasswordsFile.Length > 0 && !File.Exists(parser.Object.PasswordsFile))
            {
                Console.WriteLine("Password file not found on given location: " + parser.Object.PasswordsFile + "\nPlease enter correct passwords file location.");
                return;
            }
            if (parser.Object.PasswordsFile.Length == 0 && parser.Object.Passwords.Count == 0)
            {
                Console.WriteLine("Neither passwords file nor passwords were entered. Please enter at least one passwords source!");
                return;
            }

            var passes_base = LoadPasswords(parser.Object.PasswordsFile, parser.Object);
            var passes = new List<string>(passes_base);
            foreach (var pass in passes_base)
            {
                passes.Add(pass.toCAPS()); // add CAPSLOCK VERSION
                if (!parser.Object.SkipRussian)
                {
                    passes.Add(pass.toRus()); // add russian version
                    passes.Add(pass.toRus().toCAPS()); // add RUSSIAN CAPSLOCK VERSION
                }
            }
            HashSet<string> combinedPasses = new HashSet<string>(); // use set so it will delete duplicates automatically
            passes.ForEach(p => combinedPasses.Add(p));
            foreach (var pass1 in passes)
            {
                foreach (var pass2 in passes)
                {
                    if (parser.Object.GoDeeper)
                    {
                        foreach (var pass3 in passes)
                        {
                            combinedPasses.Add(pass1 + pass2 + pass3);
                        }
                    }
                    else
                    {
                        combinedPasses.Add(pass1 + pass2);
                    }
                }
            }
            int i = 0, total = combinedPasses.Count;
            string password = null;

            foreach (var combinedPass in combinedPasses)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Trying \"" + combinedPass + "\"...({0} of {1}, {2}%)", ++i, total, (double) i / total * 100.0);
                Console.ResetColor();
                if (TryPassword(combinedPass, parser.Object))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success! Password is: " + combinedPass);
                    Console.ResetColor();
                    password = combinedPass;
                    break;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Password \"" + combinedPass + "\" doesn't match.");
                Console.ResetColor();
            }
            if (password == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Password not found.");
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        static List<string> LoadPasswords(string file, AppSettings settings)
        {
            var lines = file.Length == 0 ? new string[] {} : File.ReadAllLines(file);
            if (lines.Any(s => s.StartsWith(" ") || s.StartsWith("\t") || s.EndsWith(" ") || s.EndsWith("\t")))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: some of your passwords have spaces in beginning or ending, that can be mistake!");
                Console.ResetColor();
            }
            var res = lines.Where(s => s.Trim().Length > 0).Concat(settings.Passwords); // add passwords from commandline here
            Console.WriteLine("Imported {0} passwords.", res.Count());
            return res.ToList();
        }

        static bool TryPassword(string password, AppSettings settings)
        {
            Process cmd = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    WorkingDirectory = settings.GethDir,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };
            cmd.Start();
            var inp = cmd.StandardInput;
            var outp = cmd.StandardOutput;
            inp.AutoFlush = true;

            inp.WriteLine("geth account update " + settings.AccountId);
            string line = "";
            while (!line.StartsWith("Unlocking account "))
            {
                line = outp.ReadLine();
            }

            inp.WriteLine(password);
            outp.ReadLine(); // warning like ""!! Unsupported terminal, password will be echoed.""
            outp.ReadLine(); // Invintation like "Passphrase:"
            line = outp.ReadLine();
            cmd.Close();
            return line.Contains("Please give a new password. Do not forget this password.");
        }
    }

    public static class StringExt
    {
        private static Dictionary<char, char> mapping = initMapping();
        
        static Dictionary<char, char> initMapping()
        {
            var eng = "F,DULT`;PBQRKVYJGHCNEA[WXIO]SM'.Zf,dult`;pbqrkvyjghcnea[wxio]sm'.z0123456789";
            var rus = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789";
            if (eng.Length != rus.Length)
            {
                throw new Exception("Некорректный маппинг!(не совпадает длина)");
            }
            var result = new Dictionary<char, char>();
            for (int i = 0; i < eng.Length; i++)
            {
                result[eng[i]] = rus[i];
            }
            return result;
        }

        public static string toRus(this string s)
        {
            StringBuilder strRussian = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                strRussian.Append(mapping.ContainsKey(s[i]) ? mapping[s[i]] : s[i]);
            }
            return strRussian.ToString();
        }

        public static string toCAPS(this string s)
        {
            StringBuilder strReverse = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= 'a' && s[i] <= 'z'
                    ||
                    s[i] >= 'а' && s[i] <= 'я')
                {
                    strReverse.Append((char)(Convert.ToInt32(s[i]) - 0x20));
                }
                else if (s[i] >= 'A' && s[i] <= 'Z'
                    ||
                    s[i] >= 'А' && s[i] <= 'Я')
                {
                    strReverse.Append((char)(Convert.ToInt32(s[i]) + 0x20));
                }
                else
                {
                    strReverse.Append(s[i]);
                }

            }
            return strReverse.ToString();
        }
    }
}
