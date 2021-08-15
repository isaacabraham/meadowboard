dotnet build -c Release
meadow app deploy -f bin\Release\netstandard2.1\App.dll
meadow listen