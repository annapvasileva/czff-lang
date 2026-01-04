using System.Collections;
using Compiler.Lexer;

namespace Compiler.Tests;

public class WhitespacesCommentsTestsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // whitespaces
        yield return new object[] 
        { 
            "int ababb", 
            new List<Token>
            {
                new (TokenType.Integer, "int", 1, 1),
                new (TokenType.Identifier, "ababb", 1, 5),
                new (TokenType.Eof, "\0", 2, 1),
            }
        };
        
        yield return new object[] 
        { 
            "int     ababb", 
            new List<Token>
            {
                new (TokenType.Integer, "int", 1, 1),
                new (TokenType.Identifier, "ababb", 1, 9),
                new (TokenType.Eof, "\0", 2, 1),
            }
        };

        yield return new object[] 
        { 
            "int    ababb", 
            new List<Token>
            {
                new (TokenType.Integer, "int", 1, 1),
                new (TokenType.Identifier, "ababb", 1, 8),
                new (TokenType.Eof, "\0", 2, 1),
            }
        };

        yield return new object[] 
        { 
            """
            int
            ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 1, 1),
                new (TokenType.Identifier, "ababb", 2, 1),
                new (TokenType.Eof, "\0", 3, 1),
            }
        };

        yield return new object[] 
        { 
            """
            int
            
            
            ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 1, 1),
                new (TokenType.Identifier, "ababb", 4, 1),
                new (TokenType.Eof, "\0", 5, 1),
            }
        };
        
        // comments
        yield return new object[] 
        { 
            """
            // simple program
            int ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 2, 1),
                new (TokenType.Identifier, "ababb", 2, 5),
                new (TokenType.Eof, "\0", 3, 1),
            }
        };
        
        yield return new object[] 
        { 
            """
            =/ simple program /=
            int ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 2, 1),
                new (TokenType.Identifier, "ababb", 2, 5),
                new (TokenType.Eof, "\0", 3, 1),
            }
        };
        
        yield return new object[] 
        { 
            """
            =/
            simple program /=
            int ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 3, 1),
                new (TokenType.Identifier, "ababb", 3, 5),
                new (TokenType.Eof, "\0", 4, 1),
            }
        };
        
        yield return new object[] 
        { 
            """
            =/
                simple   program /=
            int ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 3, 1),
                new (TokenType.Identifier, "ababb", 3, 5),
                new (TokenType.Eof, "\0", 4, 1),
            }
        };
        
        yield return new object[] 
        { 
            """
            =/
            simple     program
            
            hello 
                /=
            int ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 6, 1),
                new (TokenType.Identifier, "ababb", 6, 5),
                new (TokenType.Eof, "\0", 7, 1),
            }
        };
        
        yield return new object[] 
        { 
            """
            =/
            simple     program
            hello /=
            
            
            int ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 6, 1),
                new (TokenType.Identifier, "ababb", 6, 5),
                new (TokenType.Eof, "\0", 7, 1),
            }
        };
        
        yield return new object[] 
        { 
            """
            int ababb // some  comments
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 1, 1),
                new (TokenType.Identifier, "ababb", 1, 5),
                new (TokenType.Eof, "\0", 2, 1),
            }
        };
        
        yield return new object[] 
        { 
            """
            int =/you can do this/= ababb
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 1, 1),
                new (TokenType.Identifier, "ababb", 1, 25),
                new (TokenType.Eof, "\0", 2, 1),
            }
        };

        yield return new object[] 
        { 
            """
            int =/you can do this/=ababb
            // here
            """, 
            new List<Token>
            {
                new (TokenType.Integer, "int", 1, 1),
                new (TokenType.Identifier, "ababb", 1, 24),
                new (TokenType.Eof, "\0", 3, 1),
            }
        };
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
