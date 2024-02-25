#!/bin/bash

if [[ $# -lt 2 ]]; then
	echo "usage: $0 <path> <component>"
	exit 0
fi

dotnet new razorcomponent -n $2 -o Invoicer.App/Components/$1

