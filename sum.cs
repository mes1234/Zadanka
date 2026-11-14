using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Unordered sequence of numbers misses one element. Find missing element without enumerating source sequence more than once.
 * Sequence may be very large. Avoid persisting it in list or array.
 * You know first element and length of original sequence.
 */

IEnumerable<int> Sequence()
{
    var elements = Enumerable.Range(1, 10000).ToArray();
    Random.Shared.Shuffle(elements);
    var missing = elements[0];
    Console.WriteLine($"(missing: {missing})");
    return elements.Skip(1);
}

var seq = Sequence();
var seqSum = 0L;
var cleanSum = 0L;
var counter = 1;

foreach(var item in seq){
  seqSum = seqSum + item;
  cleanSum =cleanSum + counter;
  counter++;	
}
  cleanSum = cleanSum + counter;




Console.WriteLine($"My pick is {cleanSum-seqSum}");