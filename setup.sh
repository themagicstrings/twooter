#!/bin/sh
# Update apt-get
apt-get update

# Update apt
apt update

apt install gcc python3 libsqlite3-dev sqlitebrowser sqlite3 python3-pip
pip3 install flask
# Allows all to execute control.sh
chmod a+x ./src/control.sh
