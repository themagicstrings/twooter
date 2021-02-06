# Week 1

## Summary
In this week, we SSH'ed into a running ITUMinitwit server, to have a look around. Afterwards, we secure copied the server contents, in order to gain a copy of the running code and database.

## ./control.sh

The server had a [`./control.sh`](src/control.sh) script, with a few sub commands:

### ./control.sh init
Creates the database from the schema.

### ./control.sh start
Starts the server

### - ./control.sh inspectdb
View database contents

## Basic unix commands

## `ssh user@host`

**Don't ssh to this anymore**
```
ssh student@159.65.125.12
uiuiui
```

## `man <command>`
Open the manual for a command.

## `less <file>`
Less is a utility for reading file contents. It is good at opening and searching within large files.

You can pipe conent to `less` by using the pipe `|` operator.

Within `less`, you can use `/` to search and `q` to quit.

## `grep <term>`

Grep can be used to search std.input for text.

For example, to search currently running processes for a "minitwit", do the following:

```
ps aux | grep minitwit
```



# Week 2
