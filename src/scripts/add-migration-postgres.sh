#!/bin/sh
HERE=$(dirname $(readlink -f "$0"))

if [ $# -lt 1 ]; then
	echo "usage: $0 <migration name>"
	exit 0
fi

NAME=$1
dotnet ef migrations add $NAME --startup-project Invoicer.App --project Invoicer.Migrations/Invoicer.Postgres -- --provider Postgres
