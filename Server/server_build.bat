SET GOPATH=%cd%
go build -o ../Build/Server/server.exe server
go build -o ../Build/Server/dummy.exe dummyClient