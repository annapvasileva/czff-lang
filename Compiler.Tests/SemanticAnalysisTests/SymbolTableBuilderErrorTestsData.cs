using System.Collections;

namespace Compiler.Tests.SemanticAnalysisTests;

public class SymbolTableBuilderErrorTestsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            """
            func void Main() {
                var int a = n;
                return; 
            }

            """,
            "Symbol n could not be found."
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                print n;
                return; 
            }

            """,
            "Symbol n could not be found."
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = 1 + b;
                var int b = 5;
                return; 
            }

            """,
            "Symbol b could not be found."
        };
        
        yield return new object[]
        {
            """
            func void foo() {
                var int a = 1;
                
                return;
            }

            func void Main() {
                print a;
                
                return;
            }

            """,
            "Symbol a could not be found."
        };

        yield return new object[]
        {
            """
            func void foo() {
                var int a = 1;
                
                return;
            }
            """,
            "Program does not contain the Main function"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = 1;
                var int a = 2;
                
                return;
            }
            """,
            "Variable a has been already declared"
        };
        
        yield return new object[]
        {
            """
            func void foo(int b) {
                var int b = 1;
                return;
            }
            func void Main() {
                return;
            }
            """,
            "Variable b has been already declared"
        };
        
        yield return new object[]
        {
            """
            func void foo(int c) {
                var array<int> c = new int(5)[];
                return;
            }
            func void Main() {
                return;
            }
            """,
            "Variable c has been already declared"
        };
    }

    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
}