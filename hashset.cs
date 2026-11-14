using System;
using System.Collections.Generic;

/*
 * Implement Linq filter method `NotDistinct` that returns only elements that already have been observed in source sequence.
 *
 * Example:
 *   from   [1, 2, 2, 3, 2, 5, 1, 5, 6]
 *   return [      2,    2,    1, 5   ]
 *
 */

var numbers = new[] { 1, 2, 2, 3, 2, 5, 1, 5, 6 };

foreach (var num in numbers.NotDistinct()) Console.WriteLine(num);


public static class MySweetExtensions{

	public static List<int> NotDistinct(this int[] array){
	    
		var result = new List<int>();
		var observed = new HashSet<int>();
		
		for(var i=0;i<array.Length;i++){
		  var observedCurrentCount = observed.Count;
		  var item = array[i];
		  observed.Add(item);
		  var updatedObservedCount = observed.Count;
			
		  if (updatedObservedCount == observedCurrentCount)
			  result.Add(item);
		
		}
		
	
		return result;
	}

}