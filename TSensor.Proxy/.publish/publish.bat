cd ..
dotnet publish -r linux-arm /p:ShowLinkerSizeComparison=true --self-contained=false
pushd .\bin\Debug\netcoreapp3.1\linux-arm\publish
..\..\..\..\..\.publish\pscp.exe -pw tsensor -P 4466 -v -r .\* pi@109.167.253.104:/home/pi/proxy2
popd
cd .publish