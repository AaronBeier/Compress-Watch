# Compress & Watch
Compress & Watch is a simple file compressor that will compress files in a given directory with [gzip](https://en.wikipedia.org/wiki/Gzip), [zstd](https://en.wikipedia.org/wiki/Zstd) and [brotli](https://en.wikipedia.org/wiki/Brotli).
The name is inspired by [Nintendo's old Game & Watch electronic handheld games](https://en.wikipedia.org/wiki/Game_%26_Watch). It uses [.NET's FileSystemWatcher](https://learn.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher) to automatically update the compressed files.
I made Compress & Watch specifically for compressing static web assets like CSS, JS and HTML files, but it should™ work with all files.

## Usage
By default Compress & Watch does not include subdirectories and watches all files (*.*)

Watch a folder:  
`dotnet CompressWatch.dll /srv/http/project/`  

Watch a folder recursively:  
`dotnet CompressWatch.dll /srv/http/project/ -r`  

Watch a folder with a custom filter:  
`dotnet CompressWatch.dll /srv/http/project/ -f *.txt,*.log`

## Known Issues
FileSystemWatcher triggers twice sometimes, because it sees certain modifications as two separate events. Compress & Watch does not filter these duplicate events and will compress your files twice.
https://stackoverflow.com/questions/1764809/filesystemwatcher-changed-event-is-raised-twice
I tried many things to fix this, including a MemoryCache idea from https://www.codeproject.com/Articles/1220093/A-Robust-Solution-for-FileSystemWatcher-Firing-Eve, but nothing really passed all my tests.