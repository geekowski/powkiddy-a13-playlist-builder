# Powkiddy A13 Playlist Builder

Building Windows executable file

```
dotnet publish --output "./out" --runtime win-x64 --configuration Release -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true
```
## How it works

Just insert exe file in root of the TF card. Playlists are created based on directory content.

