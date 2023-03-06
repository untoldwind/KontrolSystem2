#!/bin/sh

version=$1

cd dist

rm *.zip

zip -r KontrolSystem2-${version}.zip BepInEx

gh release upload v${version} KontrolSystem2-${version}.zip