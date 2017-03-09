docker network create go-nethttpserver-noop-network
docker run -d --network=go-nethttpserver-noop-network -p 8080:8080 -e NOOP=1 --name go-nethttpserver-noop-frontend go-frontend
