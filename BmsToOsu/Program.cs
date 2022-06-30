﻿using System.IO.Compression;
using BmsToOsu;
using BmsToOsu.Converter;
using BmsToOsu.Entity;
using CommandLine;
using log4net;

Logger.Config();

var logger = LogManager.GetLogger("");

var availableBmsExt = new[]
{
    ".bms", ".bml", ".bme", ".bmx"
};

Parser.Default.ParseArguments<Option>(args)
    .WithParsed(o =>
    {
        var osz = o.OutPath.EndsWith(".osz", StringComparison.OrdinalIgnoreCase) ? o.OutPath : o.OutPath + ".osz";

        if (Directory.Exists(o.OutPath))
        {
            logger.Warn($"{o.OutPath} exists, `--no-remove` will be appended to the parameter");
            o.NoRemove = true;
        }

        if (File.Exists(osz))
        {
            logger.Warn($"{osz} exists, ignoring...");
            return;
        }

        var ftc = new HashSet<string>();

        var bms = Directory
            .GetFiles(o.InputPath, "*.*", SearchOption.AllDirectories)
            .Where(f => availableBmsExt.Any(ext => f.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));

        foreach (var fp in bms)
        {
            try
            {
                logger.Info($"Processing {fp}");

                var dir = Path.GetDirectoryName(fp) ?? "";

                var data = BmsFileData.FromFile(fp);

                var (osu, ftc2) = data.ToOsuBeatMap();

                foreach (var c in ftc2) ftc.Add(Path.Join(dir, c));

                var dest = dir.Replace(o.InputPath, o.OutPath);

                Directory.CreateDirectory(dest);
                File.WriteAllText(Path.Join(dest, Path.GetFileNameWithoutExtension(fp) + ".osu"), osu);
            }
            catch (InvalidDataException)
            {
            }
        }

        if (!o.NoCopy)
        {
            logger.Info("Copying sound files");
            Parallel.ForEach(ftc, c =>
            {
                var dest = c.Replace(o.InputPath, o.OutPath);

                if (!File.Exists(dest))
                {
                    File.Copy(c, dest, true);
                }
            });
        }

        if (!o.NoZip && Directory.Exists(o.OutPath))
        {
            logger.Info($"Creating {osz}");
            ZipFile.CreateFromDirectory(o.OutPath, osz, CompressionLevel.Fastest, false);
        }

        if (!o.NoRemove)
        {
            logger.Info($"Removing {o.OutPath}");
            Directory.Delete(o.OutPath, true);
        }
    });