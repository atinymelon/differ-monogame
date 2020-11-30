## What is differ-cs?

A port of the original differ for Haxe (https://github.com/underscorediscovery/differ). The port is (where possible) a one-to-one mapping, with a few changes to the API (no more out parameters). This is a [Separating Axis Theorom](http://en.wikipedia.org/wiki/Hyperplane_separation_theorem) collision library for C# games, intended for use in MonoGame.

----

## Details

- Supports polygons, circles, and rays currently
- 2D only
- **COLLISION ONLY.** No physics
- Project is a .net core library

##Quick look

**A simple collision example**
This is taken from the original, but this syntax is 100% compatible.

    var circle = new Circle( 300, 200, 50 );
    var box = Polygon.Rectangle( 0, 0, 50, 150 );

    box.Rotation = 45;

    if (circle.CollidesWith(box, out var collideInfo)) 
    {
        // collideInfo.Separation
        // collideInfo.Normal
        // collideInfo.Overlap
    }

