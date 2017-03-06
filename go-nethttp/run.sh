docker network create go-nethttp-network
docker run -d --network=go-nethttp-network -p 8080:8080 --name go-nethttp-frontend go-nethttp-frontend
docker run -d --network=go-nethttp-network --name go-nethttp-backend go-nethttp-backend
