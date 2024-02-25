
all: build

build:
	dotnet build -c Debug

build-electron:
	cd Invoicer.App && dotnet build -c Debug -p:ExtraDefineConstants=ELECTRON_APP

build-electron-linux:
	cd Invoicer.App && electronize build /target linux /p:ExtraDefineConstants=ELECTRON_APP

build-electron-win:
	cd Invoicer.App && electronize build /target win /p:ExtraDefineConstants=ELECTRON_APP

clean:
	dotnet clean

run-sqlite:
	dotnet watch run -c Debug --project Invoicer.App/Invoicer.App.csproj -- --provider Sqlite

run-sqlite-release:
	dotnet run -c Release --project Invoicer.App/Invoicer.App.csproj -- --provider Sqlite

run-postgres:
	dotnet watch run -c Debug --project Invoicer.App/Invoicer.App.csproj -- --provider Postgres

run-electron:
	cd Invoicer.App && _ENVIRONMENT=Development ConnectionStrings__Sqlite="Data Source=$(PWD)/../data/invoicer.db" provider=Sqlite ExtraDefineConstants=ELECTRON_APP \
		electronize start /dotnet-configuration Debug /clear-cache

run-electron-production:
	cd Invoicer.App && provider=Sqlite ExtraDefineConstants=ELECTRON_APP electronize start /dotnet-configuration Release

launch:
	nohup chromium --accept-lang=sr --flag-switches-begin --allow-insecure-localhost  --allow-running-insecure-content --flag-switches-end --user-data-dir=/tmp/$(shell uuid) \
		--url http://localhost:5000 >/dev/null 2>&1 &
