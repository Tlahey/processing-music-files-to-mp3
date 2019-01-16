# processing-music-files-to-mp3
Tool to convert music files of various format to mp3. Formatting the name, adding lyrics ...

Need to convert .net Framwork 4.5 app to .net Core 2 App

# How to compile

Build the 3 projects with `msbuild TraitementFichierMp3.sln`

# How to launch

On mac `mono <path to your app> <your app's arguments>`

# Application configuration 

Change config file `TraitementFichierMp3.exe.config`

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5" />
    </startup>
    <appSettings>
      <add key="musiqueDirectory" value="C:\Users\Antoine\Music\A mettre sur le NAS"/>
      <add key="formatFile" value="{track} - {title}"/>
      <add key="savePath" value="c:\temp\logTraitementmp3.txt"/>
      <add key="searchFiles" value="*.mp3,*.wma,*.m4a,*.flac"/>
    
      <!-- 
          Transform File 
              1 - TRUE
              0 - FALSE
      -->
      
      <!-- Get lyrics from internet -->
      <add key="Lyrics" value="0"/>  
      
      <!-- Rename file with metadatas -->
      <add key="Rename" value="1"/>
      
      <!-- Force to update metadatas with file name -->
      <add key="ForceFileNameToMetaData" value="0"/>
  
      <!-- Convert to MP3 files -->
      <add key="ConvertMP3" value="0"/>
      <add key="DebitEncodage" value="320"/>
      <add key="DeleteOldFile" value="0"/>
    
    </appSettings>
</configuration>
```