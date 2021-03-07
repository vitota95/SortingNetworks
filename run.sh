#!/bin/sh
dotnet build --configuration Release SortingNetworks/SortingNetworks.csproj

dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:15 -k:56 -h:50000 -t:126
dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:16 -k:60 -h:50000 -t:126
