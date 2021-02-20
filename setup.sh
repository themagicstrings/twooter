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
apt install python-pytest

# Vagrant
apt-get install -y virtualbox virtualbox-ext-pack
wget -c https://releases.hashicorp.com/vagrant/2.2.14/vagrant_2.2.14_x86_64.deb
dpkg -i vagrant_2.2.14_x86_64.deb
rm vagrant_2.2.14_x86_64.deb
vagrant plugin install vagrant-digitalocean
