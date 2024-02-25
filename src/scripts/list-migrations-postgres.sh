#!/bin/sh
HERE=$(dirname $(readlink -f "$0"))

dotnet ef migrations list --startup-project Invoicer.App --project Invoicer.Migrations/Invoicer.Postgres -- --provider Postgres

