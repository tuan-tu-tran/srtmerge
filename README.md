Tools to merge srt files.

The console app CAppSrtMerge is .NET code console app that reads two srt files and merges them.
The resulting subtitles are aligned and colorized.
Typically, this is used to merge subtitle files in different languages.

## How to build

Just emit:

```
dotnet publish
```

And it will generate binaries in `CAppSrtMerge/bin/Debug/.../publish`.

## How to install

Just put the result of the build somewhere and put this shell script somewhere in your `PATH`

```
dotnet-runtime-21.dotnet /path/to/CAppSrtMerge.dll $*
```

So it does require the .NET runtime 2.1, which can be installed with [Snap](https://snapcraft.io/dotnet-runtime-21).

