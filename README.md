# BMS to osu!mania 7k

> Convert bms files to osu beatmap files, including 7k and 7+1k

[中文](./README.cn.md)

## Usage

### Command Line Arguments (Recommended)

```
  -i, --input       Required. input folder, the program will recursively search
                    for available BMS beatmaps from this folder, available BMS
                    beatmaps: .bms/.bml/.bme/.bmx
  -o, --output      Required. output folder (the output folder will maintain the
                    same directory structure as the input folder)
  --no-sv           (Default: false) weather to include SV
  --no-zip          (Default: false) whether to zip output folder to .osz
  --no-copy         (Default: false) whether to copy sound/image/video files
                    into the output folder
  --no-remove       (Default: false) whether to remove the output folder after
                    zipping it to .osz
  --generate-mp3    (Default: false) generate complete song file from samples of
                    bms
  --ffmpeg          (Default: ) path of ffmpeg (The program will look for ffmpeg
                    in the PATH by default)
  --max-threads     (Default: 10) max number of ffmpeg threads
  --help            Display this help screen.
```

**It is noteworthy** that the input folder can contain multiple songs, and the output folder will process all the songs, such as

```
.\BmsToOsu.exe -i "O:\BMS Song Pack\multi" -o aaa --no-zip
```

where
```
O:\BMS Song Pack\multi
├── song1
│   ├── 1.bms
│   └── 2.bms
└── song2
    ├── 1.bms
    └── 2.bms
```

The output folder is then
```
aaa
├── song1
│   ├── 1.osu
│   ├── 1(7+1k).osu
│   ├── 2.osu
│   ├── ...
│   └── 2(7+1k).osu
└── song2
    ├── 1.osu
    ├── ...
    └── 2.osu
```

At this point, it is recommended to use the `--no-zip` parameter; otherwise, the program will package all of `aaa` into one `osz` file.

### Command Line GUI

If no startup parameters are specified, the program will launch a command line GUI, where the options correspond to the command line parameters:

[![image.png](https://i.postimg.cc/zDbtkwpx/image.png)](https://postimg.cc/hhn1Ddkm)

### Examples

#### Input with Single Song

```
$ .\BmsToOsu.exe \
    -i "O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆" \
    -o aaa --generate-mp3
```

<details>
<summary>
Output
</summary>

``` 
|Info|Use FFMPEG: C:\bin\ffmpeg.exe
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_b7k.bme
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_b7k.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_fz5.bms
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_fz5.bms: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_fz7.bms
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_fz7.bms: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je10.bms
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je10.bms: Double play mode; skipping
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je5.bms
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je5.bms: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_nm7.bme
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_nm7.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_nm7a.bme
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_nm7a.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_plus_system.bme
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_plus_system.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_sla7.bme
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_sla7.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_TML7.bml
|Warn|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_TML7.bml: Bga frame 8a is not founded, ignoring...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:01:18.7499999-00:01:24.2045454...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:34.6022727-00:00:41.2499999...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:01:12.6988636-00:01:18.6647727...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:02.3863636-00:00:12.9545454...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:41.2499999-00:00:56.7613636...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:13.1250000-00:00:26.4204545...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:01:06.8181818-00:01:12.6136363...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:01:24.2045454-00:01:35.1136363...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:56.8465909-00:01:06.8181818...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:26.5056818-00:00:34.6022727...
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: merging...
|Error|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_TML7.bml: Double note at the same time. ignoring...
|Info|Copying files
|Info|Creating F:\workspace\bms\aaa.osz
|Info|Removing F:\workspace\bms\aaa
|Info|------------------------------------------------------------
|Info|Skipped List:
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je10.bms
|Info|O:\BMS Song Pack\Normal\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_TML7.bml
```
</details>

#### Input with Multiple Songs

```
$ .\BmsToOsu.exe -i "O:\BMS Song Pack\multi" -o bbb --generate-mp3 --no-zip
```

<details>
<summary>
Output
</summary>

```
|Warn|`--no-remove` is appended to the parameter
|Info|Use FFMPEG: C:\bin\ffmpeg.exe
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_b7k.bme
|Info|Processing O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\_White_Field_7A.bms
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_b7k.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\_White_Field_7H.bms
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_fz5.bms
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_fz5.bms: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_fz7.bms
|Info|Processing O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\_White_Field_7N.bms
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_fz7.bms: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je10.bms
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je10.bms: Double play mode; skipping
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je5.bms
|Info|Processing O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\10-whitefield.bms
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\10-whitefield.bms: Double play mode; skipping
|Info|Processing O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\5-whitefield-hd.bms
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je5.bms: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_nm7.bme
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_nm7.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_nm7a.bme
|Info|Processing O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\5-whitefield-mx.bms
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_nm7a.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_plus_system.bme
|Info|Processing O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\5-whitefield.bms
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_plus_system.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_sla7.bme
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_sla7.bme: Bga frame 8a is not founded, ignoring...
|Info|Processing O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_TML7.bml
|Warn|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_TML7.bml: Bga frame 8a is not founded, ignoring...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:01:06.8181818-00:01:12.6136363...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:34.6022727-00:00:41.2499999...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:56.8465909-00:01:06.8181818...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:02.3863636-00:00:12.9545454...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:01:12.6988636-00:01:18.6647727...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:13.1250000-00:00:26.4204545...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:01:18.7499999-00:01:24.2045454...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:01:24.2045454-00:01:35.1136363...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:26.5056818-00:00:34.6022727...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: Generating 00:00:41.2499999-00:00:56.7613636...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:00:01.8750000-00:00:26.4843750...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:00:26.4843750-00:00:43.1250000...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:00:43.1250000-00:00:56.7187500...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:00:56.7187500-00:01:09.5312500...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:01:09.6093750-00:01:22.5000000...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:01:22.5000000-00:01:34.6875000...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:01:34.6875000-00:01:44.1796875...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:01:44.1796875-00:01:52.4218750...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:01:52.5000000-00:02:00.2343750...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: Generating 00:02:00.3515625-00:02:18.7500000...
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆: merging...
|Error|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_TML7.bml: Double note at the same time. ignoring...
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field: merging...
|Info|Copying files
|Info|------------------------------------------------------------
|Info|Skipped List:
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_je10.bms
|Info|O:\BMS Song Pack\multi\[#ねここ14歳] ☆ さくらなみきのかぜ ☆\_nekoko14_TML7.bml
|Info|O:\BMS Song Pack\multi\[[IX] mov ： Optie] White Field\10-whitefield.bms
```
</details>

## Credits

- [vysiondev](https://github.com/vysiondev)

## License

GNU AFFERO GENERAL PUBLIC LICENSE

see [LICENSE](./LICENSE) for more information.