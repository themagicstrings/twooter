# Git ssh setup

## Edit .gitconfig

```
git config --global core.editor code
git config --global --edit
```

## Key
Get user email
`git config --global user.email`

Generate key
`ssh-keygen -t rsa -C "account@yourdomain.com"`

When asked don't input a file name

See public key
`cat ~/.ssh/id_rsa.pub`

## Add public key to
https://github.com/settings/keys

## Test if works
`ssh -T git@github.com`
