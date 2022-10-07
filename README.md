# Archiver5E2D
## About The Project

This library is being created as part of a laboratory work on the subject "Fundamentals of information theory and coding".
It allows you to archive files and folders using compression.

## Getting Started

The Archiver5E2D library contains a static Archiver class with Archive and Dearchive methods.
Let's take a step-by-step look at how to archive and unarchive a file and folder.

### Archive
Use the Archive method to archive a file or folder by passing the appropriate paths, and then use the Create method to save the archive to the desired path.
If desired path to create does not exist, it will be created.

File
```C#
const string filePath = @"C:\PATH\FILE_NAME.FORMAT";
const string saveFilePath = @"C:\SAVE_PATH\FOLDER_NAME";
var archivedFile = Archiver.Archive(filePath);
archivedFile.Create(saveFilePath);
```

Folder
```C#
const string folderPath = @"C:\PATH\FOLDER_NAME";
const string saveFolderPath = @"C:\SAVE_PATH\FOLDER_NAME";
var archivedFolder = Archiver.Archive(folderPath);
archivedFolder.Create(saveFolderPath);
```

### Unarchive
Use the archive method to unzip the file or folder, bypassing the appropriate paths,
and then use the Creation method for all dearchived objects to save them to the desired folder.

File
```C#
var unarchivedFile = Archiver.Dearchive(Path.Combine(saveFilePath, archivedFile.Name));
foreach (var entity in unarchivedFile)
    entity.Create(saveFilePath);
```

Folder
```C#
var unarchivedFolder = Archiver.Dearchive(Path.Combine(saveFolderPath, archivedFolder.Name));
foreach (var entity in unarchivedFolder)
    entity.Create(saveFolderPath);
```

## Roadmap

- [x] Add files archiving/unarchiving
- [x] Add folders archiving/unarchiving
- [X] Add subfolders archiving/unarchiving
- [ ] Add compression algorithms

## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

## Contact

Your Name - [Dmitrii](https://t.me/kondim007) - kondim007@gmail.com

Project Link: [https://github.com/Dikin01/Archiver5E2D](https://github.com/Dikin01/Archiver5E2D)
