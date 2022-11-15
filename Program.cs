﻿using hell1;
using System.Diagnostics;
var w = new Stopwatch();
var generator = new Generator();
await generator.Generate(Configuration.InputFile, 10485760);
var sorter = new Sorter();
w.Start();
await sorter.Sort();
Console.WriteLine(TimeSpan.FromMilliseconds(w.ElapsedMilliseconds));
File.Delete(Configuration.InputFile);