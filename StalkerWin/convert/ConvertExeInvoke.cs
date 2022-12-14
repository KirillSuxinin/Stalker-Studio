using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Stalker_Studio.StalkerWin.convert
{
    public static class ConvertExeInvoke
    {
        public static string ConvertExe = $"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\converter.exe";

        public static FileInfo Convert(string nameFileToConvert,string outputFile,format_specific_option[] formats,string _GAME_DATA_ = null)
        {
            //delete other datas
            string[] vs = Directory.GetFiles($"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\");
            foreach (var v in vs)
            {
                if (MainWindow.ProgramData.GetLastSplash(v).ToUpper() != "converter.exe".ToUpper() && MainWindow.ProgramData.GetLastSplash(v).ToUpper() != "fsconverter.ltx".ToUpper())
                    File.Delete(v);
            }
            if(!string.IsNullOrWhiteSpace(nameFileToConvert))
            if (!File.Exists($"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\{MainWindow.ProgramData.GetLastSplash(nameFileToConvert)}"))
                File.Copy(nameFileToConvert, $"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\{MainWindow.ProgramData.GetLastSplash(nameFileToConvert)}");
            if(!string.IsNullOrWhiteSpace(outputFile))
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            string wPath = _GAME_DATA_;

            if (_GAME_DATA_ == null && MainWindow.ProgramData.Gamedata != null)
                wPath = MainWindow.ProgramData.Gamedata;

            if (Directory.Exists(_GAME_DATA_))
            {
                string[] lines = File.ReadAllLines($"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\fsconverter.ltx", Encoding.GetEncoding(1251));
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith("$game_data$"))
                    {
                        lines[i] = $"$game_data$             = false| true|  $fs_root$|            {wPath}";
                    }
                }
                File.WriteAllLines($"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\fsconverter.ltx", lines, Encoding.GetEncoding(1251));
            }


            string expander = $"@echo off{Environment.NewLine}cd AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32{Environment.NewLine}converter.exe";

            string inputName = MainWindow.ProgramData.GetLastSplash(nameFileToConvert);
            string outputName = MainWindow.ProgramData.GetLastSplash(outputFile);
            string outputOrig = "";
            foreach (var format in formats)
            {
                switch (format)
                {
                    case format_specific_option.ogf:
                        expander += $" -ogf {inputName}";
                        break;
                    case format_specific_option.@object:
                        expander += $" -object {outputName.Split('.')[0]}.object";
                        outputOrig = $"{outputName.Split('.')[0]}.object";
                        break;
                    case format_specific_option.skls:
                        expander += $" -skls {outputName.Split('.')[0]}.skls";
                        outputOrig = outputName.Split('.')[0] + ".skls";
                        break;
                    case format_specific_option.skl:
                        expander += $" -skl {outputName.Split('.')[0]}.skl";
                        outputOrig = outputName.Split('.')[0] + ".skl";
                        break;
                    case format_specific_option.bones:
                        expander += $" -bones {outputName.Split('.')[0]}.bones";
                        outputOrig = outputName.Split('.')[0] + ".bones";
                        break;
                    case format_specific_option.omf:
                        expander += $" -omf {outputName.Split('.')[0]}.omf";
                        outputOrig = outputName.Split('.')[0] + ".omf";
                        break;
                    case format_specific_option.dm:
                        expander += $" -dm -object {inputName} -out {outputName.Split('.')[0]}.object";
                        outputOrig = outputName.Split('.')[0] + ".object";
                        break;
                    case format_specific_option.level:
                        expander += $" -level";
                        break;
                    case format_specific_option.mode_maya:
                        expander += $" -mode maya";
                        break;
                    case format_specific_option.mode_le:
                        expander += " -mode le";
                        break;
                    case format_specific_option.mode_le2:
                        expander += " -mode le2";
                        break;
                    case format_specific_option.with_lods:
                        expander += " -with_lods";
                        break;
                    case format_specific_option.fancy:
                        expander += " -fancy";
                        break;
                    case format_specific_option.ogg2wav:
                        expander += $" -ogg2wav {inputName} -out {outputName.Split('.')[0]}.wav";
                        outputOrig = outputName.Split('.')[0] + ".wav";
                        break;
                    case format_specific_option.dds2tga:
                        expander += $" -dds2tga {inputName} -out {outputName.Split('.')[0]}.tga";
                        outputOrig = outputName.Split('.')[0] + ".tga";
                        break;
                    case format_specific_option.with_solid:
                        expander += " -with_solid";
                        break;
                    case format_specific_option.with_bump:
                        expander += " -with_bump";
                        break;

                    default:

                        break;
                }


            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(expander);
            Console.ForegroundColor = ConsoleColor.White;

            File.WriteAllText($"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\bat.bat", expander, Encoding.Default);
            string d = $"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\bat.bat";
            ProcessStartInfo pr = new ProcessStartInfo(d) { CreateNoWindow = false, UseShellExecute = false, RedirectStandardOutput = true,RedirectStandardInput = true };
            Process proc = Process.Start(pr);
            proc.WaitForExit();

            int counterWaiter = 0;
            const int maxCounter = 100;
            while(counterWaiter < maxCounter)
            {
                System.Threading.Thread.Sleep(50);

                if (File.Exists($"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\{outputOrig}"))
                {
                    return new FileInfo($"{System.Windows.Forms.Application.StartupPath}\\AddonSoft\\Convert\\converter_2015-06-24\\converter\\win32\\{outputOrig}");
                }
                counterWaiter++;

            }

            return null;
        }


        public enum format_specific_option
        {
            None = -1,
            ogf,
            @object,
            skls,
            skl,
            bones,
            omf,
            dm,
            level,
            mode_maya,
            mode_le,
            mode_le2,
            with_lods,
            fancy,
            ogg2wav,
            dds2tga,
            with_solid,
            with_bump
        }

        public enum DB
        {
            None = -1,
            unpack,
            pack,
            _11xx,
            _2215,
            _2945,
            _2947ru,
            _2947ww,
            xdb,
            xdb_ud,
            fls
        }

        /// <summary>
        /// Изучи логику конверта дебил
        /// </summary>
        /// <param name="dirLevel">Путь где лежит наш уровень (gamedata\levels)</param>
        /// <param name="Sborka">Код сборки</param>
        /// <param name="nameLevel">Имя уровня</param>
        /// <param name="outName">Выход имя</param>
        /// <param name="modes">Моды</param>
        public static void ConvertLevel(string dirLevel,string Sborka,string nameLevel,string outName, string[] modes)
        {
            //convert.exe -level C:\Games\SoC\gamedataUE\levels default:l01_escape -out l01_escape_esc -mode le

            string expander = "converter.exe";
            File.Copy(ConvertExe, $"{dirLevel.TrimEnd('\\')}\\converter.exe");
            string IConverter = $"{dirLevel.TrimEnd('\\')}\\converter.exe";
            expander += $" -level {Sborka}:{nameLevel} -out {outName}";
            if(modes.Length > 0)
            {
                expander += " -mode";
                foreach (string vm in modes)
                    expander += " " + vm;
            }

            string batCommand = $"@echo off{Environment.NewLine}@chcp 1251{Environment.NewLine}cd {dirLevel}{Environment.NewLine}{expander}";
            File.WriteAllText(dirLevel.TrimEnd('\\') + "\\conv.bat", batCommand, Encoding.Default);
            Process bat = Process.Start(dirLevel.TrimEnd('\\') + "\\conv.bat");
            bat.WaitForExit();
        }
    }
}
