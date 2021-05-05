# Lessons Learned Perspective
<!--
Describe the biggest issues, how you solved them, and which are major lessons learned with regards to:
  - evolution and refactoring 
  - operation, and
  - maintenance
of your _ITU-MiniTwit_ systems. Link back to respective commit messages, issues, tickets, etc. to illustrate these.
Also reflect and describe what was the "DevOps" style of your work. For example, what did you do differently to previous development projects and how did it work?
!-->

## Evolution of our database solution

When we first deployed our system, we used an in-memory database. This was naturally a flawed solution for any system that needs to persist data and will be redeployed with any frequency. 

We changed to a docker container running a MSSQL Server image, on a separate DigitalOcean droplet server. This solution did not work had some big issues. By default, MSSQL Server will try to keep as much data as it can in memory, to speed up queries. In our case the memory usage would steadily climb, until the container was starved for resources, and any operation would slow to a near halt causing response timeouts. 

Our attempt to fix this, was simply to not use a docker container, instead running as MSSQL Server directly on a droplet server. This did help reduce the speed at which the database would be starved, although it did still occur. To solve this we read quite a few articles on configureation issues that a MSSQL Server could have. One such issue, was that the default configuration had a maximum memory usage of around 2 TeraBytes, which is more than our server has. After correcting the configuration, it no longer would starve itself. 

This solution is however not scalable. Our final solution was a postgresql database cluster provided by DigitalOcean. Moving to this solution came with a few benefits. The database management is handled entirely by DigitalOcean, including standby nodes with automatic switch over on failure for high availability. Additionally, we gained the monitoring that DigitalOcean provides and the ability to maintain the database and webserver, from the same interface.

One additional note, is on the transition between different database management systems (i.e. MSSQL & PostgreSql). Changing between these does provide some issues, as the representaiton of data may differ. There may exist tools that would be able to transform a snapshot of one database to another. Our solution, however, was simply retrofitting our source code, with a "data siphon" and a connection to the old and the new database, launching the program on our own machines and transferring the data this way.