#!/bin/sh
dotnet build --configuration Release SortingNetworks/SortingNetworks.csproj

dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:8 -k:19 -t:126
dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:9 -k:25 -t:126
