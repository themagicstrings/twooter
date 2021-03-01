#!/bin/bash

#Run model tests
cd minitwit/Models.Test
dotnet test

#Start the api in background
cd ..
cd Api
dotnet run &
sleep 5s
apipid=$!

#Run api tests
cd ..
cd Api.Test
pip3 install pytest
python3 -m pytest SimulationControllerTests.py
python3 -m pytest MinitwitControllerTests.py

#Stop the api
kill $apipid

$SHELL
