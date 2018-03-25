from os import listdir
from os import system
from os.path import isfile, join

targetPath = "schema"
buildCmdPrefix = r'bin\flatc.exe --csharp --gen-onefile -o ..\UnityProject\SHU\Assets\Scripts\FlatBuffers '

files = [(targetPath + "\\" + f) for f in listdir(targetPath) if isfile(join(targetPath, f))]

buildCmd = buildCmdPrefix + " ".join(files)

print(buildCmd)
system(buildCmd)