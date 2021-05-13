# twooter
Twitter replica for DevOps course.

[![Test and Deploy](https://github.com/themagicstrings/twooter/actions/workflows/test-and-deploy.yml/badge.svg)](https://github.com/themagicstrings/twooter/actions/workflows/test-and-deploy.yml)
[![Coverage Status](https://coveralls.io/repos/github/themagicstrings/twooter/badge.svg?branch=main)](https://coveralls.io/github/themagicstrings/twooter?branch=main)
- [Week log](LOG.md)

## Setup

### Update and install dependancies
First, make `./setup.sh` executable

```
chmod a+x ./setup.sh
```
Now you can run setup.sh:
```
sudo ./setup.sh
```

### Add a database with sample data

Download [minitwit.db](https://github.com/themagicstrings/twooter/blob/124351635a81895ba5d488335600f2144712f8d4/tmp/minitwit.db?raw=true)

Place `minitwit.db` in `/tmp/`
