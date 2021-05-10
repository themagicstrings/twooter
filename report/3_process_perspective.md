# Process' perspective
<!--
A description and illustration of:
  - How do you interact as developers?
  - How is the team organized?
  - A complete description of stages and tools included in the CI/CD chains.
    -  That is, including deployment and release of your systems.
  - Organization of your repositor(ies).
    - That is, either the structure of of mono-repository or organization of artifacts across repositories.
    - In essence, it has to be be clear what is stored where and why.
  - Applied branching strategy.
  - Applied development process and tools supporting it
    - For example, how did you use issues, Kanban boards, etc. to organize open tasks
  - How do you monitor your systems and what precisely do you monitor?
  - What do you log in your systems and how do you aggregate logs?
  - Brief results of the security assessment.
  - Applied strategy for scaling and load balancing.
In essence it has to be clear how code or other artifacts come from idea into the running system and everything that happens on the way.
!-->

## Logging solution
Our solution uses a logging tool for ASP.NET Core called NLog. This allows us to make seven levels of logs (DEBUG, ERROR, FATAL, INFO, OFF, TRACE, WARN), which are written to date stamped *.log*-files, formatted as defined in *nlog.config*. These log files are stored in a docker volume which is mounted to the docker container. To enable ourselves to access these logs, we have created a /logs/{h@dd-mm-yyyy} endpoint, which displays logs for one hour in a table format. Additionally, accessing the /logs endpoint, will redirect the user to the newest logs. For ease of analyzing the logs, it is possible to toggle the INFO level of logs on or off. 

Everything that is written to console will be logged by NLog. For example, uncaught exceptions will be logged as ERROR or FATAL, and the information printed when starting an ASP.NET Core application, is logged as INFO. In addition to what is automatically a part of the logging, the system writes an ERROR level log, whenever some request fails, containing information on why it failed. INFO level logs are also written when a request to post a message is received.

## Scaling and high availability strategy
### Database
The scalability of our database is provided by DigitalOcean.

~~For a scalable database solution, we use a postgresql database cluster via DigitalOcean. Moving to this solution came with a few benefits. The database management is handled entirely by DigitalOcean, including standby nodes with automatic switch over on failure for high availability. Additionally, we gained the monitoring that DigitalOcean provides and the ability to maintain the database and webserver, from the same interface.~~

### Webserver
Our web page is provided by a single DigitalOcean droplet server, accessed via a floating IP. Additionally, we have the DNS-name *twooter.hojelse.com* which points to the floating IP. 

We did not successfully create a high availability setup for this. We tried to setup a switch over system, using keepalived and the DigitalOcean API. 