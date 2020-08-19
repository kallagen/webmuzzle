cd ..
dotnet publish -r linux-x64 /p:ShowLinkerSizeComparison=true --self-contained=false
pushd .\bin\Debug\netcoreapp3.1\linux-x64\publish
..\..\..\..\..\.publish\pscp.exe -pw iuJNhjhF3 -v -r .\* root@185.230.141.200:/var/www/fakesensor
popd
cd .publish