docker-compose down
# remove old kafka logs
docker volume rm $(docker volume ls -q | grep kafka)
docker-compose up
