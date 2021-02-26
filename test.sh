#Run model tests
cd minitwit/models.Test
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
pytest SimulationControllerTests.py
pytest MinitwitControllerTests.py

#Stop the api
kill $apipid

$SHELL