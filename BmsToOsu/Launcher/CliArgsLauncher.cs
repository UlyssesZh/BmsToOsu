﻿using System.IO.Compression;
using BmsToOsu.Converter;
using BmsToOsu.Entity;
using BmsToOsu.Utils;
using CommandLine;
using CommandLine.Text;
using NLog;

namespace BmsToOsu.Launcher;

public static class CliArgsLauncher
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    private static readonly string[] AvailableBmsExt =
    {
        ".bms", ".bml", ".bme", ".bmx"
    };

    public static void Launch(string[] args)
    {
        var argsParser = new Parser(with =>
        {
            with.AutoVersion = false;
            with.AutoHelp    = true;
            with.HelpWriter  = null;
        });

        var result = argsParser.ParseArguments<Option>(args);

        result.WithParsed(Convert);

        result.WithNotParsed(_ =>
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AutoHelp                      = true;
                h.AutoVersion                   = false;
                h.AutoVersion                   = false;
                h.AdditionalNewLineAfterOption  = false;
                h.AddNewLineBetweenHelpSections = false;
                h.Heading                       = "";
                h.Copyright                     = Const.Copyright;
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        });
    }

    private static IEnumerable<string> RetrieveFilesFromNetworkDevice(string root)
    {
        var d = new Dictionary<int, List<string>>
        {
            [0] = new() { root }
        };

        var current = 0;

        while (true)
        {
            if (d[current].Count == 0) break;

            var next = current + 1;

            d[next] = new List<string>();

            Parallel.ForEach(d[current].Distinct(), x =>
            {
                var d1 = Directory.GetDirectories(x, "*", SearchOption.TopDirectoryOnly);
                lock (d) d[next].AddRange(d1);
            });

            current += 1;
        }

        return d.SelectMany(x => x.Value)
            .AsParallel()
            .SelectMany(x => Directory
                .GetFiles(x, "*.*", SearchOption.TopDirectoryOnly))
            .Distinct()
            .AsSequential();
    }

    public static void Convert(Option o)
    {
        o.OutPath   = Path.GetFullPath(o.OutPath);
        o.InputPath = Path.GetFullPath(o.InputPath);
        o.Ffmpeg    = string.IsNullOrEmpty(o.Ffmpeg) ? "" : Path.GetFullPath(o.Ffmpeg);

        var osz = o.OutPath + ".osz";

        #region check options

        // avoid removing existing folder
        if (Directory.Exists(o.OutPath) && !o.NoRemove)
        {
            Log.Warn($"{o.OutPath} exists, `--no-remove` will be appended to the parameter");
            o.NoRemove = true;
        }

        if (o.NoSv && o.IncludeNoSv)
        {
            o.IncludeNoSv = false;
        }

        if (o.NoCopy && o.GenerateMp3)
        {
            Log.Error($"`--no-copy` is conflict with `--generate-mp3`");
            return;
        }

        // avoid removing after generation
        if (o.NoZip && !o.NoRemove)
        {
            Log.Warn("`--no-remove` is appended to the parameter");
            o.NoRemove = true;
        }

        // avoid duplication
        if (File.Exists(osz))
        {
            Log.Warn($"{osz} exists, ignoring...");
            return;
        }

        #endregion

        #region parse & convert

        var bms = RetrieveFilesFromNetworkDevice(o.InputPath)
            .Where(f => AvailableBmsExt.Any(ext => f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var skippedFileList      = new List<string>();
        var generationFailedList = new List<string>();

        var converter = new Converter(o, bms.Count);

        Parallel.ForEach(bms.GroupBy(Path.GetDirectoryName), groupedBms => Proc(groupedBms.ToArray()));

        #endregion

        #region after convertion (e.g. remove temp files)

        if (!o.NoCopy)
        {
            Log.Info("Copying files");
            Parallel.ForEach(converter.FilesToCopy, x =>
            {
                if (!PathExt.FileExists(x.Item2)) File.Copy(x.Item1, x.Item2);
            });
        }

        if (!o.NoZip && Directory.Exists(o.OutPath))
        {
            Log.Info($"Creating {osz}");
            ZipFile.CreateFromDirectory(o.OutPath, osz, CompressionLevel.Fastest, false);
        }

        if (!o.NoRemove)
        {
            Log.Info($"Removing {o.OutPath}");
            Directory.Delete(o.OutPath, true);

            if (o.OutPath.EndsWith(".osz", StringComparison.OrdinalIgnoreCase))
            {
                File.Move(osz, o.OutPath);
            }
        }

        if (skippedFileList.Any())
        {
            Log.Info(new string('-', 60));
            Log.Info("Skipped List:");

            skippedFileList.ForEach(path => Log.Info(path));
        }

        if (generationFailedList.Any())
        {
            Log.Info(new string('-', 60));
            Log.Info("Generation Failed List:");

            generationFailedList.ForEach(path => Log.Info(path));
        }

        #endregion

        return;

        void Proc(params string[] path)
        {
            try
            {
                converter.Convert(path);
            }
            catch (BmsParserException e)
            {
                lock (skippedFileList) skippedFileList.AddRange(e.FailedList);
            }
            catch (GenerationFailedException e)
            {
                lock (generationFailedList) generationFailedList.AddRange(e.FailedList);
            }
        }
    }
}

internal class Converter
{
    private readonly Option _option;
    private readonly SampleToMp3 _mp3Generator;
    private readonly ILogger _log = LogManager.GetCurrentClassLogger();
    private readonly int _maxCount;
    private readonly CountdownEvent _countdown;


    public Converter(Option option, int count)
    {
        _option       = option;
        _mp3Generator = new SampleToMp3(_option);
        _countdown    = new CountdownEvent(count);
        _maxCount     = count;
    }

    public readonly HashSet<(string, string)> FilesToCopy = new();

    private void ConvertOne(BmsFileData data, string mp3Path, HashSet<Sample> excludingSamples, string title, string artist)
    {
        var bmsDir    = Path.GetDirectoryName(data.BmsPath) ?? "";
        var outputDir = bmsDir.Replace(_option.InputPath, _option.OutPath);

        Directory.CreateDirectory(outputDir);

        foreach (var includePlate in new[] { true, false })
        {
            foreach (var sv in _option is { NoSv: false, IncludeNoSv: true } ? new[] { true, false } : new[] { _option.NoSv })
            {
                var (osuBeatmap, ftc) =
                    data.ToOsuBeatMap(excludingSamples, sv, mp3Path, includePlate, !_option.NoBga);

                foreach (var c in ftc)
                {
                    var fn   = Path.GetFileName(c);
                    var dest = Path.Join(outputDir, fn.Escape());

                    lock (FilesToCopy) FilesToCopy.Add((Path.Join(bmsDir, c), dest));
                }

                var plate = includePlate ? " (7+1K)" : "";

                var osuName =
                    $"{title} - {artist} - BMS Converted{plate} - {Path.GetFileNameWithoutExtension(data.BmsPath)}.osu";

                File.WriteAllText(Path.Join(outputDir, osuName.MakeValidFileName()), osuBeatmap);
            }
        }
    }

    public void Convert(string[] bmsFiles)
    {
        bmsFiles = bmsFiles.OrderBy(s => s).ToArray();

        if (!bmsFiles.Any()) return;

        List<Sample>? soundFileList = null;

        var dataList       = new List<BmsFileData>();
        var parseErrorList = new List<string>();

        var workPath = Path.GetDirectoryName(bmsFiles[0])!;

        foreach (var bmsFilePath in bmsFiles)
        {
            _log.Info($"Processing {bmsFilePath}");

            BmsFileData data;

            try
            {
                data = BmsFileData.FromFile(bmsFilePath);
            }
            catch (InvalidBmsFileException)
            {
                parseErrorList.Add(bmsFilePath);
                continue;
            }

            dataList.Add(data);

            if (!_option.GenerateMp3) continue;

            var soundFiles = data.GetSoundFileList();

            soundFileList ??= soundFiles;
            soundFileList =   soundFileList.Intersect(soundFiles, Sample.Comparer).ToList();
        }

        if (!dataList.Any()) return;

        var title  = dataList.Select(x => x.Metadata.Title).ToList().MostCommonPrefix();
        var artist = dataList.Select(x => x.Metadata.Artist).ToList().MostCommonPrefix();

        var filename = _option.GenerateMp3 && soundFileList != null && soundFileList.Any()
            ? $"{title} - {artist}.mp3".MakeValidFileName()
            : "";

        var mp3 = Path.Join(
            Path.GetDirectoryName(dataList[0].BmsPath)!
                .Replace(_option.InputPath, _option.OutPath)
          , filename
        );

        var invalidSound = new HashSet<string>();

        if (_option.GenerateMp3)
        {
            try
            {
                if (File.Exists(mp3))
                {
                    _log.Warn($"{workPath}: {mp3} exists, skipping...");
                }
                else
                {
                    if (soundFileList != null && soundFileList.Any())
                    {
                        invalidSound = _mp3Generator.GenerateMp3(soundFileList, workPath, mp3);
                    }
                    else
                    {
                        _log.Warn($"{workPath}: The sampling intersection of the same song is too small, use hit sound instead.");
                    }
                }
            }
            catch
            {
                throw new GenerationFailedException(bmsFiles);
            }
        }

        Parallel.ForEach(dataList, data =>
        {
            try
            {
                // filter invalid sound file
                data.HitObject.Values.SelectMany(h => h)
                    .Where(h => invalidSound.Contains(h.HitSoundFile)).ToList()
                    .ForEach(h => h.HitSoundFile = "");
                data.SoundEffects.RemoveAll(s => invalidSound.Contains(s.SoundFile));

                var soundExclude = new HashSet<Sample>(soundFileList ?? new List<Sample>(), Sample.Comparer);

                ConvertOne(data, filename, soundExclude, title, artist);
            }
            catch (InvalidNoteConfigException)
            {
                lock (parseErrorList) parseErrorList.Add(data.BmsPath);
            }
            catch (AggregateException aggregateException)
            {
                foreach (var e in aggregateException.Flatten().InnerExceptions)
                {
                    if (e is not InvalidNoteConfigException) throw e;

                    lock (parseErrorList) parseErrorList.Add(data.BmsPath);
                    break;
                }
            }
            finally
            {
                _countdown.Signal();

                if (_countdown.CurrentCount % 100 == 0)
                {
                    _log.Info("====================================================");
                    _log.Info($"Remaining: {_countdown.CurrentCount}/{_maxCount}");
                    _log.Info("====================================================");
                }
            }
        });

        if (parseErrorList.Any())
        {
            throw new BmsParserException(parseErrorList);
        }
    }
}