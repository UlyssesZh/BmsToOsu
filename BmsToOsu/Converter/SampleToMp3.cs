﻿using System.Diagnostics;
using System.Text;
using BmsToOsu.Entity;
using BmsToOsu.Utils;
using NLog;

namespace BmsToOsu.Converter;

public class SampleToMp3
{
    private readonly string _ffmpeg;
    private readonly Option _option;

    private readonly ILogger _log = LogManager.GetCurrentClassLogger();
    private readonly SemaphoreSlim _lock;

    private readonly AudioValidator _validator;

    public SampleToMp3(Option option)
    {
        _option = option;
        var ffmpeg = _option.Ffmpeg;

        _lock = new SemaphoreSlim(_option.MaxThreads, _option.MaxThreads);

        if (string.IsNullOrEmpty(ffmpeg))
        {
            foreach (var path in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';'))
            {
                var p = Path.Join(path, "ffmpeg");

                if (File.Exists(p))
                {
                    _ffmpeg = p;
                    break;
                }

                if (File.Exists(p + ".exe"))
                {
                    _ffmpeg = p + ".exe";
                    break;
                }
            }

            if (string.IsNullOrEmpty(_ffmpeg))
            {
                throw new FileNotFoundException("Can not find ffmpeg in PATH, use `--ffmpeg` to specify the path of ffmpeg");
            }
        }
        else
        {
            _ffmpeg = ffmpeg;
        }

        _validator = new AudioValidator(_ffmpeg);
    }

    private void Generate(
        List<Sample> soundList, string workPath, string output)
    {
        var filter = new FilterComplex();

        foreach (var sound in soundList)
        {
            filter.AddFile(sound.SoundFile, sound.StartTime);
        }

        var argsFile = Path.GetTempPath() + Guid.NewGuid() + ".txt";

        File.WriteAllText(argsFile, filter.GetScript());

        using var p = new Process();

        p.StartInfo.UseShellExecute  = false;
        p.StartInfo.CreateNoWindow   = false;
        p.StartInfo.WorkingDirectory = workPath;
        p.StartInfo.FileName         = _ffmpeg;
        p.StartInfo.Arguments =
            $"-y -hide_banner -loglevel error -filter_complex_script \"{argsFile}\" -map \"[mix]\" -b:a 256k \"{output}\"";

        // Prevent performance degradation due to too many threads
        _lock.Wait();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);

            p.Start();
            p.WaitForExit();
        }
        finally
        {
            _lock.Release();
        }

        if (p.ExitCode != 0)
        {
            throw new Exception("generation failed");
        }

        File.Delete(argsFile);
    }

    public HashSet<string> GenerateMp3(List<Sample> allSoundList, string workPath, string output)
    {
        var invalidSound = _validator.CheckSoundValidity(
            allSoundList.Select(s => s.SoundFile).ToList(), workPath
        ).Result.ToHashSet();

        foreach (var sound in invalidSound)
        {
            _log.Error($"Invalid Sound File: {Path.Join(workPath, sound)}, ignoring...");
        }

        allSoundList = allSoundList.Where(s => !invalidSound.Contains(s.SoundFile)).ToList();

        // ffmpeg can open at most 1300 files
        var groupSize = Math.Max(
            Math.Min((allSoundList.Count + _option.MaxThreads - 1) / _option.MaxThreads, 1300)
          , 10
        );

        var groupedSoundList = new List<(List<Sample> SoundList, string Output)>();

        var n = 0;

        // group sound file list by every X elements
        while (true)
        {
            var l = allSoundList.Skip(n++ * groupSize).Take(groupSize).ToList();
            if (!l.Any()) break;

            groupedSoundList.Add((l, Path.GetTempPath() + Guid.NewGuid() + ".mp3"));
        }

        // generate mp3 in parallel
        Parallel.ForEach(groupedSoundList, g =>
        {
            var startTime = g.SoundList.Min(x => x.StartTime);
            var endTime   = g.SoundList.Max(x => x.StartTime);

            _log.Info(
                $"{workPath}: Generating {TimeSpan.FromMilliseconds(startTime)}-{TimeSpan.FromMilliseconds(endTime)}...");

            Generate(g.SoundList, workPath, g.Output);
        });

        // merge temp mp3 files to result
        {
            _log.Info($"{workPath}: merging...");

            var soundList = groupedSoundList.Select(x => new Sample(0d, x.Output)).ToList();

            Generate(soundList, workPath, output);

            soundList.ForEach(l => File.Delete(l.SoundFile));
        }

        return invalidSound;
    }
}

internal class FilterComplex
{
    private readonly StringBuilder _inputBuilder = new();
    private readonly StringBuilder _delayBuilder = new();
    private readonly StringBuilder _mixBuilder = new();

    private int _fileCount;

    private static readonly List<(string, string)> Escape = new()
    {
        (@"\", @"\\\\"),
        (@"=", @"\\\="),
        (@",", @"\,"),
        (@";", @"\;"),
        (@"[", @"\["),
        (@"]", @"\]"),
        (@":", @"\\\:"),
        (@"'", @"\\\'")
    };

    private static string EscapeWindowsPath(string p)
    {
        return Escape.Aggregate(p, (current, e) => current.Replace(e.Item1, e.Item2));
    }

    public void AddFile(string path, double delay)
    {
        if (string.IsNullOrEmpty(path)) return;

        _inputBuilder.AppendLine($"amovie={EscapeWindowsPath(path)}[input_{_fileCount}];");
        _delayBuilder.AppendLine($"[input_{_fileCount}]adelay={delay:F3}|{delay:F3}[delay_{_fileCount}];");
        _mixBuilder.Append($"[delay_{_fileCount}]");
        _fileCount++;
    }

    public string GetScript()
    {
        return $"{_inputBuilder}{_delayBuilder}{_mixBuilder}amix=inputs={_fileCount}:normalize=0[mix]";
    }
}