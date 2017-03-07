docker network create go-nethttp-network
docker run -d --network=go-nethttp-network -p 8080:8080 --name go-nethttp-frontend go-frontend
docker run -d --network=go-nethttp-network --name go-nethttp-backend go-backend

docker network create go-fasthttp-network
docker run -d --network=go-fasthttp-network -p 8090:8080 -e FASTHTTP=1 --name go-fasthttp-frontend go-frontend
docker run -d --network=go-fasthttp-network --name go-fasthttp-backend go-backend
