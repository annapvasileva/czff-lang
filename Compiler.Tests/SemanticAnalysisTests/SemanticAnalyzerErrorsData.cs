using System.Collections;

namespace Compiler.Tests.SemanticAnalysisTests;

public class SemanticAnalyzerErrorsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            """
            func void Main() {
                var void a = 1;

                return;
            }
            """,
            "Variable a: type - I does not match void"
        };

        yield return new object[]
        {
            """
            func void Main() {
                var array<void> a = 1;

                return;
            }
            """,
            "Variable a: type - I does not match [void"
        };

        yield return new object[]
        {
            """
            func void Main() {
                var array<int> a = 1;

                return;
            }
            """,
            "Variable a: type - I does not match [I"
        };

        yield return new object[]
        {
            """
            func void Main() {
                var array<int> a;

                return;
            }
            """,
            "You must provide an array size"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = new int(5)[];

                return;
            }
            """,
            "Variable a: type - [I does not match I"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a;
                return;
            }

            """,
            "Variable a is not initialized"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a;
                print a;
                a = 9;
                return;
            }

            """,
            "Variable a is not initialized"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a;
                return;
            }

            """,
            "Variable a is not initialized"
        };

        yield return new object[]
        {
            """
            func void Main() {
                var int a;
                return;
            }

            """,
            "Variable a is not initialized"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var array<int> arr = new int(5)[];
                var int a = arr;
                return;
            }

            """,
            "Variable a: type - [I does not match I"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                return 1;
            }

            """,
            "Function Main expected return type is void but got I"
        };
        
        yield return new object[]
        {
            """
            func int foo() {
                return;
            }
            
            func void Main() {
                return;
            }

            """,
            "Function foo expected return type is I but got void"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var array<int> arr = new int(5)[];
                var int a;
                a = arr;
                return;
            }

            """,
            "Assigment: identifier have type: I but got type: [I"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var array<int> arr = new int(5)[];
                var array<int> arr2 = new int(5)[];
                arr[0] = arr2;
                return;
            }

            """,
            "Assigment: array have type: I but got type: [I"
        };

        yield return new object[]
        {
            """
            func void Main() {
                var array<int> arr = new int(5)[];
                var array<int> arr2 = new int(arr)[];
                return;
            }

            """,
            "Array creation: size type is [I but must be I"
        };
        
         yield return new object[]
         {
             """
             func void foo() {
                 return;
             }
             
             func void Main() {
                 var array<int> arr = new int(foo)[];
                 return;
             }

             """,
             "Functions as types are not supported now"
         };

        yield return new object[]
        {
            """
            func void Main() {
                var array<int> arr = new int(5)[];
                var array<int> arr2 = new int(arr)[];
                return;
            }

            """,
            "Array creation: size type is [I but must be I"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var array<int> arr = new int(5)[];
                var array<int> arr2 = new int(3)[];
                arr2[arr] = 5;
                return;
            }

            """,
            "Array index: index type is [I but must be I"
        };
        
        yield return new object[]
        {
            """
            func void foo() {
                return;
            }
            
            func void Main() {
                var array<int> arr = new int(5)[];
                arr[foo] = 5;
                return;
            }

            """,
            "Functions as types are not supported now"
        };
    }

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
}