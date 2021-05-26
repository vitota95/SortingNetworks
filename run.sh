#!/bin/sh
dotnet build --configuration Release SortingNetworks/SortingNetworks.csproj
mkdir -p outputs/

for population in 100 250 500 1000 5000 10000
do
	for i in {9..16}
	do
		dotnet SortingNetworks/bin/Release/net5.0/SortingNetworks.dll -s:$i -k:100 -t:126 -z -h:$population -l:log.txt
	done
done
