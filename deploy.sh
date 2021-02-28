#!/bin/bash

. deploy.config

apt update
apt install -y apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
add-apt-repository \"deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable\"
apt update
apt-cache policy docker-ce
apt install -y docker-ce
docker login -u $GITHUB_USER -p $GITHUB_TOKEN
cd minitwit/Api
rm -r bin
rm -r publish
dotnet publish -c Release -o ./publish
docker build -t twooter .
docker tag twooter docker.pkg.github.com/themagicstrings/twooter/twooter
docker push docker.pkg.github.com/themagicstrings/twooter/twooter

ssh root@$HOST "
  apt update
  apt install -y apt-transport-https ca-certificates curl software-properties-common
  curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
  add-apt-repository \"deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable\"
  apt update
  apt-cache policy docker-ce
  apt install -y docker-ce
  docker login https://docker.pkg.github.com -u $GITHUB_USER -p $GITHUB_TOKEN
  docker stop twooter-instance
  docker rm twooter-instance
  docker rmi docker.pkg.github.com/themagicstrings/twooter/twooter:latest
  docker pull docker.pkg.github.com/themagicstrings/twooter/twooter:latest
  docker run --rm -p 443:443 -p 80:80 --name twooter-instance docker.pkg.github.com/themagicstrings/twooter/twooter:latest -e ASPNETCORE_URLS=\"http://localhost:80;https://localhost:443\""
