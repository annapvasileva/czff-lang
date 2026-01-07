using System.Collections;

namespace Compiler.Tests.Parser;

public class SyntaxErrorTestsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            """
            func void Main() { return; }
            
            var int a = 1;
            """,
            "Top-level statements can only include function declarations at line 3, column 1"
        };
        
        yield return new object[]
        {
            """
            func Main() {}
            """,
            "Type expected at line 1, column 6"
        };
        
        yield return new object[]
        {
            """
            func int() {}
            """,
            "Ожидался Identifier, но получен LeftRoundBracket at line 1, column 9"
        };
        
        yield return new object[]
        {
            """
            func void Main) {}
            """,
            "Ожидался LeftRoundBracket, но получен RightRoundBracket at line 1, column 15"
        };
        
        yield return new object[]
        {
            """
            func void Main( {}
            """,
            "Ожидался RightRoundBracket, но получен LeftCurlyBracket at line 1, column 17"
        };
        
        yield return new object[]
        {
            """
            func void Main() 
                var int a = 1;
            }
            """,
            "Ожидался LeftCurlyBracket, но получен Var at line 2, column 5"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                int a = 1;
            }
            """,
            "Ожидался expression at line 2, column 5"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var Point a = 1;
            }
            """,
            "Type expected at line 2, column 9"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var a = 1;
            }
            """,
            "Type expected at line 2, column 9"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a + ;
            }
            """,
            "Ожидался Semicolon, но получен Plus at line 2, column 15"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = ;
            }
            """,
            "Ожидался expression at line 2, column 17"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = 1 +;
            }
            """,
            "Ожидался expression at line 2, column 20"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = + 2;
            }
            """,
            "Ожидался expression at line 2, column 17"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = +;
            }
            """,
            "Ожидался expression at line 2, column 17"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = 1 + (2 + 3;
            }
            """,
            "Ожидался RightRoundBracket, но получен Semicolon at line 2, column 27"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = 2 5;
            }
            """,
            "Ожидался Semicolon, но получен IntegerLiteral at line 2, column 19"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                var int a = 2
            }
            """,
            "Ожидался Semicolon, но получен RightCurlyBracket at line 3, column 1"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                print
            }
            """,
            "Ожидался expression at line 3, column 1"
        };
        
        yield return new object[]
        {
            """
            func void Main() {
                print 123;
            """,
            "Ожидался RightCurlyBracket, но получен Eof at line 3, column 1"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
