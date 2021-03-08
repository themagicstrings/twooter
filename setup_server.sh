#!/bin/bash
docker network create twooter-network
docker run --name dbserver -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$DB_PASSWORD" -d --net twooter-network -p 1433:1433 mcr.microsoft.com/mssql/server:2019-latest
77286670-16bd-4026-b4af-7d7b70b6e0e1
