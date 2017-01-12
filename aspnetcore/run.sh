docker network create aspnetcore-network
docker run -d --network aspnetcore-network -p 8081:8080 --name aspnetcore-frontend aspnetcore-frontend
docker run -d --network aspnetcore-network --name aspnetcore-backend aspnetcore-backend

