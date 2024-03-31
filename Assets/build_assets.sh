#!/bin/sh

DIR=$(dirname $0)
UNITY_PATH=$HOME/Unity/Hub/Editor/2022.3.5f1

rm -rf $DIR/Assets/AssetBundles $DIR/Assets/AssetBundles.meta 

$UNITY_PATH/Editor/Unity -batchmode -quit -projectPath $DIR -executeMethod CreateAssetBundles.BuildAllAssetBundles

cp $DIR/Assets/AssetBundles/kontrolsystem2 $DIR/../SpaceWarpMod/Assets/kontrolsystem2.bundle
