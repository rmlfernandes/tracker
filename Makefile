test-trackerapi:
	cd ./TrackerApi.Tests && dotnet test

test-trackerstorage:
	cd ./TrackerStorage.Tests && dotnet test

build-trackerapi:
	cd ./TrackerApi.Tests && dotnet build

build-trackerstorage:
	cd ./TrackerStorage.Tests && dotnet build

build-trackerapi-image:
	cd ./TrackerApi && docker build -t trackerapi .

build-trackerstorage-image:
	cd ./TrackerStorage && docker build -t trackerstorage .

tests: test-trackerapi test-trackerstorage

builds: build-trackerapi build-trackerstorage

start: build-trackerapi-image build-trackerstorage-image compose

compose:
	docker compose up -d

clean:
	docker compose stop && docker compose rm -f