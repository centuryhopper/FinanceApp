.PHONY: build clean copy all

# Composite target: run all in order
all: clean build copy run

# Clean the wwwroot directory
clean:
	rm -rf ./Server/wwwroot
	rm -rf ./Client/dist

# Build the Vue client app
build:
	cd ./Client && npm run build

# Copy the built files into the server's wwwroot
copy:
	cp -r ./Client/dist ./Server/wwwroot

run:
	cd ./Server/ && dotnet run


