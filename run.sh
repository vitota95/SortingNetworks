#!/bin/sh
dotnet build --configuration Release SortingNetworks/SortingNetworks.csproj
dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:9 -k:25 -t:126 -l:run9Improved.txt
dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:11 -k:35 -t:126 -l:run11Improved.txt
