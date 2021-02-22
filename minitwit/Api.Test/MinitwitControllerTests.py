import os
import json
import base64
import sqlite3
import requests
from contextlib import closing


BASE_URL = 'http://127.0.0.1:5000'
DATABASE = "/tmp/minitwit.db"
USERNAME = 'simulator'
PWD = 'super_safe!'
CREDENTIALS = ':'.join([USERNAME, PWD]).encode('ascii')
ENCODED_CREDENTIALS = base64.b64encode(CREDENTIALS).decode()
HEADERS = {'Connection': 'close',
           'Content-Type': 'application/json',
           f'Authorization': f'Basic {ENCODED_CREDENTIALS}'}

def test_login_nonexisting_user():
    url = f"{BASE_URL}/login"
    data = {'username': 'nonuser', 'password': 'nonuser'}
    response = requests.post(url, data=json.dumps(data), headers=HEADERS)

    assert response.status_code == 400


def test_root_not_loggedin():
    url = f"{BASE_URL}/"
    response = requests.get(url, headers=HEADERS)

    assert response.ok