# Microsoft.Data.Sqlite-Sample

I create this sample to reproduce the [issue](https://github.com/aspnet/EntityFrameworkCore/issues/18328).

There are "geometry" and "geometryString" in the output of this sample. The "geometryString" is a string field that converted from "geometry"(byte array field) by the "CS_GeometryString" function. And I convert the "geometry" to string when I output it to the console. So they should be the same in the output. (But they don’t.) That means the "geometry" is not passed to the "CS_GeometryString" function correctly.

And it is strange that the preview version of the Microsoft.Data.Sqlite package(3.0.0-preview5.19227.1) doesn’t have this bug.
