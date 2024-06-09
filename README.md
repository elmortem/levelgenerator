# Level Generator

[![color:ff69b4](https://img.shields.io/badge/licence-MIT-blue)](https://opensource.org/license/mit)
![color:ff69b4](https://img.shields.io/badge/Unity-2019.3.x-red)

Node-based (xNode) procedural level generator.

## Installation

### Dependencies

Install xNode from https://github.com/siccity/xNode

### Package

Installation as a unity module via a git link in PackageManager or direct editing of `Packages/manifest' is supported.json:
```
"com.elmortem.levelgenerator": "https://github.com/elmortem/levelgenerator.git?path=Packages/levelgenerator",
```

## LevelGeneratorGraph asset

Create asset from Create menu selected Level Generator/Graph 

## Scene

Create GameObject on scene and add component LevelGeneratorSceneGraph and component LevelInstancer. Select your LevelGeneratorGraph.

## Generation

Open graph asset by double click and add nodes by right click. Connect nodes and finish all your work in Result node.

## Screenshot

<img src="screenshot.png" width="600">

### Other

Support Unity 2022.3 or later.

Enjoy!