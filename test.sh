#!/bin/bash

# Run model tests
cd minitwit/Models.Test
dotnet test
exitCode1=$?
testid=$!


# Start the api in background
cd ..
cd Api
nohup dotnet run > /dev/null &
apipid=$!
sleep 5s

# Run api tests
cd ..
cd Api.Test
pip3 install pytest
python3 -m pytest SimulationControllerTests.py
exitCode2=$?
python3 -m pytest MinitwitControllerTests.py
exitCode3=$?

# Stop the api
kill $apipid

if [ $exitCode1 -ne 0 -o $exitCode2 -ne 0 -o $exitCode3 -ne 0 ] ; then
    echo "Tests failed"
    exit 1
  else
    echo "Tests passed"
    exit 0
fi
