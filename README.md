
# ClifferBasic

ClifferBasic is a sample program for the Cliffer CLI library that implements a very simple BASIC interpreter environment as a REPL (Read-Eval-Print Loop). This project demonstrates the usage of the Cliffer CLI library to build a custom command-line application with an interactive BASIC-like language interpreter.

## Features

- Interactive REPL for executing BASIC-like commands.
- Supports variable assignment and arithmetic operations.
- Commands for printing, listing, saving, and loading programs.
- Extensible command structure using the Cliffer CLI library.

## Commands

### cls
Clear the screen

```basic
> cls
```

### delete
Delete a line from a program

```basic
> 10 print "Hello"
> 20 print "World"
> 30 end
> delete 20
> list
10 print "Hello"
30 end
> del 30
> list
10 print "Hello"
```

### end
End the execution of a program

```basic
> 10 print "A bit"
> 20 end
> 30 print "A bit more"
> run
A bit
```

### for
Repeat a section of code for a number of times

```basic
> 10 for i = 1 to 10
> 20 print i
> 30 next i
> run
1
2
3
4
5
6
7
8
9
10
```

### gosub
Jump to a subroutine

```basic
> 10 let x = 5
> 20 gosub 60
> 30 let x = 25
> 40 gosub 60
> 50 end
> 60 rem Square a number
> 70 print x * x
> 80 return
> run
25
625
```

### goto
Jump to a line and continue execution

```basic
> 10 goto 50
> 20 rem Square a number
> 30 print x * x
> 40 return
> 50 rem Execution continues here
> 60 let x = 5
> 70 gosub 20
> 80 let x = 25
> 90 gosub 20
> 100 end
> run
25
625
```

### if
Take an action conditionally based on a Boolean evaluation

```basic
> if 1 = 1 then print "All is well"
All is well
> if (2 + 2) = 5 then print "Something is wrong"
> if (2 + 2) = 4 then print "All is well again"
All is well again
```

### let
Assign a value to a variable.
```basic
> let x = 5.5
> print x
5.5
> let y# = 2
> print y#
2
> let z$ = "foo"
> print z$
foo
```

### list
List the current program in memory.
```basic
> 10 print "Hello, World!"
> 20 end
> list
10 print "Hello, World!"
20 end
```

### load
Load a program from persistent storage.

```basic
> load "hello.bas"
> list
10 print "Hello, World!"
20 end
> run
Hello, World!
```

### new
Clear the current program from memory

```basic
> 10 print "Hello, World!"
> run
Hello, World!
> new
> list
> run
>
```

### next
Return to the start of a for loop

```basic
> 10 for i = 1 to 10
> 20 print i
> 30 next i
> run
1
2
3
4
5
6
7
8
9
10
```
x
### print
Print text to the screen or evaluate an expression.
```basic
> print "Hello, World!"
Hello, World!
> print x + y#
7.5
> print z$
foo
```

### rem
Add a comment to the program.

```basic
> 10 rem This is a comment
> 20 print "This is a command"
> run
This is a command
```

### return
Return from a subroutine

```basic
> 10 let x = 5
> 20 gosub 60
> 30 let x = 25
> 40 gosub 60
> 50 end
> 60 rem Square a number
> 70 print x * x
> 80 return
> run
25
625
```

### run
Run the program currently in memory.

```basic
> 10 print "Hello, World!"
> 20 end
> run
Hello, World!
```

### save
Save the current program to persistent storage.
```basic
> 10 print "Hello, World!"
> 20 end
> save "hello.bas"
```

## Project Structure

- [`ClifferBasic.cs`](ClifferBasic.cs): Entry point of the application, sets up the command-line interface and services.
- [`BasicReplContext.cs`](BasicReplContext.cs): Custom REPL context for handling command input and execution.
- [`Commands`](Commands): Directory containing all CLI command implementations.
- [`Services`](Services): Directory containing supporting services.
- [`Model`](Model): Directory containing models classes.

## Example

Here's an example session with ClifferBasic:

```basic
> let x = 20.5
> let y# = 10
> print x + y#
30.5
> 20 print "World!"
> 10 print "Hello"
> 30 let x = 123
> 40 print x
> list
10 print "Hello"
20 print "World!"
30 let x = 123
40 print x
> save "program.bas"
> load "program.bas"
> run
```
