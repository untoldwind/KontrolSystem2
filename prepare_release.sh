#!/bin/sh

version=$1

cd dist

rm *.zip

zip -r KontrolSystem2-${version}.zip BepInEx

gh release upload v${version} KontrolSystem2-${version}.zip
gh release upload v${version} ../Tools/vscode/to2-syntax/to2-syntax-0.0.1.vsix
gh release upload v${version} ../Tools/vscode/to2-syntax/to2-syntax-0.0.46.vsix
gh release upload v${version} ../Tools/vscode/to2-syntax/server/out/lsp-server.js
