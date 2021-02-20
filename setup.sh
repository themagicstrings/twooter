#!/bin/bash

if [ "$EUID" -ne 0 ]
  then echo "Please use sudo"
  exit
fi


# Update apt-get
apt-get update
apt-update

# Setup .NET 5

wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get install -y apt-transport-https

apt-get install -y dotnet-sdk-5.0
rm packages-microsoft-prod.deb
apt install -y python-pytest

apt update
apt install -y apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu focal stable"
apt update
apt-cache policy docker-ce
apt install -y docker-ce
systemctl status docker
usermod -aG docker ${USER}
su - ${USER}
id -nG

