## Naming convention C

**Local Variables**: camelCase  
**Const Variables**: ALL_CAPS  
**Global variables**: same as variables with prefix ==g_==  
**Pointers**: same as variables with prefix ==p_==  
**Structs**: ModulePascalCase (Module = full module name, or a 2-3 letter abbreviation, but still in PascalCase.)  
**Struct Member Variables**: camelCase  
**Enums**: ModulePascalCase  
**Enum Values**: ALL_CAPS  
**Typedefs**: ModulePascalCase  
**Functions**: PascalCase  
**Macros**: ALL_CAPS()  
**Define**: ALL_CAPS  
**Trivial Variables**: i,n,c,etc...  

## Intendation
Use 4 spaces.

## Coding
Functions
```C
int32_t foo () 
{
    ...
}
```

Oneline loops and statements
```C
while (1) 
{
    // Empty
}
```

Switch
```C
switch (condition) 
{
    case condition1: 
        // code
        break;
    case condition2:
        // code
        break;
    default:
        // code
        break;
}
```

If/else
```C
if (condition1) 
{
    // code
} 
else if (condition2) 
{
    // code
} 
else 
{
    // code
}
```

## Other notes:
Names with leading and trailing underscores are reserved for system purposes and should not be used for any user-created names  
Avoid names that differ only in case, like foo and Foo. Similarly, avoid foobar and foo_bar. The potential for confusion is considerable.  


## [Reference page link](https://www.doc.ic.ac.uk/lab/cplus/cstyle.html#N103FD)

## Naming convention C#

**Variables**: camelCase  
**Trivial Variables**: i,n,c,etc...  
**Enums**: PascalCase  
**Enum members**: PascalCase  
**Structs**: PascalCase  
**Classes**: PascalCase  
**Interfaces**: IPascalCase (use I as prefix)  
**Properties**: PascalCase  
**Private attributes**: camelCase  
**Methods**: PascalCase  
**Method parameters**: camelCase  
**Public members**: camelCase (class fields, record fields, ...)  
**Private members**: _camelCase (class fields, record fields, ...)  

**Classes library**: <library_name>Library

## Intendation
Use 4 spaces.

## Coding
Functions
```C#
int foo () 
{
    ...
}
```

Oneline loops and statements
```C#
while (1) 
{
    // Empty
}
```

Switch
```C#
switch (condition) 
{
    case condition1: 
        // code
        break;
    case condition2:
        // code
        break;
    default:
        // code
        break;
}
```

If/else
```C#
if (condition1) 
{
    // code
} 
else if (condition2) 
{
    // code
}
else 
{
    // code
}
```

Unit tests
```C#
public void ClassName_MethodName_ExpectedResult()
{
    // Arrange - initialize all cariables, classes, mocks

    // Act - execute function

    // Assert - check results

}
```

## Other notes:
Avoid names that differ only in case, like foo and Foo. Similarly, avoid foobar and foo_bar. The potential for confusion is considerable.  


## [Reference page link](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)