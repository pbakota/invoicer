all: build

build:

package:
	cd src && rm -rf ../releases/linux-x64 && dotnet publish -f "net8.0" -c Release Invoicer.App -r linux-x64 --output ../releases/linux-x64 --no-self-contained && \
        cd ../releases/linux-x64 && tar -zcf ../invoicer-linux-x64.tgz * && cd -

deploy-all:
	make deploy-docker && make deploy-app

deploy-docker:
	rsync -a --info=progress2 docker/* root@playbox:~/csharp-invoicer-docker

deploy-app:
	rsync -a --info=progress2 releases/invoicer-linux-x64.tgz root@playbox:~/csharp-invoicer-docker/

