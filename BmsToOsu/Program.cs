﻿using System.IO.Compression;
using BmsToOsu;
using BmsToOsu.Converter;
using BmsToOsu.Entity;
using CommandLine;
using log4net;

Logger.Config();

var logger = LogManager.GetLogger("");

Parser.Default.ParseArguments<Option>(args)
    .WithParsed(o =>
    {
        foreach (var song in Directory.GetDirectories(o.InputPath).Take(o.Number <= 0 ? int.MaxValue : o.Number))
        {
            var bms = Directory.GetFiles(song, "*.bms", SearchOption.TopDirectoryOnly);

            var ftc = new HashSet<string>();

            var dest = Path.Join(o.OutPath, Path.GetFileNameWithoutExtension(song));

            var osz = dest + ".osz";

            if (File.Exists(osz))
            {
                logger.Warn($"{osz} exists, ignoring...");
                continue;
            }

            foreach (var fp in bms.Select(Path.GetFileName))
            {
                var data = BmsFileData.FromFile(Path.Join(song, fp));

                var (osu, ftc2) = data.ToOsuBeatMap();

                foreach (var c in ftc2) ftc.Add(c);

                Directory.CreateDirectory(dest);

                File.WriteAllText(Path.Join(dest, Path.GetFileNameWithoutExtension(fp) + ".osu"), osu);
            }

            foreach (var c in ftc)
            {
                File.Copy(Path.Join(song, c), Path.Join(dest, c), true);
            }

            if (File.Exists(osz)) File.Delete(osz);

            ZipFile.CreateFromDirectory(dest, osz, CompressionLevel.Fastest, false);

            Directory.Delete(dest, true);
        }
    });