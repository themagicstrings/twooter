#!/bin/bash
# Update apt-get
apt-get update

# Setup .NET 5

wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb

sudo apt-get update
sudo apt-get install -y apt-transport-https
sudo apt-get update
sudo apt-get install -y dotnet-sdk-5.0
rm packages-microsoft-prod.deb
sudo apt install python-pytest

sudo apt-get install -y virtualbox virtualbox-ext-pack
wget -c https://releases.hashicorp.com/vagrant/2.2.14/vagrant_2.2.14_x86_64.deb
sudo dpkg -i vagrant_2.2.14_x86_64.deb
rm vagrant_2.2.14_x86_64.deb
vagrant plugin install vagrant-digitalocean
