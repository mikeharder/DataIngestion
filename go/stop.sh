docker rm -f go-nethttp-frontend
docker rm -f go-nethttp-backend
docker network rm go-nethttp-network

docker rm -f go-fasthttp-frontend
docker rm -f go-fasthttp-backend
docker network rm go-fasthttp-network
