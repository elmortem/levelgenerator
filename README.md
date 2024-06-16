# Level Generator

[![color:ff69b4](https://img.shields.io/badge/licence-MIT-blue)](https://opensource.org/license/mit)
![color:ff69b4](https://img.shields.io/badge/Unity-2022.3.x-red)

Node-based (xNode) procedural level generator.

## Warning

I'm still looking for an ideal and fast data structure for the generator, so everything may change completely in the near future. For example, now I don’t like that the points are generated from BoundData. I would like BoundData to limit points after they are created (see BoundContainPointsNode). And I want to link the creation of points to surfaces, such as Plane, Terrain, Mesh, etc. Also, now the points are just Vector3, and the normal appears at the last stage in the descendants of the BaseVectorsNode node. This is not correct and will be redone. A separate structure will be created for the point, containing the point itself and a normal taken from the surface. And BaseVectorsNode will be converted into nodes that provide only a scale or something else... Perhaps the scale should also be immediately added to the structure of points so that nodes such as RemoveIntersectPointsNode can take it into account for better calculation of the intersection of points. In general, everything can change, if there are comments - welcome to Issues. Thank you.

## Installation

### Dependencies

Install xNode from https://github.com/siccity/xNode

### Packages

#### Level Generator

Installation as a unity module via a git link in PackageManager or direct editing of `Packages/manifest' is supported.json:
```
"com.elmortem.levelgenerator": "https://github.com/elmortem/levelgenerator.git?path=Packages/levelgenerator",
```

#### Splines Addon
Support splines for generate points and SpriteShapeInstanceData for make SpriteShapes

```
"com.elmortem.levelgenerator_splines": "https://github.com/elmortem/levelgenerator.git?path=Packages/levelgenerator_splines",
```

## LevelGeneratorGraph asset

Create asset from Create menu selected Level Generator/Graph 

## Scene

Create GameObject on scene and add component LevelGeneratorSceneGraph and component LevelInstancer. Select your LevelGeneratorGraph.

## Generation

Open graph asset by double click and add nodes by right click. Connect nodes and finish all your work in Result node.

Relax and [read wiki](https://github.com/elmortem/levelgenerator/wiki/) for more info (in progress...).

## Screenshot

<img src="screenshot.png" width="600">

### Other

Support Unity 2022.3 or later.

Enjoy!
