# MutexLock

`MutexLock` class is a simple mutex getter for C#.

You can make cross-process exclusions.


# Usage

Can be used in combination with `using` statement.

```C#
using (var mutex = new MutexLock("mutexname", 1000))
{
  // exclusive processing
}
