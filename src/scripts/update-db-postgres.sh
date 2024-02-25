#!/bin/sh
HERE=$(dirname $(readlink -f "$0"))

dotnet ef database update --startup-project Invoicer.App --project Invoicer.Migrations/Invoicer.Postgres -- --provider Postgres
