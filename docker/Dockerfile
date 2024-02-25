FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN echo "deb http://deb.debian.org/debian/ bookworm main contrib non-free" >> /etc/apt/sources.list
RUN apt-get update && \
	DEBIAN_FRONTEND=noninteractive apt-get install ttf-mscorefonts-installer -y
ADD invoicer-linux-x64.tgz /app/
WORKDIR /app
RUN chmod +x /app/Invoicer.App
ENTRYPOINT ["/app/Invoicer.App", "--provider", "Postgres"]
