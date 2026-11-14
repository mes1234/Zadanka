using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
 * Fix code to allow it to properly handle DivideByZeroException and OverflowException (log a message)
 */

try
{
    Console.WriteLine(await CalculateAsync());
}
catch (OverflowException)
{
    Console.WriteLine("Calm down, it's just an overflow");
}

async  Task<int> CalculateAsync()
{
    try
    {
        return await   CalculateInternalAsync();
    }
    catch (DivideByZeroException e)
    {
        throw new OverflowException("Just ignore it", e);
    }
}

async Task<int> CalculateInternalAsync()
{
    var tasks = new List<Task<int>>
    {
        Task.FromResult(5),
        Task.FromResult(10),
        Task.FromException<int>(new DivideByZeroException()),
    };

    var results = await Task.WhenAll(tasks);
		
	return results.Sum();
}
