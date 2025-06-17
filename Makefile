.PHONY: build clean copy all

# Composite target: run all in order
all: clean build copy run

# Clean the wwwroot directory
clean:
	rm -rf ./Server/dist
	rm -rf ./Client/dist

# Build the Vue client app
build:
	cd ./Client && npm run build

# Copy the built files into the server's wwwroot
copy:
	cp -r ./Client/dist ./Server/dist

run:
	cd ./Server/ && npm run dev


