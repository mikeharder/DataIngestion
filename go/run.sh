docker network create go-network
docker run -d --network=go-network -p 8080:8080 --name go-frontend go-frontend
docker run -d --network=go-network --name go-backend go-backend
