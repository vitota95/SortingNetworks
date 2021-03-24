#!/bin/sh
dotnet build --configuration Release SortingNetworks/SortingNetworks.csproj -optimize
dotnet build --configuration Debug SortingNetworks/SortingNetworks.csproj

#dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:11 -k:35 -h:500 -t:12 -l:11log.txt
#dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:12 -k:39 -h:500 -t:12 -l:12log.txt
dotnet SortingNetworks/bin/Debug/net5.0/SortingNetworks.dll -s:19 -k:2 -h:500 -t:12 -l:19log.txt
dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:19 -k:88 -h:500 -t:12 -l:19log.txt
dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:14 -k:53 -h:500 -t:12 -l:14log.txt
