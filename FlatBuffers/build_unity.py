from os import listdir
from os import system
from os.path import isfile, join

targetPath = "schema"
buildCmdPrefix = r'./bin/flatc --csharp --gen-onefile -o ../UnityProject/SHU/Assets/Scripts/FlatBuffers '

files = [(targetPath + "/" + f) for f in listdir(targetPath) if isfile(join(targetPath, f))]

buildCmd = buildCmdPrefix + " ".join(files)

print(buildCmd)
system(buildCmd)

buildCmdPrefix = r'./bin/flatc --go --gen-onefile -o ../Server/src/protocol '

files = [(targetPath + "/" + f) for f in listdir(targetPath) if isfile(join(targetPath, f))]

buildCmd = buildCmdPrefix + " ".join(files)

print(buildCmd)
system(buildCmd)