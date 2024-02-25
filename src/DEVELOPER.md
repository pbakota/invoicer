# Initial migration
```shell
dotnet ef migrations add Initial --project Invoicer.Migrations
dotnet ef database update --project Invoicer.Migrations
```
# Other migrations
```shell
dotnet ef migrations add [name] --project Invoicer.Migrations
dotnet ef database update --project Invoicer.Migrations
```
# Create component
```shell
dotnet new razorcomponent -n [name] -o Invoicer.App/Components/Pages
```


# Scaffolding tool
```shell
dotnet tool install --global dotnet-aspnet-codegenerator --version 8.0.1
```